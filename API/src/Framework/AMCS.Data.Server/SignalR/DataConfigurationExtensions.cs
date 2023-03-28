#if !NETFRAMEWORK

using AMCS.Data.Configuration;
using AMCS.Data.Server.Services;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SignalR
{
  public static class DataConfigurationExtensions
  {
    /// <summary>
    /// Activates and configures SignalR for this application.
    /// </summary>
    /// <param name="self">The data configuration to add the configuration to</param>
    /// <param name="azureConnectionString">An optional Azure SignalR connection string (if Azure SignalR should be used)</param>
    /// <param name="hubs">A list of SignalR hub classes to configure</param>
    public static void ConfigureSignalR(this DataConfiguration self, IConnectionString azureConnectionString, IList<IHubRegistration> hubs)
    {
      self.ContainerBuilder
        .Register(p => new SignalRConfiguration(p.Resolve<IAppSetupService>(), azureConnectionString, hubs))
        .SingleInstance()
        .AutoActivate()
        .AsSelf();

      foreach (IHubRegistration hub in hubs)
      {
        hub.ConfigureHubContext(self);
      }
    }
  }
}

#endif
