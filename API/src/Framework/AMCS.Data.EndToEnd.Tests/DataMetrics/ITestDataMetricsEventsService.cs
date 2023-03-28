using AMCS.Data.Server.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EndToEnd.Tests.DataMetrics
{
  public interface ITestDataMetricsEventsService
  {
    int OpenConnections { get; }

    void IncrementOpenConnections(SqlConnection connection);

    int OpenDetachedConnections { get; }

    void IncrementOpenDetachedConnections(SqlConnection connection);

    int ClosedConnections { get; }

    void IncrementClosedConnections(SqlConnection connection);

    int TransactionCommits { get; }

    void IncrementTransactionCommits(SqlConnection connection);

    int TransactionRollbacks { get; }

    void IncrementTransactionRollbacks(SqlConnection connection);

    int TransactionStarts { get; }

    void IncrementTransactionStarts(SqlConnection connection);

    int DynamicNonQueries { get; }

    int StoredProcedureNonQueries { get; }

    int Readers { get; }

    int ReaderRows { get; }
    
    int Inserts { get; }
    
    int Updates { get; }

    int Deletes { get; }

    int SoftDeletes  { get; }

    void BeginEvent(TestDataMetricEventType dataMetricEventType, SqlConnection connection);

    void EndEvent(SqlConnection connection, SqlCommand command, int rows);

    List<MetricsLimitExceededException> Errors { get; }

    List<TestDataMetricEvent> GetDataMetricEvents();

    void Reset(TestDataMetricsLimits limits = null);
  }
}
