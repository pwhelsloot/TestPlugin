//-----------------------------------------------------------------------------
// <copyright file="IDataSession.cs" company="AMCS Group">
//   Copyright © 2010-12 AMCS Group. All rights reserved.
// </copyright>
//
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

using AMCS.Data.Server.BslTriggers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server
{
  /// <summary>
  /// Specification of the data session interface
  /// </summary>
  public interface IDataSession : IDisposable
  {
    IDataSessionEvents Events { get; }

    IDictionary<string, object> Context { get; }

    /// <summary>
    /// Open connection
    /// </summary>
    /// <param name="connectionString">the connection string to use</param>
    /// <param name="isDetached">(Optional) is the connection detached</param>
    void OpenConnection(string connectionString, bool? isDetached = false);

    /// <summary>
    /// Checks if a transaction is active
    /// </summary>
    /// <returns></returns>
    bool IsTransaction();

    /// <summary>
    /// Attempts to get transaction ID
    /// </summary>
    /// <returns></returns>
    Guid GetTransactionId();

    /// <summary>
    /// Close the connection
    /// </summary>
    void CloseConnection();

    /// <summary>
    /// Starts a new transaction
    /// </summary>
    void StartTransaction();

    /// <summary>
    /// Starts a new transaction
    /// </summary>
    void StartTransaction(IsolationLevel level);

    /// <summary>
    /// Commits a transaction
    /// </summary>
    void CommitTransaction();

    /// <summary>
    /// Rollsback a transaction
    /// </summary>
    void RollbackTransaction();

    DbConnection GetConnection();

    DbTransaction GetTransaction();

    void BeforeCommit(Action<IDataSession> callback);

    void AfterCommit(Action<IDataSession> callback);

    void AfterDispose(Action<IDataSession> callback);
  }
}