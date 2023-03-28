//-----------------------------------------------------------------------------
// <copyright file="SQLDataSession.cs" company="AMCS Group">
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
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Configuration;
using AMCS.Data.Entity;
using AMCS.Data.Server.Services;
using AMCS.Data.Support;
using log4net;
using Microsoft.Azure.Services.AppAuthentication;

namespace AMCS.Data.Server.SQL
{
  /// <summary>
  /// Class for managing a session and transactions
  /// </summary>
  public class SQLDataSession : IDataSession
  {
    #region Properties

    private static readonly ILog Logger = LogManager.GetLogger(typeof(SQLDataSession));

    private readonly Stopwatch performanceMonitorStopwatch;
    
    private List<Action<IDataSession>> afterCommitActions = new List<Action<IDataSession>>();
    private List<Action<IDataSession>> afterDisposeActions = new List<Action<IDataSession>>();

    private Guid currentTransactionId;

    public SqlConnection Connection { get; set; }

    public SqlTransaction Transaction { get; set; }

    public SQLDataSessionConfiguration Configuration { get; }
    
    public IDataSessionEvents Events { get; }

    public IDictionary<string, object> Context { get; }
    internal IDataMetricsEvents Metrics { get; }

    #endregion Properties

    #region Constructors Destructors

    /// <summary>
    /// Constructor - the connection string picked up from configuration
    /// </summary>
    internal SQLDataSession(IConnectionString connectionString, SQLDataSessionConfiguration configuration, IDataEvents events, IDataMetricsEvents metrics, bool? isDetached = false)
    {
      Metrics = metrics;
      Configuration = configuration;
      Context = new Dictionary<string, object>();
      Events = new DataSessionEvents(this, events);

      if (configuration.IsPerformanceMonitoringEnabled)
        performanceMonitorStopwatch = Stopwatch.StartNew();

      OpenConnection(connectionString.GetConnectionString(), isDetached);
    }

    /// <summary>
    /// Destructor - call dispose.
    /// </summary>
    ~SQLDataSession()
    {
      // The Dispose method must be called from the finalizer (destructor in C#) to ensure that
      this.Dispose(false);
    }

    #endregion Constructors Destructors

    #region IDisposable Implementation

    /// <summary>
    /// Track if disposed.
    /// </summary>
    protected bool Disposed { get; private set; }

    /// <summary>
    /// This method is the implementation of the IDisposable interface's Dispose operation.
    /// This method must be public to enable it to be called on an object reference
    /// (as well as via an IDisposable interface reference).
    /// </summary>
    public void Dispose()
    {
      // The Dispose method is called with true specified as the value of the disposing parameter
      // to indicate that the instance IS being "disposed", i.e.
      //   - the instance's public Dispose method has been called directly via an object reference
      //   - the instance's implementation of the IDisposable interface has been called
      //     (i.e. the instance's Dispose method has been called via an IDisposable interface reference,
      //     e.g from a using statement)
      this.Dispose(true);

      // The garbage collector's SuppressFinalize method is called to indicate to the garbage collector
      // that there is no need to call the finalizer for this instance (desctrutor in C#),
      // since the invocation of this Dispose method has performed the required cleanup to release
      // unmanaged resources.
      GC.SuppressFinalize(this);

      if (performanceMonitorStopwatch != null)
      {
        performanceMonitorStopwatch.Stop();
        Logger.InfoFormat("{0}ms \t SQLDataSession lifetime", performanceMonitorStopwatch.ElapsedMilliseconds);
      }
    }

    /// <summary>
    /// Release
    ///   - unmanaged resources
    ///   - managed resources that directly or indirectly hold unmanaged resources
    /// </summary>
    /// <param name="disposing">
    /// True if the instance is being disposed directly or indirectly;
    /// false if the instance is being finalized by the garbage collector.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
      // The implementation of Dispose should support being called multiple times without throwing an exception.
      // Only the first call to Dispose should perform any work.
      if (!this.Disposed)
      {
        if (disposing)
        {
          if (this.Transaction != null)
            this.Transaction.Dispose();
          this.Connection.Dispose();

          foreach (var action in afterDisposeActions)
          {
            action(this);
          }

          afterDisposeActions.Clear();
        }

        // Prevent further calls to dispose from performing any work.
        this.Disposed = true;

        Metrics?.ConnectionClose(Connection);
      }
    }

    protected void CheckNotDisposed()
    {
      if (this.Disposed)
      {
        throw new ObjectDisposedException(this.GetType().Name);
      }
    }

    #endregion IDisposable Implementation

    #region Public Methods

