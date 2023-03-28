using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using Autofac;

namespace AMCS.Data.Server.Services
{
  public class DataMetricsEventsBuilderService : IDataMetricsEventsBuilderService
  {
    private readonly List<IDataMetricsEvents> events = new List<IDataMetricsEvents>();
    private bool isBuilt;

    public void Add(IDataMetricsEvents events)
    {
      if (!isBuilt)
      {
        this.events.Add(events);
      }
    }

    public IDataMetricsEvents Build()
    {
      isBuilt = true;
      return new DataMetricsCollection(events.ToArray());
    }

    private class DataMetricsCollection : IDataMetricsEvents
    {
      private readonly IDataMetricsEvents[] events;

      public DataMetricsCollection(IDataMetricsEvents[] events)
      {
        this.events = events;
      }

      public void ConnectionOpen(SqlConnection connection, bool? isDetached = false)
      {
        foreach (var e in events)
        {
          e.ConnectionOpen(connection, isDetached);
        }
      }

      public void ConnectionClose(SqlConnection connection)
      {
        foreach (var e in events)
        {
          e.ConnectionClose(connection);
        }
      }

      public void TransactionStart(SqlConnection connection)
      {
        foreach (var e in events)
        {
          e.TransactionStart(connection);
        }
      }

      public void TransactionCommit(SqlConnection connection)
      {
        foreach (var e in events)
        {
          e.TransactionCommit(connection);
        }
      }

      public void TransactionRollback(SqlConnection connection)
      {
        foreach (var e in events)
        {
          e.TransactionRollback(connection);
        }
      }

      public void DynamicNonQueryBegin(SqlConnection connection)
      {
        foreach (var e in events)
        {
          e.DynamicNonQueryBegin(connection);
        }
      }

      public void DynamicNonQueryEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null)
      {
        foreach (var e in events)
        {
          e.DynamicNonQueryEnd(connection, command, rows, ex);
        }
      }

      public void DeleteBegin(SqlConnection connection)
      {
        foreach (var e in events)
        {
          e.DeleteBegin(connection);
        }
      }

      public void DeleteEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null)
      {
        foreach (var e in events)
        {
          e.DeleteEnd(connection, command, rows, ex);
        }
      }

      public void SoftDeleteBegin(SqlConnection connection)
      {
        foreach (var e in events)
        {
          e.SoftDeleteBegin(connection);
        }
      }

      public void SoftDeleteEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null)
      {
        foreach (var e in events)
        {
          e.SoftDeleteEnd(connection, command, rows, ex);
        }
      }

      public void ExecuteReaderBegin(SqlConnection connection)
      {
        foreach (var e in events)
        {
          e.ExecuteReaderBegin(connection);
        }
      }

      public void ExecuteReaderEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null)
      {
        foreach (var e in events)
        {
          e.ExecuteReaderEnd(connection, command, rows, ex);
        }
      }

      public void InsertBegin(SqlConnection connection)
      {
        foreach (var e in events)
        {
          e.InsertBegin(connection);
        }
      }

      public void InsertEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null)
      {
        foreach (var e in events)
        {
          e.InsertEnd(connection, command, rows, ex);
        }
      }

      public void StoredProcedureNonQueryBegin(SqlConnection connection)
      {
        foreach (var e in events)
        {
          e.StoredProcedureNonQueryBegin(connection);
        }
      }

      public void StoredProcedureNonQueryEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null)
      {
        foreach (var e in events)
        {
          e.StoredProcedureNonQueryEnd(connection, command, rows, ex);
        }
      }

      public void UpdateBegin(SqlConnection connection)
      {
        foreach (var e in events)
        {
          e.UpdateBegin(connection);
        }
      }

      public void UpdateEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null)
      {
        foreach (var e in events)
        {
          e.UpdateEnd(connection, command, rows, ex);
        }
      }
    }
  }
}
