using System.Collections.Generic;
using System.Linq;
using AMCS.CommsServer.Client;
using AMCS.CommsServer.Serialization;
using AMCS.Data;
using AMCS.Data.Entity.CommsServer;
using AMCS.Data.Server;
using AMCS.Data.Server.CommsServer;

namespace AMCS.ApiService.CommsServer
{
  public class SqlServerStorage : IStorage
  {
    private readonly ISessionToken userId;

    public SqlServerStorage(ISessionToken userId)
    {
      this.userId = userId;
    }

    public IStorageAccess Open()
    {
      return new Access(this);
    }

    public void Dispose()
    {
      // Nothing to do.
    }

    private class Access : IStorageAccess
    {
      private readonly SqlServerStorage storage;
      private IDataSession session;
      private IDataSessionTransaction transaction;
      private bool disposed;

      public Access(SqlServerStorage storage)
      {
        this.storage = storage;
        session = BslDataSessionFactory.GetDataSession();
        transaction = session.CreateTransaction();
      }

      public void DeleteQueueInMessage(string messageId)
      {
        DataServices.Resolve<IQueueInService>().DeleteByMessageId(messageId, session);
      }

      public void SaveQueueOutMessage(IEnumerable<Message> messages)
      {
        var service = DataServices.Resolve<IQueueOutService>();

        foreach (var message in messages)
        {
          var entity = new QueueOutEntity
          {
            MessageId = message.Id,
            Type = message.Type,
            Body = message.Body,
            CorrelationId = message.CorrelationId
          };

          service.Save(storage.userId, entity, session);
        }
      }

      public void DeleteQueueOutMessages(IEnumerable<string> deliveryIds)
      {
        DataServices.Resolve<IQueueOutService>().DeleteAllById(deliveryIds.Select(int.Parse), session);
      }

      public void SaveQueueInMessages(IEnumerable<Message> messages)
      {
        var service = DataServices.Resolve<IQueueInService>();

        foreach (var message in messages)
        {
          if (service.HasMessageId(message.Id, session))
            continue;

          var entity = new QueueInEntity
          {
            MessageId = message.Id,
            Type = message.Type,
            Body = message.Body,
            CorrelationId = message.CorrelationId
          };

          service.Save(storage.userId, entity, session);
        }
      }

      public IStorageQueueResult GetQueueOutMessages(int batchSize, IStorageQueueOffset offset)
      {
        int? offsetId = (offset as StorageQueueOffset)?.Id;

        var messages = new List<ApiMessage>();
        int? id = null;

        foreach (var entity in DataServices.Resolve<IQueueOutService>().GetAllByOffset(offsetId, batchSize, session))
        {
          id = entity.QueueOutId;

          messages.Add(new ApiMessage(
            entity.MessageId,
            entity.QueueOutId.ToString(),
            entity.Type,
            entity.Body,
            entity.CorrelationId
          ));
        }

        if (id.HasValue)
          offset = new StorageQueueOffset(id.Value);

        return new StorageQueueResult(messages, offset);
      }

      public List<Message> GetQueueInMessages()
      {
        var messages = new List<Message>();

        foreach (var entity in DataServices.Resolve<IQueueInService>().GetAll(session))
        {
          messages.Add(new Message(entity.MessageId, entity.Type, entity.Body, entity.CorrelationId));
        }

        return messages;
      }

      public void Commit()
      {
        transaction.Commit();
      }

      public void Dispose()
      {
        if (!disposed)
        {
          if (transaction != null)
          {
            transaction.Dispose();
            transaction = null;
          }
          if (session != null)
          {
            session.Dispose();
            session = null;
          }

          disposed = true;
        }
      }

      private class StorageQueueResult : IStorageQueueResult
      {
        public List<ApiMessage> Messages { get; }
        public IStorageQueueOffset Offset { get; }

        public StorageQueueResult(List<ApiMessage> messages, IStorageQueueOffset offset)
        {
          Messages = messages;
          Offset = offset;
        }
      }

      private class StorageQueueOffset : IStorageQueueOffset
      {
        public int Id { get; }

        public StorageQueueOffset(int id)
        {
          Id = id;
        }
      }
    }
  }
}
