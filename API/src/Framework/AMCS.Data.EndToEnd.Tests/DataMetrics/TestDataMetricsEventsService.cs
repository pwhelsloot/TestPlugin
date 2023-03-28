using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.Services;

namespace AMCS.Data.EndToEnd.Tests.DataMetrics
{
  public class TestDataMetricsEventsService : ITestDataMetricsEventsService
  {
    private const string LastStackTraceShown = "   at AMCS.Elemos.Server.EndToEndTests";

    private readonly Dictionary<SqlConnection, List<TestDataMetricEvent>> dataMetricEvents = new Dictionary<SqlConnection, List<TestDataMetricEvent>>();
    private TestDataMetricsLimits limits;

    public int OpenConnections { get; private set; }

    public int OpenDetachedConnections { get; private set; }

    public int ClosedConnections { get; private set; }

    public int TransactionCommits { get; private set; }

    public int TransactionRollbacks { get; private set; }

    public int TransactionStarts { get; private set; }

    public int DynamicNonQueries { get; private set; }

    public int StoredProcedureNonQueries { get; private set; }

    public int Readers { get; private set; }

    public int ReaderRows { get; private set; }

    public int Inserts { get; private set; }

    public int Updates { get; private set; }

    public int Deletes { get; private set; }

    public int SoftDeletes { get; private set; }

    public List<MetricsLimitExceededException> Errors { get; } = new List<MetricsLimitExceededException>();

    public void BeginEvent(TestDataMetricEventType dataMetricEventType, SqlConnection connection)
    {
      switch (dataMetricEventType)
      {
        case TestDataMetricEventType.DynamicNonQuery:
          DynamicNonQueries++;
          ValidateMetricLimitsExceeded(StoredProcedureNonQueries + DynamicNonQueries, limits?.Queries, "Queries");
          break;
        case TestDataMetricEventType.StoredProcedureNonQuery:
          StoredProcedureNonQueries++;
          ValidateMetricLimitsExceeded(StoredProcedureNonQueries + DynamicNonQueries, limits?.Queries, "Queries");
          break;
        case TestDataMetricEventType.Reader:
          Readers++;
          break;
        case TestDataMetricEventType.Insert:
          Inserts++;
          ValidateMetricLimitsExceeded(Inserts, limits?.Inserts, "Inserts");
          break;
        case TestDataMetricEventType.Update:
          Updates++;
          ValidateMetricLimitsExceeded(Updates, limits?.Updates, "Updates");
          break;
        case TestDataMetricEventType.Delete:
          Deletes++;
          ValidateMetricLimitsExceeded(Deletes + SoftDeletes, limits?.Deletes, "Deletes");
          break;
        case TestDataMetricEventType.SoftDelete:
          SoftDeletes++;
          ValidateMetricLimitsExceeded(Deletes + SoftDeletes, limits?.Deletes, "Deletes");
          break;
      }


      if (!dataMetricEvents.ContainsKey(connection))
      {
        dataMetricEvents.Add(connection, new List<TestDataMetricEvent>());
      }

      dataMetricEvents[connection].Add(new TestDataMetricEvent(dataMetricEventType, connection));
    }

    public void EndEvent(SqlConnection connection, SqlCommand command, int rows)
    {
      var dataMetricEvent = dataMetricEvents[connection].Last();

      dataMetricEvent.Stopwatch.Stop();
      dataMetricEvent.CommandText = command?.CommandText;
      dataMetricEvent.CommandType = command?.CommandType;
      dataMetricEvent.Rows = rows;
      dataMetricEvent.StackTrace.AddRange(GetStack(7));

      if (dataMetricEvent.Type == TestDataMetricEventType.Reader)
      {
        ReaderRows += rows;
        ValidateMetricLimitsExceeded(ReaderRows, limits?.ReaderRows, "ReaderRows");
      }

      if (command != null)
      {
        foreach (SqlParameter parameter in command.Parameters)
        {
          object value = parameter.Value;

          if (value is DBNull)
            value = null;

          dataMetricEvent.Parameters.Add(parameter.ParameterName, value);
        }
      }
      AddSingleDataMetricEvent(dataMetricEvent.Type, connection);
    }

