using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server
{
  public interface IDataEvents
  {
    void BeforeInsert(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity);

    void BeforeUpdate(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind);

    void BeforeDelete(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid);

    void AfterInsert(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid);

    void AfterUpdate(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind);

    void AfterDelete(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid);
  }
}
