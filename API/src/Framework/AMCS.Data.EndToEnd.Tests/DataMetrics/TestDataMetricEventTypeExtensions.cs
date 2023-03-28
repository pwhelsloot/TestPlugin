using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.EndToEnd.Tests.DataMetrics
{
  public static class TestDataMetricEventTypeExtensions
  {
    public static bool IsQuery(this TestDataMetricEventType self)
    {
      return (int)self < 10;
    }

    public static bool IsUpdate(this TestDataMetricEventType self)
    {
      return (int)self >= 10;
    }
  }
}
