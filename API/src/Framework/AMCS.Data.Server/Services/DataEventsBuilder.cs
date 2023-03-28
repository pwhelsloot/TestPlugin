using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using Autofac;

namespace AMCS.Data.Server.Services
{
  public class DataEventsBuilder
  {
    private readonly List<Func<IComponentContext, IDataEvents>> factories = new List<Func<IComponentContext, IDataEvents>>();

    public void Add(IDataEvents events)
    {
      AddFactory(_ => events);
    }

    public void AddFactory(Func<IComponentContext, IDataEvents> factory)
    {
      factories.Add(factory);
    }

    public IDataEvents Build(IComponentContext context)
    {
      return new DataEventsCollection(factories.Select(p => p(context)).ToArray());
    }

    private class DataEventsCollection : IDataEvents
    {
      private readonly IDataEvents[] events;

      public DataEventsCollection(IDataEvents[] events)
      {
        this.events = events;
      }

      public void AfterInsert(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
      {
        foreach (var @event in this.events)
        {
          @event.AfterInsert(dataSession, userId, entityType, entity, id, guid);
        }
      }

      public void AfterUpdate(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind)
      {
        foreach (var @event in this.events)
        {
          @event.AfterUpdate(dataSession, userId, entityType, entity, id, guid, kind);
        }
      }

      public void AfterDelete(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
      {
        foreach (var @event in this.events)
        {
          @event.AfterDelete(dataSession, userId, entityType, entity, id, guid);
        }
      }

      public void BeforeInsert(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity)
      {
        foreach (var @event in this.events)
        {
          @event.BeforeInsert(dataSession, userId, entityType, entity);
        }
      }

      public void BeforeUpdate(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind)
      {
        foreach (var @event in this.events)
        {
          @event.BeforeUpdate(dataSession, userId, entityType, entity, id, guid, kind);
        }
      }

      public void BeforeDelete(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
      {
        foreach (var @event in this.events)
        {
          @event.BeforeDelete(dataSession, userId, entityType, entity, id, guid);
        }
      }
    }
  }
}
