using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AMCS.Data.Entity;
using AMCS.Data.Entity.History;

namespace AMCS.Data.Server.SQL
{
  internal class SQLInsertCommandBuilder : SQLCommandBuilder<ISQLInsert>, ISQLInsert
  {
    private readonly ISessionToken userId;
    private readonly EntityObject entityObject;
    private bool identityInsert;
    private bool createAuditRecord;
    private bool insertOverridableDynamicColumns;
    private IList<string> restrictToFields;
    private string tableName;
    private bool returnScopeIdentity;
    private bool setIdentityInsertOn = true;

    public SQLInsertCommandBuilder(SQLDataSession dataSession, ISessionToken userId, EntityObject entityObject)
      : base(dataSession)
    {
      this.userId = userId;
      this.entityObject = entityObject;
    }

    public ISQLInsert IdentityInsert()
    {
      return IdentityInsert(true);
    }

    public ISQLInsert IdentityInsert(bool value)
    {
      identityInsert = value;
      return this;
    }

    public ISQLInsert SetIdentityInsertOn(bool value)
    {
      setIdentityInsertOn = value;
      return this;
    }

    public ISQLInsert CreateAuditRecord()
    {
      return CreateAuditRecord(true);
    }

    public ISQLInsert CreateAuditRecord(bool value)
    {
      createAuditRecord = value;
      return this;
    }

    public ISQLInsert InsertOverridableDynamicColumns()
    {
      return InsertOverridableDynamicColumns(true);
    }

    public ISQLInsert InsertOverridableDynamicColumns(bool value)
    {
      insertOverridableDynamicColumns = value;
      return this;
    }

    public ISQLInsert RestrictToFields(IList<string> fields)
    {
      restrictToFields = fields;
      return this;
    }

    public ISQLInsert TableName(string tableName)
    {
      this.tableName = tableName;
      return this;
    }

    protected override ISQLCommandFactory CreateCommandFactory()
    {
      var parameters = SQLPreparedParameters.ForInsert(
          entityObject, 
          identityInsert, 
          restrictToFields, 
          insertOverridableDynamicColumns)
        .Parameters;

      var sql = new SQLTextBuilder();

      if (identityInsert && setIdentityInsertOn)
      {
        sql.Text("SET IDENTITY_INSERT ");
        WriteTableName(sql);
        sql.Text(" ON; ");
      }

      sql.Text("INSERT INTO ");
      WriteTableName(sql);

      if (parameters.Count == 0)
      {
        // something like this will happen if we have an object/table with only an identity field.
        // TODO: This did not use tableName but entityObject.GetTableNameWithSchema() instead.

        sql.Text(" DEFAULT VALUES");
      }
      else
      {
        sql.Text(" (");

        for (var i = 0; i < parameters.Count; i++)
        {
          if (i > 0)
            sql.Text(", ");

          sql.Name(parameters[i].ColumnName);
        }

        sql.Text(") VALUES (");

        for (var i = 0; i < parameters.Count; i++)
        {
          if (i > 0)
            sql.Text(", ");

          sql.ParameterName(parameters[i].Parameter);
        }

        sql.Text(")");
      }

      if (identityInsert && setIdentityInsertOn)
      {
        sql.Text("; SET IDENTITY_INSERT ");
        WriteTableName(sql);
        sql.Text(" OFF");
      }
      if (returnScopeIdentity)
        sql.Text("; SELECT CAST(SCOPE_IDENTITY() AS INT) AS ID");

      return new SQLCommandFactory(
        CommandType.Text,
        sql.ToString(),
        parameters.Select(p => p.Parameter).ToList());
    }

    private void WriteTableName(SQLTextBuilder sql)
    {
      if (tableName != null)
        sql.Name(tableName);
      else
        sql.TableName(entityObject);
    }

    private void BeforeExecute()
    {
      DataSession.Events.BeforeInsert(userId, entityObject.GetType(), entityObject);
    }

    private void AfterExecute(int returnId)
    {
      if (entityObject.Id32 > 0)
        returnId = entityObject.Id32;

      DataSession.Events.AfterInsert(userId, entityObject.GetType(), entityObject, returnId, entityObject.GUID);

      if (createAuditRecord)
        SQLDataAccessAudit.InsertAuditRecord(userId, entityObject.GetTableName(), entityObject.GetKeyName(), returnId, SQLDataAuditChangeType.Insert, DataSession);
    }

    public void Execute()
    {
      returnScopeIdentity = false;

      BeforeExecute();

      DataSession.Metrics?.InsertBegin(DataSession.Connection);

      try
      {
        var result = ExecuteNonQuery();

        DataSession.Metrics?.InsertEnd(DataSession.Connection, result.Command, result.RowsAffected);
      }
      catch (Exception ex)
      {
        DataSession.Metrics?.InsertEnd(DataSession.Connection, null, 0, ex);
        throw;
      }

      AfterExecute(0);
    }

    public int ExecuteReturnIdentity()
    {
      returnScopeIdentity = true;

      DataSession.Metrics?.InsertBegin(DataSession.Connection);
      BeforeExecute();

      int returnId;

      try
      {
        var result = ExecuteScalar();
        returnId = (int)result.Value;

        DataSession.Metrics?.InsertEnd(DataSession.Connection, result.Command, 1);
      }
      catch (Exception ex)
      {
        DataSession.Metrics?.InsertEnd(DataSession.Connection, null, 0, ex);
        throw;
      }

      AfterExecute(returnId);

      return returnId;
    }
  }
}
