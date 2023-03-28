using System;
using System.Collections.Generic;
using System.Text;
using log4net.Appender;
using log4net.Core;

namespace AMCS.Data.Support
{
  public class ConsoleColorAppender : ConsoleAppender
  {
    private readonly object syncRoot = new object();

    protected override void Append(LoggingEvent loggingEvent)
    {
      lock (syncRoot)
      {
        var color = Console.ForegroundColor;

        try
        {
          if (loggingEvent.Level.CompareTo(Level.Error) >= 0)
            Console.ForegroundColor = ConsoleColor.Red;
          else if (loggingEvent.Level.CompareTo(Level.Warn) >= 0)
            Console.ForegroundColor = ConsoleColor.Yellow;
          else if (loggingEvent.Level.CompareTo(Level.Info) >= 0)
            Console.ForegroundColor = ConsoleColor.White;
          else
            Console.ForegroundColor = ConsoleColor.Gray;

          base.Append(loggingEvent);
        }
        finally
        {
          Console.ForegroundColor = color;
        }
      }
    }
  }
}
