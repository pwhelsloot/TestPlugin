using System;
using AMCS.CommsServer.Client;
using log4net;

namespace AMCS.ApiService.CommsServer
{
  partial class CommsServerWebSocketProtocol
  {
    public class Log4NetLogger : ILogger
    {
      private static readonly ILog Log = LogManager.GetLogger(typeof(Log4NetLogger));

      public void Debug(string message, Exception exception = null)
      {
        Log.Debug(message, exception);
      }

      public void Info(string message, Exception exception = null)
      {
        Log.Info(message, exception);
      }

      public void Warn(string message, Exception exception = null)
      {
        Log.Warn(message, exception);
      }

      public void Error(string message, Exception exception = null)
      {
        Log.Error(message, exception);
      }
    }
  }
}
