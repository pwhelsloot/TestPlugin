using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  internal abstract class SQLExecutable<T> : ISQLExecutable<T>
    where T : ISQLExecutable<T>
  {
    protected bool IsUseExtendedTimeout { get; private set; }

    protected bool IsBypassPerformanceLogging { get; private set; }

    private T Self()
    {
      return (T)(ISQLExecutable<T>)this;
    }

    public T UseExtendedTimeout()
    {
      return UseExtendedTimeout(true);
    }

    public T UseExtendedTimeout(bool value)
    {
      IsUseExtendedTimeout = value;
      return Self();
    }

    public T BypassPerformanceLogging()
    {
      return BypassPerformanceLogging(true);
    }

    public T BypassPerformanceLogging(bool value)
    {
      IsBypassPerformanceLogging = value;
      return Self();
    }
  }
}
