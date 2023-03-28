using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  public interface ISQLExecutable<out T>
    where T : ISQLExecutable<T>
  {
    T UseExtendedTimeout();

    T UseExtendedTimeout(bool value);

    T BypassPerformanceLogging();

    T BypassPerformanceLogging(bool value);
  }
}
