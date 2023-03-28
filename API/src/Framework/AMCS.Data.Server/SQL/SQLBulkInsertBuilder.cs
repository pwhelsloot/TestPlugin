using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AMCS.Data.Entity;
using Microsoft.SqlServer.Types;

namespace AMCS.Data.Server.SQL
{
  internal class SQLBulkInsertBuilder : SQLExecutable<ISQLBulkInsert>, ISQLBulkInsert
  {
    private const SqlBulkCopyOptions BulkCopyOptions = SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.FireTriggers;
    private const int DefaultBulkCopyTimeout = 600;

    private readonly SQLDataSession dataSession;
    private readonly ISessionToken userId;
    private readonly IList<EntityObject> entityObjects;

    public SQLBulkInsertBuilder(SQLDataSession dataSession, ISessionToken userId, IList<EntityObject> entityObjects)
    {
      this.dataSession = dataSession;
      this.userId = userId;
      this.entityObjects = entityObjects;
    }

    public List<Guid> Execute()
    {
      SQLStrictMode.ValidateInTransaction(dataSession);

      BeforeExecute();

      List<Guid> guids = new List<Guid>();
      var accessor = EntityObjectAccessor.ForType(entityObjects[0].GetType());

      using (SQLDataAccessHelper.CreatePerformanceLogger("DoBulkInsertEntityObject", accessor.Type.Name, IsBypassPerformanceLogging))
      {
        IList<string> columnsToReturn = GetColumnsForBulkInsert(accessor.TableName, accessor.SchemaName);

        var fullyQualifiedTableName = TableNameWithBrackets(accessor.TableNameWithSchema);

        using (var table = new DataTable(fullyQualifiedTableName))
        {
          // fill columns
          foreach (var column in columnsToReturn)
          {
            var property = accessor.GetPropertyByColumnName(column);

            if (property != null)
            {
              if (property.Type == typeof(GeographyPoint) || property.Type == typeof(GeographyPolygon))
              {
                var type = typeof(SqlGeography);
                table.Columns.Add(column, type);
              }
              else
              {
                table.Columns.Add(column, Nullable.GetUnderlyingType(property.Type) ?? property.Type);
              }
            }
          }

          // fill rows
          foreach (var entityObject in entityObjects)
          {
            var row = table.NewRow();

            var parameters = SQLPreparedParameters.ForInsert(entityObject, false, null, false).Parameters;

            foreach (var parameter in parameters)
            {
              row[parameter.ColumnName] = parameter.Parameter.Value;
              if (parameter.ColumnName == "GUID" && parameter.Parameter.Value != null)
              {
                guids.Add((Guid)parameter.Parameter.Value);
              }
            }

            table.Rows.Add(row);
          }

          using (var bulkCopy = new SqlBulkCopy(dataSession.Connection, BulkCopyOptions, dataSession.Transaction))
          {
            bulkCopy.BulkCopyTimeout = dataSession.Configuration.BulkCopyTimeout.GetValueOrDefault(DefaultBulkCopyTimeout);
            bulkCopy.DestinationTableName = fullyQualifiedTableName;

            foreach (string column in columnsToReturn)
            {
              var property = accessor.GetPropertyByColumnName(column);

              if (property != null)
                bulkCopy.ColumnMappings.Add(property.Column.ColumnName, column);
            }

            // Write from the source to the destination.
            bulkCopy.WriteToServer(table);
          }
        }
      }

      AfterExecute(guids);

      return guids;
    }

    private void BeforeExecute()
    {
      for (var i = 0; i < entityObjects.Count; i++)
      {
        var entityObject = entityObjects[i];
        dataSession.Events.BeforeInsert(userId, entityObject.GetType(), entityObject);
      }
    }

    private void AfterExecute(List<Guid> guids)
    {
      // Bulk save itself does not do auditing. There's a higher level entry point
      // that does do this itself.

      for (var i = 0; i < entityObjects.Count; i++)
      {
        var entityObject = entityObjects[i];
        dataSession.Events.AfterInsert(userId, entityObject.GetType(), entityObject, entityObject.Id32, entityObject.GUID ?? guids[i]);
      }
    }

    private IList<string> GetColumnsForBulkInsert(string tableName, string schemaName)
    {
      return dataSession.Query("SELECT * FROM dbo.fn_ColumnsForBulkInsert(@TableName,@SchemaName)")
        .Set("@TableName", tableName)
        .Set("@SchemaName", schemaName)
        .Execute()
        .List(record => record.Get<string>("ColumnName"));
    }

    private string TableNameWithBrackets(string tableNameWithSchema)
    {
      var split = tableNameWithSchema.Split('.');

      if (split.Length != 2)
        return tableNameWithSchema;

      var schema = split[0]
        .TrimStart('[')
        .TrimEnd(']');

      var tableName = split[1]
        .TrimStart('[')
        .TrimEnd(']');

      return $"[{schema}].[{tableName}]";
    }
  }
}
