using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using AMCS.Data.Entity;
using AMCS.Data.Entity.Search;
using AMCS.Data.Entity.SQL;
using log4net;
using Microsoft.SqlServer.Server;

namespace AMCS.Data.Server.SQL
{
  public static class SQLDataAccessHelper
  {
    #region Properties/Variables

    private static readonly ILog Logger = LogManager.GetLogger(typeof(SQLDataAccessHelper));

    #endregion Properties/Variables

    #region insertcommands

    /// <summary>
    /// Inserts entity object and returns the identiy
    /// </summary>
    /// <param name="dataSession">the data session to perform insert under</param>
    /// <param name="userId">user id performing the insert</param>
    /// <param name="entityObject">entity object to insert</param>
    /// <returns>Identity value inserted</returns>
    public static int InsertEntityObjectReturnIdentity(IDataSession dataSession, ISessionToken userId, EntityObject entityObject)
    {
      return dataSession.Insert(userId, entityObject)
        .CreateAuditRecord()
        .ExecuteReturnIdentity();
    }

    /// <summary>
    /// Bulk inserts entity objects (No Audit)
    /// </summary>
    /// <param name="dataSession">the data session to perform insert under</param>
    /// <param name="userId">user id performing the insert</param>
    /// <param name="entityObject">entity object to insert</param>
    public static void BulkInsertEntityObjects(IDataSession dataSession, ISessionToken userId, IList<EntityObject> entityObjects)
    {
      dataSession.BulkInsert(userId, entityObjects).Execute();
    }

    /// <summary>
    /// Bulk inserts entity objects returns object guid
    /// </summary>
    /// <param name="dataSession">the data session to perform insert under</param>
    /// <param name="userId">user id performing the insert</param>
    /// <param name="entityObject">entity object to insert</param>
    public static List<Guid> BulkInsertEntityObjectsReturnIdentity(IDataSession dataSession, ISessionToken userId, IList<EntityObject> entityObjects)
    {
      return dataSession.BulkInsert(userId, entityObjects).Execute();
    }

    /// <summary>
    /// Inserts entity object and returns the identiy
    /// </summary>
    /// <param name="dataSession">the data session to perform insert under</param>
    /// <param name="userId">user id performing the insert</param>
    /// <param name="entityObject">entity object to insert</param>
    /// <param name="retrictToFields">only these fields are included </param>
    /// <returns>Identity value inserted</returns>
    public static int InsertEntityObjectReturnIdentity(IDataSession dataSession, ISessionToken userId, EntityObject entityObject, string[] retrictToFields, bool bypassPerformanceLogging = false)
    {
      return dataSession.Insert(userId, entityObject)
        .RestrictToFields(retrictToFields)
        .CreateAuditRecord()
        .BypassPerformanceLogging(bypassPerformanceLogging)
        .ExecuteReturnIdentity();
    }

    /// <summary>
    /// Inserts entity object and does not return identiy.
    /// </summary>
    /// <param name="dataSession">the data session to perform insert under</param>
    /// <param name="userId">user id performing the insert</param>
    /// <param name="entityObject">entity object to insert</param>
    public static void InsertEntityObject(IDataSession dataSession, ISessionToken userId, EntityObject entityObject)
    {
      dataSession.Insert(userId, entityObject)
        .Execute();
    }

    public static void InsertEntityObject(IDataSession dataSession, ISessionToken userId, EntityObject entityObject, string tableName, bool insertOverridableDynamicColumns = false)
    {
      dataSession.Insert(userId, entityObject)
        .TableName(tableName)
        .InsertOverridableDynamicColumns(insertOverridableDynamicColumns)
        .IdentityInsert()
        .Execute();
    }

    /// <summary>
    /// Inserts entity object and specifies the identity value
    /// </summary>
    /// <param name="dataSession">the data session to perform insert under</param>
    /// <param name="userId">user id performing the insert</param>
    /// <param name="entityObject">entity object to insert</param>
    public static void InsertEntityObjectWithIdentity(IDataSession dataSession, ISessionToken userId, EntityObject entityObject)
    {
      dataSession.Insert(userId, entityObject)
        .IdentityInsert()
        .Execute();
    }

