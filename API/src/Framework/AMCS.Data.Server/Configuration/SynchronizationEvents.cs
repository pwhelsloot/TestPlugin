using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using AMCS.Data.Server.Synchronisation;

namespace AMCS.Data.Server.Configuration
{
  internal class SynchronizationEvents : IDataEvents
  {
    private readonly SynchronisationRequestManager synchronisationRequestManager;

    public SynchronizationEvents(SynchronisationRequestManager synchronisationRequestManager)
    {
      this.synchronisationRequestManager = synchronisationRequestManager;
    }

    public void AfterInsert(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
    {
      if (id > 0)
      {
        var accessor = EntityObjectAccessor.ForType(entityType);

        synchronisationRequestManager.SynchroniseRecordInsertion(dataSession, userId, accessor.TableName, accessor.KeyName, id);
      }
    }

    public void AfterUpdate(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind)
    {
      if (id > 0)
      {
        var accessor = EntityObjectAccessor.ForType(entityType);

        synchronisationRequestManager.SynchroniseRecordUpdate(dataSession, userId, accessor.TableName, accessor.KeyName, id);
      }
    }

    public void AfterDelete(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
    {
      if (id > 0)
      {
        var accessor = EntityObjectAccessor.ForType(entityType);

        synchronisationRequestManager.SynchroniseRecordDeletion(dataSession, userId, accessor.TableName, accessor.KeyName, id);
      }
    }

    public void BeforeInsert(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity)
    {
    }

    public void BeforeUpdate(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind)
    {
    }

    public void BeforeDelete(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
    {
    }
  }
}