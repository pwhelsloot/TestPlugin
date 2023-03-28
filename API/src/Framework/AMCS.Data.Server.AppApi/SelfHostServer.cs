#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService;
using AMCS.Data.Server.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace AMCS.Data.Server.AppApi
{
  internal class SelfHostServer : IDisposable
  {
    private IWebHost server;
    private bool disposed;

    public SelfHostServer(string url)
    {
      var configLogLevel = DataServices.Resolve<IServerConfiguration>().ApplicationInsightsLoggingLevel;
      if (string.IsNullOrEmpty(configLogLevel) || !Enum.TryParse(configLogLevel, true, out LogLevel logLevel))
        logLevel = LogLevel.Warning;

      var builder = WebHost.CreateDefaultBuilder()
        .ConfigureLogging(p =>
        {
          p.ClearProviders();
          p.AddLog4Net(new Log4NetProviderOptions
          {
            ExternalConfigurationSetup = true
          });
          p.SetMinimumLevel(logLevel);
        })
        .UseStartup<Startup>();

      if (url == null)
      {
        server = builder
          .UseIIS()
          .Build();
      }
      else
      {
        server = builder
          .UseKestrel()
          .UseUrls(url)
          .Build();
      }

      server.Start();
    }

    public void WaitForShutdown()
    {
      server.WaitForShutdown();
    }

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

#endif