    /// <summary>
    /// Inserts entity object and specifies the identity value but does not set identity_insert to true
    /// This is required when inserting identities into tables that do not have an identity property
    /// </summary>
    /// <param name="dataSession">the data session to perform insert under</param>
    /// <param name="userId">user id performing the insert</param>
    /// <param name="entityObject">entity object to insert</param>
    public static void InsertEntityObjectOverrideSetIdentity(IDataSession dataSession, ISessionToken userId, EntityObject entityObject)
    {
      dataSession.Insert(userId, entityObject)
        .IdentityInsert()
        .SetIdentityInsertOn(false)
        .Execute();
    }

    #endregion insertcommands

    #region updatecommands

    /// <summary>
    /// Updates entity object and specifies the identity value
    /// </summary>
    /// <param name="dataSession">the data session to perform update under</param>
    /// <param name="userToken">user token performing the update</param>
    /// <param name="entityObject">entity object to update</param>
    /// <param name="specialFields">special fields to be handled</param>
    /// <param name="ignoreSpecialFields">ignore special fields</param>
    public static void UpdateEntityObject(IDataSession dataSession, ISessionToken userToken, EntityObject entityObject, string[] specialFields, bool ignoreSpecialFields)
    {
      UpdateEntityObject(dataSession, userToken, entityObject, specialFields, ignoreSpecialFields, null);
    }

    /// <summary>
    /// Updates entity object and specifies the identity value
    /// </summary>
    /// <param name="dataSession">the data session to perform update under</param>
    /// <param name="userToken">user token performing the update</param>
    /// <param name="entityObject">entity object to update</param>
    /// <param name="specialFields">special fields to be handled</param>
    /// <param name="ignoreSpecialFields">ignore special fields</param>
    public static void UpdateEntityObject(IDataSession dataSession, ISessionToken userToken, EntityObject entityObject, string[] specialFields, bool ignoreSpecialFields, string[] retrictToFields, bool updateOverridableDynamicColumns = false, bool bypassPerformanceLogging = false)
    {
      dataSession.Update(userToken, entityObject)
        .CreateAuditRecord()
        .SpecialFields(specialFields)
        .IgnoreSpecialFields(ignoreSpecialFields)
        .RestrictToFields(retrictToFields)
        .UpdateOverridableDynamicColumns(updateOverridableDynamicColumns)
        .BypassPerformanceLogging(bypassPerformanceLogging)
        .Execute();
    }

    #endregion updatecommands

    #region deletecommands

    /// <summary>
    /// Deletes entity object and specifies the identity value
    /// </summary>
    /// <param name="dataSession">the data session to perform delete under</param>
    /// <param name="userId">user id performing the delete</param>
    /// <param name="entityObject">entity object to delete</param>
    /// <param name="isUndelete">is the operation an undelete</param>
    /// <returns>true if done fals other wise</returns>
    public static bool DeleteEntityObject(IDataSession dataSession, ISessionToken userId, EntityObject entityObject, bool isUndelete, bool bypassPerformanceLogging = false)
    {
      return dataSession.Delete(userId, entityObject)
        .CreateAuditRecord()
        .Undelete(isUndelete)
        .BypassPerformanceLogging(bypassPerformanceLogging)
        .Execute();
    }

    #endregion deletecommands

    #region readcommands

    [Obsolete("Use new ORM system")]
    public static IDataReader ExecuteReader(IDataSession dataSession, DbCommand cmd, bool bypassPerformanceLogging = false)
    {
      return ExecuteReader(dataSession, cmd, CommandBehavior.Default, false, bypassPerformanceLogging);
    }

    [Obsolete("Use new ORM system")]
    public static IDataReader ExecuteReaderWithExtendedTimeout(IDataSession dataSession, DbCommand cmd, bool bypassPerformanceLogging = false)
    {
      return ExecuteReader(dataSession, cmd, CommandBehavior.Default, true, bypassPerformanceLogging);
    }

