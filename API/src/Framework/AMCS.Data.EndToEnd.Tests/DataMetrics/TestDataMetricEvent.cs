using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EndToEnd.Tests.DataMetrics
{
  public class TestDataMetricEvent
  {
    public TestDataMetricEventType Type { get; }

    public string CommandText { get; internal set; }

    public List<string> StackTrace { get; } = new List<string>();

    public CommandType? CommandType { get; internal set; }

    public int? Rows { get; internal set; }

    public Stopwatch Stopwatch { get; }

    public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

    public SqlConnection SqlConnection { get; internal set; }

    public TestDataMetricEvent(TestDataMetricEventType dataMetricEventType, SqlConnection sqlConnection, bool isTimed = true)
    {
      Type = dataMetricEventType;
      SqlConnection = sqlConnection;
      if (isTimed)
      {
        Stopwatch = Stopwatch.StartNew();
      }
    }
  }
}
