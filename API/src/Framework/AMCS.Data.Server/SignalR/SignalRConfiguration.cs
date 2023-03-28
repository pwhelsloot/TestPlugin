#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Configuration;
using AMCS.Data.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AMCS.Data.Server.SignalR
{
  internal class SignalRConfiguration
  {
    internal SignalRConfiguration(IAppSetupService appSetupService, IConnectionString azureConnectionString, IList<IHubRegistration> hubs)
    {
      appSetupService.RegisterConfigureServices(services =>
      {
        var signalRServerBuilder = services.AddSignalR((hubOptions) => { hubOptions.ClientTimeoutInterval = TimeSpan.FromMinutes(2); });
        if (azureConnectionString != null)
        {
          signalRServerBuilder.AddAzureSignalR(azureConnectionString.GetConnectionString());
        }
      });

      appSetupService.RegisterConfigure(app =>
      {
        app.UseRouting();
        app.UseFileServer();
        app.UseEndpoints(endpoints =>
        {
          foreach (IHubRegistration hub in hubs)
          {
            hub.MapHub(endpoints);
          }
        });
      });
    }
  }
}

#endif
