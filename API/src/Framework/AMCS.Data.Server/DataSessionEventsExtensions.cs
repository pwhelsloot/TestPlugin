using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server
{
  public static class DataSessionEventsExtensions
  {
    public static void AfterInsert(this IDataSessionEvents self, ISessionToken userId, EntityObject entityObject)
    {
      self.AfterInsert(userId, entityObject.GetType(), entityObject, entityObject.Id32, entityObject.GUID);
    }

    public static void AfterUpdate(this IDataSessionEvents self, ISessionToken userId, EntityObject entityObject)
    {
      AfterUpdate(self, userId, entityObject, DataUpdateKind.Update);
    }

    public static void AfterUpdate(this IDataSessionEvents self, ISessionToken userId, EntityObject entityObject, DataUpdateKind kind)
    {
      self.AfterUpdate(userId, entityObject.GetType(), entityObject, entityObject.Id32, entityObject.GUID, kind);
    }

    public static void AfterUpdate(this IDataSessionEvents self, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
    {
      self.AfterUpdate(userId, entityType, entity, id, guid, DataUpdateKind.Update);
    }

    public static void AfterDelete(this IDataSessionEvents self, ISessionToken userId, EntityObject entityObject)
    {
      self.AfterDelete(userId, entityObject.GetType(), entityObject, entityObject.Id32, entityObject.GUID);
    }

    public static void BeforeInsert(this IDataSessionEvents self, ISessionToken userId, EntityObject entityObject)
    {
      self.BeforeInsert(userId, entityObject.GetType(), entityObject);
    }

    public static void BeforeUpdate(this IDataSessionEvents self, ISessionToken userId, EntityObject entityObject, DataUpdateKind kind)
    {
      self.BeforeUpdate(userId, entityObject.GetType(), entityObject, entityObject.Id32, entityObject.GUID, kind);
    }

    public static void BeforeUpdate(this IDataSessionEvents self, ISessionToken userId, EntityObject entityObject)
    {
      self.BeforeUpdate(userId, entityObject.GetType(), entityObject, entityObject.Id32, entityObject.GUID, DataUpdateKind.Update);
    }

    public static void BeforeDelete(this IDataSessionEvents self, ISessionToken userId, EntityObject entityObject)
    {
      self.BeforeDelete(userId, entityObject.GetType(), entityObject, entityObject.Id32, entityObject.GUID);
    }
    
    public static void BeforeAndAfterUpdate(this IDataSessionEvents self, ISessionToken userId, EntityObject entityObject)
    {
      BeforeUpdate(self, userId, entityObject);
      AfterUpdate(self, userId, entityObject);
    }

    public static void BeforeAndAfterInsert(this IDataSessionEvents self, ISessionToken userId, EntityObject entityObject)
    {
      BeforeInsert(self, userId, entityObject);
      AfterInsert(self, userId, entityObject);
    }

    public static void BeforeAndAfterDelete(this IDataSessionEvents self, ISessionToken userId,
      EntityObject entityObject)
    {
      BeforeDelete(self, userId, entityObject);
      AfterDelete(self, userId, entityObject);
    }
  }
}
