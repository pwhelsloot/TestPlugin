using AMCS.Data.Server;
using AMCS.Data.Server.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AMCS.Data.EndToEnd.Tests.DataMetrics
{
  public class TestDataMetricsEvents : IDataMetricsEvents
  {
    private readonly ITestDataMetricsEventsService sampleDataMetricsEventsService;

    public TestDataMetricsEvents(ITestDataMetricsEventsService sampleDataMetricsEventsService)
    {
      this.sampleDataMetricsEventsService = sampleDataMetricsEventsService;
    }

    public void TransactionCommit(SqlConnection connection)
    {
      sampleDataMetricsEventsService.IncrementTransactionCommits(connection);
    }

    public void TransactionRollback(SqlConnection connection)
    {
      sampleDataMetricsEventsService.IncrementTransactionRollbacks(connection);
    }

    public void TransactionStart(SqlConnection connection)
    {
      sampleDataMetricsEventsService.IncrementTransactionStarts(connection);
    }

    public void ConnectionClose(SqlConnection connection)
    {
      sampleDataMetricsEventsService.IncrementClosedConnections(connection);
    }

    public void ConnectionOpen(SqlConnection connection, bool? isDetached = false)
    {
      if (isDetached == true)
      {
        sampleDataMetricsEventsService.IncrementOpenDetachedConnections(connection);
      }
      else
      {
        sampleDataMetricsEventsService.IncrementOpenConnections(connection);
      }
    }

    public void InsertBegin(SqlConnection connection)
    {
      sampleDataMetricsEventsService.BeginEvent(TestDataMetricEventType.Insert, connection);
    }

    public void InsertEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null)
    {
      sampleDataMetricsEventsService.EndEvent(connection, command, rows);
    }

    public void UpdateBegin(SqlConnection connection)
    {
      sampleDataMetricsEventsService.BeginEvent(TestDataMetricEventType.Update, connection);
    }

    public void UpdateEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null)
    {
      sampleDataMetricsEventsService.EndEvent(connection, command, rows);
    }

    public void DeleteBegin(SqlConnection connection)
    {
      sampleDataMetricsEventsService.BeginEvent(TestDataMetricEventType.Delete, connection);
    }

    public void DeleteEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null)
    {
      sampleDataMetricsEventsService.EndEvent(connection, command, rows);
    }

    public void SoftDeleteBegin(SqlConnection connection)
    {
      sampleDataMetricsEventsService.BeginEvent(TestDataMetricEventType.SoftDelete, connection);
    }

    public void SoftDeleteEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null)
    {
      sampleDataMetricsEventsService.EndEvent(connection, command, rows);
    }

    public void DynamicNonQueryBegin(SqlConnection connection)
    {
      sampleDataMetricsEventsService.BeginEvent(TestDataMetricEventType.DynamicNonQuery, connection);
    }

    public void DynamicNonQueryEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null)
    {
      sampleDataMetricsEventsService.EndEvent(connection, command, rows);
    }

    public void ExecuteReaderBegin(SqlConnection connection)
    {
      sampleDataMetricsEventsService.BeginEvent(TestDataMetricEventType.Reader, connection);
    }

    public void ExecuteReaderEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null)
    {
      sampleDataMetricsEventsService.EndEvent(connection, command, rows);
    }

    public void StoredProcedureNonQueryBegin(SqlConnection connection)
    {
      sampleDataMetricsEventsService.BeginEvent(TestDataMetricEventType.StoredProcedureNonQuery, connection);
    }

    public void StoredProcedureNonQueryEnd(SqlConnection connection, SqlCommand command, int rows, Exception ex = null)
    {
      sampleDataMetricsEventsService.EndEvent(connection, command, rows);
    }
  }
}