    private static IDataReader ExecuteReader(IDataSession dataSession, DbCommand cmd, CommandBehavior behavior, bool useExtendedTimeout, bool bypassPerformanceLogging = false)
    {
      SQLStrictMode.ValidateInTransaction(dataSession);

      using (CreatePerformanceLogger("ExecuteReader", cmd.CommandText, bypassPerformanceLogging))
      {
        var sqlDataSession = (SQLDataSession)dataSession;

        cmd.Connection = sqlDataSession.Connection;
        cmd.Transaction = sqlDataSession.Transaction;

        if (useExtendedTimeout && sqlDataSession.Configuration.CommandTimeoutExtended.HasValue)
          cmd.CommandTimeout = sqlDataSession.Configuration.CommandTimeoutExtended.Value;
        else if (sqlDataSession.Configuration.CommandTimeout.HasValue)
          cmd.CommandTimeout = sqlDataSession.Configuration.CommandTimeout.Value;

        return cmd.ExecuteReader(behavior);
      }
    }

    [Obsolete("Use new ORM system")]
    public static DataTable GetDataTable(IDataSession dataSession, DbCommand cmd, bool bypassPerformanceLogging = false)
    {
      using (CreatePerformanceLogger("GetDataTable", cmd.CommandText, bypassPerformanceLogging))
      using (IDataReader rdr = ExecuteReader(dataSession, cmd, bypassPerformanceLogging))
      {
        DataTable result = new DataTable();
        result.Load(rdr);
        return result;
      }
    }

    #endregion readcommands

    #region executeNonQuery

    [Obsolete("Use new ORM system")]
    public static int ExecuteNonQuery(IDataSession dataSession, DbCommand cmd, bool useExtendedTimeout = false)
    {
      SQLStrictMode.ValidateInTransaction(dataSession);

      using (CreatePerformanceLogger("ExecuteNonQuery", cmd.CommandText, false))
      {
        NullifyParameters(cmd.Parameters);

        var sqlDataSession = (SQLDataSession)dataSession;

        cmd.Connection = sqlDataSession.Connection;
        cmd.Transaction = sqlDataSession.Transaction;

        if (useExtendedTimeout && sqlDataSession.Configuration.CommandTimeoutExtended.HasValue)
          cmd.CommandTimeout = sqlDataSession.Configuration.CommandTimeoutExtended.Value;
        else if (sqlDataSession.Configuration.CommandTimeout.HasValue)
          cmd.CommandTimeout = sqlDataSession.Configuration.CommandTimeout.Value;

        return cmd.ExecuteNonQuery();
      }
    }

    [Obsolete("Use new ORM system")]
    public static object ExecuteScaler(IDataSession dataSession, DbCommand cmd)
    {
      SQLStrictMode.ValidateInTransaction(dataSession);

      using (CreatePerformanceLogger("ExecuteScalar", cmd.CommandText, false))
      {
        NullifyParameters(cmd.Parameters);

        var sqlDataSession = (SQLDataSession)dataSession;

        cmd.Connection = sqlDataSession.Connection;
        cmd.Transaction = sqlDataSession.Transaction;

        if (sqlDataSession.Configuration.CommandTimeout.HasValue)
        {
          cmd.CommandTimeout = sqlDataSession.Configuration.CommandTimeout.Value;
        }

        return cmd.ExecuteScalar();
      }
    }

    #endregion executeNonQuery

    #region search

