using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EndToEnd.Tests.DataMetrics
{
  public class TestDataMetricsLimits
  {
    public int? OpenConnections { get; set; }

    public int? OpenDetachedConnections { get; set; }

    public int? TransactionCommits { get; set; }

    public int? TransactionRollbacks { get; set; }

    public int? TransactionStarts { get; set; }

    public int? Queries { get; set; }

    public int? ReaderRows { get; set; }

    public int? Inserts { get; set; }

    public int? Updates { get; set; }

    public int? Deletes { get; set; }
  }
}
