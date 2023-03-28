using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using Autofac;

namespace AMCS.Data.Server.Services
{
  public class DataEventsBuilderService : IDataEventsBuilderService
  {
    private readonly List<IDataEvents> events = new List<IDataEvents>();
    private bool isBuilt;

    public void Add(IDataEvents events)
    {
      if (isBuilt)
        throw new InvalidOperationException("Events cannot be added after the builder has been built");

      this.events.Add(events);
    }

    public IDataEvents Build()
    {
      isBuilt = true;
      return new DataEventsCollection(events.ToArray());
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
