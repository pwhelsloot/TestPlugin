using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EndToEnd.Tests.DataMetrics
{
  public class MetricsLimitExceededException : Exception
  {
    public MetricsLimitExceededException()
    {
    }

    public MetricsLimitExceededException(string message)
      : base(message)
    {
    }

    public MetricsLimitExceededException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