    public List<TestDataMetricEvent> GetDataMetricEvents()
    {
      return dataMetricEvents.SelectMany(d => d.Value).ToList();
    }

    public void Reset(TestDataMetricsLimits limits = null)
    {
      this.limits = limits;
      OpenConnections = 0;
      OpenDetachedConnections = 0;
      ClosedConnections = 0;
      TransactionCommits = 0;
      TransactionRollbacks = 0;
      TransactionStarts = 0;
      DynamicNonQueries = 0;
      StoredProcedureNonQueries = 0;
      Readers = 0;
      ReaderRows = 0;
      Inserts = 0;
      Updates = 0;
      Deletes = 0;
      SoftDeletes = 0;
      Errors.Clear();
      dataMetricEvents.Clear();
    }

    private static List<string> GetStack(int removeLines)
    {
      string[] stack = Environment.StackTrace.Split(
          new string[] { Environment.NewLine },
          StringSplitOptions.RemoveEmptyEntries);

      if (stack.Length <= removeLines)
        return null;

      bool isLastStackTraceShown = false;
      List<string> actualResult = new List<string>();
      for (int i = removeLines; i < stack.Length; i++)
      {
        if (stack[i].StartsWith(LastStackTraceShown))
        {
          isLastStackTraceShown = true;
        }
        else if (isLastStackTraceShown)
        {
          break;
        }

        actualResult.Add(stack[i]);
      }

      return actualResult;
    }

    public void IncrementOpenConnections(SqlConnection connection)
    {
      OpenConnections++;
      AddSingleDataMetricEvent(TestDataMetricEventType.OpenConnection, connection);
      ValidateMetricLimitsExceeded(OpenConnections, limits?.OpenConnections, "OpenConnections");
    }

    public void IncrementOpenDetachedConnections(SqlConnection connection)
    {
      OpenDetachedConnections++;
      AddSingleDataMetricEvent(TestDataMetricEventType.OpenDetachedConnection, connection);
      ValidateMetricLimitsExceeded(OpenDetachedConnections, limits?.OpenDetachedConnections, "OpenDetachedConnections");
    }

    public void IncrementClosedConnections(SqlConnection connection)
    {
      ClosedConnections++;
      AddSingleDataMetricEvent(TestDataMetricEventType.ClosedConnection, connection);
    }

    public void IncrementTransactionCommits(SqlConnection connection)
    {
      TransactionCommits++;
      AddSingleDataMetricEvent(TestDataMetricEventType.TransactionCommit, connection);
      ValidateMetricLimitsExceeded(TransactionCommits, limits?.TransactionCommits, "TransactionCommits");
    }

    public void IncrementTransactionRollbacks(SqlConnection connection)
    {
      TransactionRollbacks++;
      AddSingleDataMetricEvent(TestDataMetricEventType.TransactionRollback, connection);
      ValidateMetricLimitsExceeded(TransactionRollbacks, limits?.TransactionRollbacks, "TransactionRollbacks");
    }

    public void IncrementTransactionStarts(SqlConnection connection)
    {
      TransactionStarts++;
      AddSingleDataMetricEvent(TestDataMetricEventType.TransactionStart, connection);
      ValidateMetricLimitsExceeded(TransactionStarts, limits?.TransactionStarts, "TransactionStarts");
    }

    private void AddSingleDataMetricEvent(TestDataMetricEventType dataMetricEventType, SqlConnection connection)
    {
      var dataMetricEvent = new TestDataMetricEvent(dataMetricEventType, connection, false);
      dataMetricEvent.StackTrace.AddRange(GetStack(7));

      if (!dataMetricEvents.ContainsKey(connection))
      {
        dataMetricEvents.Add(connection, new List<TestDataMetricEvent>());
      }

      dataMetricEvents[connection].Add(dataMetricEvent);
    }

    private void ValidateMetricLimitsExceeded(int value, int? limit, string field)
    {
      if (value > limit)
      {
        var errorMessage = $"{field} has exceeded limit of {limit}";
        if (Errors.Count(e => e.Message == errorMessage) == 0)
        {
          Errors.Add(new MetricsLimitExceededException(errorMessage));
        }
      }
    }
  }
}