    /// <summary>
    /// Open connection
    /// </summary>
    /// <param name="connectionString">the connection string to use</param>
    public void OpenConnection(string connectionString, bool? isDetached = false)
    {
      CheckNotDisposed();

      if (Connection == null)
      {
        Connection = ConnectionStringUtils.OpenSqlConnection(connectionString);
      }

      if (Connection.State != ConnectionState.Open)
        Connection.Open();

      Metrics?.ConnectionOpen(Connection, isDetached);
    }

    /// <summary>
    /// Close the connection
    /// </summary>
    public void CloseConnection()
    {
      CheckNotDisposed();

      if (Connection != null && Connection.State != ConnectionState.Closed)
        Connection.Close();
    }

    public bool IsTransaction()
    {
      return Transaction != null;
    }

    public Guid GetTransactionId()
    {
      if (IsTransaction())
        return currentTransactionId;

      throw new InvalidOperationException("IDataSession is not currently in a transaction");
    }

    /// <summary>
    /// Starts a new transaction
    /// </summary>
    public void StartTransaction()
    {
      try
      {
        CheckNotDisposed();
        Transaction = Connection.BeginTransaction();
        Metrics?.TransactionStart(Connection);
      }
      finally
      {
        currentTransactionId = Guid.NewGuid();
      }
    }

    /// <summary>
    /// Starts a new transaction
    /// </summary>
    public void StartTransaction(IsolationLevel level)
    {
      try
      {
        CheckNotDisposed();
        Transaction = Connection.BeginTransaction(level);
        Metrics?.TransactionStart(Connection);
      }
      finally
      {
        currentTransactionId = Guid.NewGuid();
      }
    }

    /// <summary>
    /// Commits a transaction
    /// </summary>
    public void CommitTransaction()
    {
      CheckNotDisposed();

      Metrics?.TransactionCommit(Connection);
      
      try
      {
        Transaction.Commit();
        Transaction = null;
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("Commit failed in SQLDataSession: {0}", ex.Message), ex);
      }
      finally
      {
        currentTransactionId = Guid.Empty;
      }

      if (!afterCommitActions.Any())
        return;

      foreach (var action in afterCommitActions)
      {
        action(this);
      }

      afterCommitActions.Clear();
    }

    /// <summary>
    /// Rollsback a transaction
    /// </summary>
    public void RollbackTransaction()
    {
      CheckNotDisposed();

      Metrics?.TransactionRollback(Connection);

      try
      {
        Transaction.Rollback();
        Transaction = null;
      }
      catch
      {
        // tbd
      }

      // On rollback we just drop the after commit actions to ensure they're
      // not called on the next transaction.
      afterCommitActions.Clear();
    }

    public DbConnection GetConnection()
    {
      return Connection;
    }

    public DbTransaction GetTransaction()
    {
      return Transaction;
    }

    public void BeforeCommit(Action<IDataSession> callback)
    {
      CheckNotDisposed();

      if (Transaction == null)
        throw new Exception("Session must be in a transaction to add an BeforeCommit action");
      callback(this);
    }

    public void AfterCommit(Action<IDataSession> callback)
    {
      CheckNotDisposed();

      if (Transaction == null)
        throw new Exception("Session must be in a transaction to add an AfterCommit action");

      if (afterCommitActions == null)
        afterCommitActions = new List<Action<IDataSession>>();
      afterCommitActions.Add(callback);
    }

    public void AfterDispose(Action<IDataSession> callback)
    {
      CheckNotDisposed();

      if (afterDisposeActions == null)
        afterDisposeActions = new List<Action<IDataSession>>();
      afterDisposeActions.Add(callback);
    }

    #endregion Public Methods

    private class DataSessionEvents : IDataSessionEvents
    {
      private readonly SQLDataSession dataSession;
      private readonly IDataEvents events;

      public DataSessionEvents(SQLDataSession dataSession, IDataEvents events)
      {
        this.dataSession = dataSession;
        this.events = events;
      }

      public void AfterInsert(ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
      {
        events.AfterInsert(dataSession, userId, entityType, entity, id, guid);
      }

      public void AfterUpdate(ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind)
      {
        events.AfterUpdate(dataSession, userId, entityType, entity, id, guid, kind);
      }

      public void AfterDelete(ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
      {
        events.AfterDelete(dataSession, userId, entityType, entity, id, guid);
      }

      public void BeforeInsert(ISessionToken userId, Type entityType, EntityObject entity)
      {
        events.BeforeInsert(dataSession, userId, entityType, entity);
      }

      public void BeforeUpdate(ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind)
      {
        events.BeforeUpdate(dataSession, userId, entityType, entity, id, guid, kind);
      }

      public void BeforeDelete(ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
      {
        events.BeforeDelete(dataSession, userId, entityType, entity, id, guid);
      }
    }
  }
}