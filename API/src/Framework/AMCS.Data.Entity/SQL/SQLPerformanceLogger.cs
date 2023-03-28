using System;
using System.Diagnostics;
using log4net;

namespace AMCS.Data.Entity.SQL
{
  internal class SQLPerformanceLogger : IDisposable
  {
    private static bool IsEnabled;

    internal static void SetEnabled(bool enabled)
    {
      IsEnabled = enabled;
    }

    public static SQLPerformanceLogger Create(ILog logger, string operationType)
    {
      if (!IsEnabled)
        return null;

      return new SQLPerformanceLogger(logger, operationType);
    }

    private readonly Stopwatch stopwatch = Stopwatch.StartNew();
    private readonly ILog logger;
    private readonly string operationType;
    private bool disposed;

    public string Information { get; set; }

    public string AdditionalInformation { get; set; }

    private SQLPerformanceLogger(ILog logger, string operationType)
    {
      this.logger = logger;
      this.operationType = operationType;
    }

    public void Dispose()
    {
      if (!disposed)
      {
        string information = Information;
        if (information != null && information.Length > 200)
          information = information.Substring(0, 150);

        string log = stopwatch.ElapsedMilliseconds + "ms \t " + operationType + " " + information;

        if (AdditionalInformation != null)
          log += " " + AdditionalInformation;

        logger.Info(log);

        disposed = true;
      }
    }
  }
}
