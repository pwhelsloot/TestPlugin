//-----------------------------------------------------------------------------
// <copyright file="SQLDataAccessHistory.cs" company="AMCS Group">
//   Copyright © 2010-12 AMCS Group. All rights reserved.
// </copyright>
//
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AMCS.Data.Entity.History;
using AMCS.Data.Entity.SQL;
using log4net;
using Newtonsoft.Json;

namespace AMCS.Data.Server.SQL
{
  public static class SQLDataAccessHistory
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(SQLDataAccessHistory));

    /// <summary>
    /// Inserts the history record
    /// </summary>
    /// <param name="userId">The sys user id</param>
    /// <param name="tableName">The table name</param>
    /// <param name="keyName">The key name</param>
    /// <param name="keyId">The key id</param>
    /// <param name="changes">The changes</param>
    /// <param name="date">The changed date</param>
    /// <param name="dataSession">The data session</param>
    public static void InsertHistoryRecord(ISessionToken userId, string tableName, string keyName, int keyId, string changes, EntityHistoryTypeEnum entityHistoryTypeId, string parentTable, int? parentTableId, DateTime date, IDataSession dataSession)
    {
      // RDM - Temp fix for bug 187964. Issue is some server calls are using SessionManager.SysUserSessionKey instead of the real string userId token.
      // This results in a sysUserId of 0 being found for the SysUserId which is incorrect.
      if (userId.UserId <= 0)
        return;

      using (CreatePerformanceLogger("InsertHistoryRecord", tableName))
      {
        var history = new EntityHistory()
        {
          Table = tableName,
          TableId = keyId,
          SysUserId = userId.UserId,
          Date = TimeZoneUtils.NeutralClock.GetCurrentZonedDateTime(),
          Changes = changes,
          EntityHistoryTypeId = (int)entityHistoryTypeId,
          ParentTable = parentTable,
          ParentTableId = parentTableId,
          CorrelationId = Activity.Current?.Id
        };
        
        dataSession.Save<EntityHistory>(userId, new[] { history });
      }
    }

    public static void DeserializeEntityHistory(EntityHistory history)
    {
      string formatVersion = history.Changes.Substring(0, history.Changes.IndexOf(':'));
      switch (formatVersion)
      {
        case "2":
          ConvertHistoryV2(history);
          break;

        default:
          ConvertHistoryV1(history);
          break;
      }
    }

    private static void ConvertHistoryV1(EntityHistory history)
    {
      history.TypedChanges = SQLEntityHistoryChange.Convert(Newtonsoft.Json.JsonConvert.DeserializeObject<List<SQLEntityHistoryChange>>(history.Changes));
    }

    private static void ConvertHistoryV2(EntityHistory history)
    {
      // Remove the version header
      string changes = history.Changes.Substring(2);

      history.TypedChanges = new List<EntityHistoryChange>();

      // For optimal performance read it manually
      using (var reader = new JsonTextReader(new StringReader(changes)))
      {
        while (reader.Read())
        {
          if (reader.TokenType != JsonToken.PropertyName)
            continue;

          var change = new EntityHistoryChange();
          change.PropertyKey = (string)reader.Value;

          reader.Read(); // Should be ArrayStart
          reader.Read(); // Should be OldValue
          change.OldValue = reader.Value;
          reader.Read(); // Should be OldValue
          change.NewValue = reader.Value;
          history.TypedChanges.Add(change);
        }
      }
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
