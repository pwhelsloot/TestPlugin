using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AMCS.Data.Server.AppApi;
using AMCS.TestPlugin.Server.Services;
using log4net;

namespace AMCS.TestPlugin.Server.App
{
  public static class Program
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

    [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Suppressed for the template project only")]
    public static int Main(string[] args)
    {
      RunServer();
      return 0;
    }

    private static void RunServer()
    {
      var configuration = XDocument.Load("configuration.xml");

      var log4net = configuration.Document.Root.Element("log4net");

      using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(log4net.ToString())))
      {
        DataApp.SetupLog4Net(stream);
      }

      using (ServiceSetup.Setup(configuration))
      using (var app = DataApp.Start())
      {
        Log.Info("Startup complete; waiting for exit");

        app.WaitForShutdown();
      }
    }
  }
}
