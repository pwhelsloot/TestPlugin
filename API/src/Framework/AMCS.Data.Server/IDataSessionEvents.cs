using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server
{
  public interface IDataSessionEvents
  {
    void BeforeInsert(ISessionToken userId, Type entityType, EntityObject entity);

    void BeforeUpdate(ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind);

    void BeforeDelete(ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid);

    void AfterInsert(ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid);

    void AfterUpdate(ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind);

    void AfterDelete(ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid);
  }
}
