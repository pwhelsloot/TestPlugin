using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server
{
  public sealed class FakeDataSession : IDataSession
  {
    private bool isTransaction;
    private bool isDisposed;
    private List<Action<IDataSession>> beforeCommitActions;
    private List<Action<IDataSession>> afterCommitActions;
    private List<Action<IDataSession>> afterDisposeActions;

    public IDataSessionEvents Events => EmptyDataSessionEvents.Instance;

    public string ConnectionString => string.Empty;

    public Guid DataSessionId => Guid.Empty;

    public IDictionary<string, object> Context => new Dictionary<string, object>();

    public void CloseConnection()
    {
      CheckNotDisposed();
    }

    public void CommitTransaction()
    {
      CheckInTransaction();
      CheckNotDisposed();
      isTransaction = false;

      if (afterCommitActions == null)
        return;

      foreach (var action in afterCommitActions)
      {
        action(this);
      }

      afterCommitActions = null;
    }

    public void Dispose()
    {
      if (!isDisposed)
      {
        if (afterDisposeActions != null)
        {
          foreach (var action in afterDisposeActions)
          {
            action(this);
          }

          afterDisposeActions = null;
        }

        isDisposed = true;
      }
    }

    public DbConnection GetConnection()
    {
      return null;
    }

    public DbTransaction GetTransaction()
    {
      return null;
    }

    public bool IsTransaction()
    {
      return isTransaction;
    }

    public Guid GetTransactionId()
    {
      return Guid.Empty;
    }

    public void OpenConnection(string connectionString, bool? isDetached)
    {
      CheckNotDisposed();
    }

    public void RollbackTransaction()
    {
      CheckInTransaction();
      CheckNotDisposed();
      isTransaction = false;
      afterCommitActions = null;
    }

    public void StartTransaction()
    {
      CheckNotInTransaction();
      CheckNotDisposed();
      isTransaction = true;
    }

    public void StartTransaction(IsolationLevel level)
    {
      CheckNotInTransaction();
      CheckNotDisposed();
      isTransaction = true;
    }

    private void CheckNotInTransaction()
    {
      if (IsTransaction())
      {
        throw new InvalidOperationException("Already in a transaction, cannot start another");
      }
    }

    private void CheckInTransaction()
    {
      if (!IsTransaction())
      {
        throw new InvalidOperationException("No transaction to commit or rollback.");
      }
    }

    private void CheckNotDisposed()
    {
      if (this.isDisposed)
      {
        throw new ObjectDisposedException(this.GetType().Name);
      }
    }

    public void BeforeCommit(Action<IDataSession> callback)
    {
      CheckNotDisposed();

      if (!isTransaction)
        throw new Exception("Session must be in a transaction to add an BeforeCommit action");

      if (beforeCommitActions == null)
        beforeCommitActions = new List<Action<IDataSession>>();
      beforeCommitActions.Add(callback);
    }

    public void AfterCommit(Action<IDataSession> callback)
    {
      CheckNotDisposed();

      if (!isTransaction)
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

    private class EmptyDataSessionEvents : IDataSessionEvents
    {
      public static readonly EmptyDataSessionEvents Instance = new EmptyDataSessionEvents();

      private EmptyDataSessionEvents()
      {
      }

      public void AfterInsert(ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
      {
      }

      public void AfterUpdate(ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind)
      {
      }

      public void AfterDelete(ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
      {
      }

      public void BeforeInsert(ISessionToken userId, Type entityType, EntityObject entity)
      {
      }

      public void BeforeUpdate(ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid, DataUpdateKind kind)
      {
      }

      public void BeforeDelete(ISessionToken userId, Type entityType, EntityObject entity, int id, Guid? guid)
      {
      }
    }
  }
}
