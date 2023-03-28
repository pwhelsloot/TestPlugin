using System;
using System.Data;
using System.Data.SqlClient;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.SQL
{
  internal class SQLDeleteCommandBuilder : SQLCommandBuilder<ISQLDelete>, ISQLDelete
  {
    private readonly ISessionToken userId;
    private readonly EntityObject entityObject;
    private bool undelete;
    private bool createAuditRecord;

    public SQLDeleteCommandBuilder(SQLDataSession dataSession, ISessionToken userId, EntityObject entityObject)
      : base(dataSession)
    {
      this.userId = userId;
      this.entityObject = entityObject;
    }

    public ISQLDelete CreateAuditRecord()
    {
      return CreateAuditRecord(true);
    }

    public ISQLDelete CreateAuditRecord(bool value)
    {
      createAuditRecord = value;
      return this;
    }

    public ISQLDelete Undelete()
    {
      return Undelete(true);
    }

    public ISQLDelete Undelete(bool value)
    {
      undelete = value;
      return this;
    }

    protected override ISQLCommandFactory CreateCommandFactory()
    {
      var accessor = EntityObjectAccessor.ForType(entityObject.GetType());
      var sql = new SQLTextBuilder();

      if (!accessor.CanUndelete)
      {
        if (undelete)
          throw new NotSupportedException("It is not possible to undelete objects of type '" + accessor.Type.Name + "'");

        sql
          .Text("DELETE FROM ").TableName(accessor)
          .Text(" WHERE ").Name(accessor.KeyName).Text(" = @keyValue");
      }
      else
      {
        if (undelete)
        {
          sql
            .Text("UPDATE ").TableName(accessor)
            .Text(" SET IsDeleted = 0 WHERE ").Name(accessor.KeyName).Text(" = @keyValue");
        }
        else
        {
          sql
            .Text("UPDATE ").TableName(accessor)
            .Text(" SET IsDeleted = 1 WHERE ").Name(accessor.KeyName).Text(" = @keyValue");
        }
      }

      return new SQLCommandFactory(CommandType.Text, sql.ToString(), new SqlParameter("@keyValue", entityObject.Id));
    }

    public bool Execute()
    {      
      var accessor = EntityObjectAccessor.ForType(entityObject.GetType());
      var hardDelete = !accessor.CanUndelete;

      BeforeExecute(hardDelete);

      if (hardDelete)
      {
        InsertAuditRecord(accessor);

        DataSession.Metrics?.DeleteBegin(DataSession.Connection);

        try
        {
          var result = ExecuteNonQuery();

          DataSession.Metrics?.DeleteEnd(DataSession.Connection, result.Command, result.RowsAffected);
        }
        catch (Exception ex)
        {
          DataSession.Metrics?.DeleteEnd(DataSession.Connection, null, 0, ex);
          throw;
        }
      }

      else
      {
        DataSession.Metrics?.SoftDeleteBegin(DataSession.Connection);

        try
        {
          var result = ExecuteNonQuery();

          DataSession.Metrics?.SoftDeleteEnd(DataSession.Connection, result.Command, result.RowsAffected);
        }
        catch (Exception ex)
        {
          DataSession.Metrics?.SoftDeleteEnd(DataSession.Connection, null, 0, ex);
          throw;
        }

        InsertAuditRecord(accessor);
      }

      AfterExecute(hardDelete);

      return hardDelete;
    }

    private void BeforeExecute(bool hardDelete)
    {
      if (hardDelete)
      {
        DataSession.Events.BeforeDelete(userId, entityObject.GetType(), entityObject, entityObject.Id32, entityObject.GUID);
      }
      else
      {
        var kind = undelete ? DataUpdateKind.Undelete : DataUpdateKind.Delete;
        DataSession.Events.BeforeUpdate(userId, entityObject.GetType(), entityObject, entityObject.Id32, entityObject.GUID, kind);
      }
    }

    private void AfterExecute(bool hardDelete)
    {
      if (hardDelete)
      {
        DataSession.Events.AfterDelete(userId, entityObject.GetType(), entityObject, entityObject.Id32, entityObject.GUID);
      }
      else
      {
        var kind = undelete ? DataUpdateKind.Undelete : DataUpdateKind.Delete;
        DataSession.Events.AfterUpdate(userId, entityObject.GetType(), entityObject, entityObject.Id32, entityObject.GUID, kind);
      }
    }

    private void InsertAuditRecord(EntityObjectAccessor accessor)
    {
      if (createAuditRecord)
      {
        var changeType = undelete ? SQLDataAuditChangeType.Update : SQLDataAuditChangeType.Delete;

        SQLDataAccessAudit.InsertAuditRecord(userId, accessor.TableName, accessor.KeyName, entityObject.Id32, changeType, DataSession);
      }
    }
  }
}
