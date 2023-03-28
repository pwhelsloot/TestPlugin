//-----------------------------------------------------------------------------
// <copyright file="SQLDataAccessAudit.cs" company="AMCS Group">
//   Copyright © 2010-12 AMCS Group. All rights reserved.
// </copyright>
//
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Server.SQL
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Diagnostics;
  using AMCS.Data.Entity.SQL;
  using Data.Util.Extension;
  using log4net;

  /// <summary>
  /// Type of change to a SQL record
  /// </summary>
  public enum SQLDataAuditChangeType
  {
    [StringValue("I")]
    Insert,

    [StringValue("U")]
    Update,

    [StringValue("D")]
    Delete
  }

  public static class SQLDataAccessAudit
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(SQLDataAccessAudit));

    /// <summary>
    /// Inserts the audit record.
    /// </summary>
    /// <param name="sysUserId">The sys user id.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="keyName">Name of the key.</param>
    /// <param name="keyId">The key id.</param>
    /// <param name="changeType">The type of change recorded against the record (eg. Insert, Update or Delete)</param>
    /// <param name="dataSession">The data session.</param>
    public static void InsertAuditRecord(ISessionToken sysUserId, string tableName, string keyName, int keyId, SQLDataAuditChangeType changeType, IDataSession dataSession)
    {
      if (!((SQLDataSession)dataSession).Configuration.IsAuditTableEnable)
        return;

      using (CreatePerformanceLogger("InsertAuditRecord", tableName))
      {
        dataSession.StoredProcedure("audit.spI_AuditRecord")
          .Set("@SysUserId", sysUserId.UserId)
          .Set("@TableName", tableName)
          .Set("@KeyName", keyName)
          .Set("@KeyId", keyId)
          .SetObject("@ChangeType", GetChangeTypeChar(changeType), SqlDbType.Char)
          .ExecuteNonQuery();
      }
    }

    /// <summary>
    /// Inserts audit record based on a list of Id values.
    /// </summary>
    /// <param name="userId">The sys user id.</param>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="keyName">Name of the key.</param>
    /// <param name="keyId">The key id.</param>
    /// <param name="changeType">The type of change recorded against the record (eg. Insert, Update or Delete)</param>
    /// <param name="dataSession">The data session.</param>
    public static void InsertAuditRecordBatchByGuid(ISessionToken userId, string tableName, string keyName, List<Guid> guids, SQLDataAuditChangeType changeType, IDataSession dataSession)
    {
      if (!((SQLDataSession)dataSession).Configuration.IsAuditTableEnable)
        return;

      using (CreatePerformanceLogger("InsertAuditRecordBatchByGuid", tableName))
      {
        dataSession.StoredProcedure("audit.spI_AuditRecordBatchByGuid")
          .Set("@SysUserId", userId.UserId)
          .Set("@TableName", tableName)
          .Set("@KeyName", keyName)
          .SetIdList("@GUIDS", "Id", guids)
          .SetObject("@ChangeType", GetChangeTypeChar(changeType), SqlDbType.Char)
          .ExecuteNonQuery();
      }
    }

    private static char GetChangeTypeChar(SQLDataAuditChangeType changeType)
    {
      char c;

      switch (changeType)
      {
        case SQLDataAuditChangeType.Insert:
          c = 'I';
          break;
        case SQLDataAuditChangeType.Update:
          c = 'U';
          break;
        case SQLDataAuditChangeType.Delete:
          c = 'D';
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(changeType), changeType, null);
      }

      Debug.Assert(c == changeType.GetStringValue()[0], "SQLDataAuditChangeType is not mapped correctly");

      return c;
    }

    private static SQLPerformanceLogger CreatePerformanceLogger(string operationType, string information)
    {
      var logger = SQLPerformanceLogger.Create(Logger, operationType);
      if (logger != null)
        logger.Information = information;
      return logger;
    }
  }
}