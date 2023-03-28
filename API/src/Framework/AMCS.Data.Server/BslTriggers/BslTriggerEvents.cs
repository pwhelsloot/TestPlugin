using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.BslTriggers
{
  public class BslTriggerEvents : IDataEvents
  {
    private readonly IBslTriggerManager triggerManager;

    public BslTriggerEvents(IBslTriggerManager triggerManager)
    {
      this.triggerManager = triggerManager;
    }

    private void Raise(IDataSession dataSession, ISessionToken userId, Type entityType, int id, Guid? guid, BslAction action, EntityObject entityObject)
    {
      triggerManager.RaiseEntity(dataSession, userId, entityType, action, id, guid, entityObject);
    }

    public void BeforeInsert(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity)
    {
      Raise(dataSession, userId, entityType, 0, Guid.Empty, BslAction.BeforeCreate, entity);
    }

    public void BeforeUpdate(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind)
    {
      var action = kind == DataUpdateKind.Delete
        ? BslAction.BeforeDelete
        : BslAction.BeforeUpdate;

      Raise(dataSession, userId, entityType, id, guid, action, entity);
    }

    public void BeforeDelete(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
    {
      Raise(dataSession, userId, entityType, id, guid, BslAction.BeforeDelete, entity);
    }

    public void AfterInsert(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
    {
      Raise(dataSession, userId, entityType, id, guid, BslAction.Create, null);
    }

    public void AfterUpdate(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind)
    {
      var action = kind == DataUpdateKind.Delete
        ? BslAction.Delete
        : BslAction.Update;

      Raise(dataSession, userId, entityType, id, guid, action, null);
    }

    public void AfterDelete(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
    {
      Raise(dataSession, userId, entityType, id, guid, BslAction.Delete, null);
    }
  }
}
