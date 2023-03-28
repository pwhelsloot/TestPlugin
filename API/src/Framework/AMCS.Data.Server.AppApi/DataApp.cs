using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace AMCS.Data.Server.AppApi
{
  public class DataApp : IDisposable
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(DataApp));

    public static void SetupLog4Net(Stream stream)
    {
      System.Diagnostics.Activity.DefaultIdFormat = System.Diagnostics.ActivityIdFormat.Hierarchical;

      XmlConfigurator.Configure(LogManager.GetRepository(typeof(DataApp).Assembly), stream);
    }

    public static DataApp Start(string url = null)
    {
      Log.InfoFormat("Starting web server");

      return new DataApp(StartServer(url));
    }

    private static SelfHostServer StartServer(string url)
    {
      return new SelfHostServer(url);
    }

    private SelfHostServer server;
    private bool disposed;

    private DataApp(SelfHostServer server)
    {
      this.server = server;
    }

    /// <summary>
    /// Wait for the Comms Server to shut down.
    /// </summary>
    public void WaitForShutdown()
    {
      server.WaitForShutdown();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
      if (!disposed)
      {
        if (server != null)
        {
          server.Dispose();
          server = null;
        }

        disposed = true;
      }
    }
  }
}
