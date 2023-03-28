using AMCS.Data;
using AMCS.Data.Entity;
using AMCS.Data.Entity.History;
using AMCS.Data.Server.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace AMCS.Data.Server
{
  public class EntityHistoryDataEvents : IDataEvents
  {
    public void AfterDelete(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
    {
      if (entity == null || !entity.SupportsTracking())
        return;

      var accessor = EntityObjectAccessor.ForType(entity.GetType());
      var builder = new EntityHistoryBuilder(entity, DataUpdateKind.Delete);

      if (accessor.TrackDeletes)
      {
        var hardDelete = !accessor.CanUndelete;
        string tableName = new SQLTextBuilder().TableName(entity.GetSchemaName(), entity.GetTableName()).ToString();
        string changes = builder.GetChangesForDelete();

        if (hardDelete)
        {
          SQLDataAccessHistory.InsertHistoryRecord(userId, tableName, entity.GetKeyName(), entity.Id32, changes, EntityHistoryTypeEnum.Delete, builder.GetParentTable(), builder.GetParentTableId(), DateTime.Now, dataSession);
        }
        else
        {
          SQLDataAccessHistory.InsertHistoryRecord(userId, tableName, entity.GetKeyName(), entity.Id32, changes, EntityHistoryTypeEnum.Update, builder.GetParentTable(), builder.GetParentTableId(), DateTime.Now, dataSession);
        }
      }

      builder.UpdateLoadedValues();
    }

    public void AfterInsert(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
    {
      if (entity == null || !entity.SupportsTracking())
        return;

      var accessor = EntityObjectAccessor.ForType(entity.GetType());
      var builder = new EntityHistoryBuilder(entity, DataUpdateKind.Insert);
      if (accessor.TrackInserts)
      {
        string changes = builder.GetChangesForInsert();
        if (changes != null)
        {
          string tableName = new SQLTextBuilder().TableName(entity.GetSchemaName(), entity.GetTableName()).ToString();
          SQLDataAccessHistory.InsertHistoryRecord(userId, tableName, entity.GetKeyName(), id, changes, EntityHistoryTypeEnum.Insert, builder.GetParentTable(), builder.GetParentTableId(), DateTime.Now, dataSession);
        }
      }
      builder.UpdateLoadedValues();
    }

    public void AfterUpdate(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind)
    {
      if (entity == null || !entity.SupportsTracking())
        return;

      var accessor = EntityObjectAccessor.ForType(entity.GetType());
      var builder = new EntityHistoryBuilder(entity, DataUpdateKind.Update);

      if (accessor.TrackUpdates)
      {
        if (kind == DataUpdateKind.Delete || kind == DataUpdateKind.Undelete)
        {
          AfterDelete(dataSession, userId, entityType, entity, id, guid);
          return;
        }

        string changes = builder.GetChangesForUpdate();
        if (changes != null)
        {
          string tableName = new SQLTextBuilder().TableName(entity.GetSchemaName(), entity.GetTableName()).ToString();
          SQLDataAccessHistory.InsertHistoryRecord(userId, tableName, entity.GetKeyName(), id, changes, EntityHistoryTypeEnum.Update, builder.GetParentTable(), builder.GetParentTableId(), DateTime.Now, dataSession);
        }
      }
      builder.UpdateLoadedValues();
    }

    void IDataEvents.BeforeDelete(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
    {
    }

    void IDataEvents.BeforeInsert(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity)
    {
    }

    void IDataEvents.BeforeUpdate(IDataSession dataSession, ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind)
    {
      if (entity == null)
        return;

      var accessor = EntityObjectAccessor.ForType(entity.GetType());
      if (accessor.TrackUpdates && entity.GetLoadedPropertyValues() == null)
      {
        var builder = new EntityHistoryBuilder(entity, DataUpdateKind.Update);
        builder.ReadLoadedValuesFromDB(dataSession, userId);
      }
    }
  }
}