    [Obsolete("Use new ORM system")]
    public static GridSearchResultsEntity GetGridSearchResultsFromReader(string searchResultsId, IDataReader rdr, string[] ignoreColumns = null)
    {
      GridSearchResultsEntity result = new GridSearchResultsEntity();
      result.Id = searchResultsId;

      DataTable schemaTable = rdr.GetSchemaTable();

      for (int i = 0; i < rdr.FieldCount; i++)
      {
        if (ignoreColumns != null && ignoreColumns.Contains(rdr.GetName(i))) continue;
        GridColumnDefinition columnDefinition = new GridColumnDefinition
        {
          ParentId = searchResultsId,
          Path = rdr.GetName(i),
          Header = rdr.GetName(i),
          DataType = (schemaTable.Rows[i]["DataType"] as Type).FullName,
          Visible = true
        };

        result.ColumnDefinitions.Add(columnDefinition);
      }

      while (rdr.Read())
      {
        dynamic row = new SerializableDynamicObject();
        for (int i = 0; i < rdr.FieldCount; i++)
        {
          if (ignoreColumns != null && ignoreColumns.Contains(rdr.GetName(i))) continue;
          object value = rdr[i];
          if (value is DBNull)
            value = null;
          row[rdr.GetName(i)] = value;
        }
        result.Collection.Add(row);
      }

      return result;
    }

    #endregion search

    #region private

    #region helpercommands

    [Obsolete("Use SetIdList in the new ORM system")]
    public static IList<SqlDataRecord> ConvertIdListToTableType(string name, IList<int> list)
    {
      List<SqlDataRecord> ids = new List<SqlDataRecord>();

      SqlMetaData[] metaData = { new SqlMetaData(name, SqlDbType.Int) };
      if (list != null && list.Count > 0)
      {
        foreach (int id in list.Distinct())
        {
          SqlDataRecord row = new SqlDataRecord(metaData);
          row.SetInt32(0, id);
          ids.Add(row);
        }
        return ids;
      }
      return null;
    }

    [Obsolete("Use SetIdList in the new ORM system")]
    public static IList<SqlDataRecord> ConvertIdListToTableType(string name, IList<Guid> list)
    {
      List<SqlDataRecord> ids = new List<SqlDataRecord>();

      SqlMetaData[] metaData = { new SqlMetaData(name, SqlDbType.UniqueIdentifier) };
      if (list != null && list.Count > 0)
      {
        foreach (Guid id in list.Distinct())
        {
          SqlDataRecord row = new SqlDataRecord(metaData);
          row.SetSqlGuid(0, id);
          ids.Add(row);
        }
        return ids;
      }
      return null;
    }

    [Obsolete("Use SetStringList in the new ORM system")]
    public static IList<SqlDataRecord> ConvertStringListToTableType(string name, IList<string> list)
    {
      List<SqlDataRecord> values = new List<SqlDataRecord>();

      SqlMetaData[] metaData = { new SqlMetaData(name, SqlDbType.NVarChar, -1) };
      if (list != null && list.Count > 0)
      {
        foreach (string value in list)
        {
          SqlDataRecord row = new SqlDataRecord(metaData);
          if (value == null)
            row.SetDBNull(0);
          else
            row.SetSqlString(0, value);
          values.Add(row);
        }
        return values;
      }
      return null;
    }

    private static void NullifyParameters(DbParameterCollection parameters)
    {
      foreach (SqlParameter param in parameters)
      {
        if (param.Value == null)
        {
          param.Value = DBNull.Value;
        }
      }
    }

    #endregion helpercommands

    #region GuidCommands

    public static int? GetIntIdFromGUID(Guid? guid, Type entityType, IDataSession dataSession)
    {
      var accessor = EntityObjectAccessor.ForType(entityType);

      string sql = new SQLTextBuilder()
        .Text("SELECT ").Name(accessor.KeyName)
        .Text(" FROM ").TableName(accessor)
        .Text(" WHERE GUID = @GUID")
        .ToString();

      return (int?)dataSession.Query(sql)
        .Set("@GUID", guid)
        .Execute()
        .FirstOrDefaultScalar();
    }

    #endregion GuidCommands

    #region Logging

    internal static SQLPerformanceLogger CreatePerformanceLogger(string operationType, string information, bool bypassPerformanceLogging)
    {
      if (bypassPerformanceLogging)
        return null;

      var logger = SQLPerformanceLogger.Create(Logger, operationType);
      if (logger != null)
        logger.Information = information;
      return logger;
    }

    #endregion Logging

    #endregion private
  }
}