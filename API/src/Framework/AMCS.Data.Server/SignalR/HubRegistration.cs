#if !NETFRAMEWORK

using AMCS.Data.Configuration;
using AMCS.Data.Server.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SignalR
{
  public class HubRegistration<THub> : IHubRegistration
    where THub : Hub
  {
    private readonly string endPointPath;

    public HubRegistration(string endPointPath)
    {
      this.endPointPath = endPointPath;
    }

    public void MapHub(IEndpointRouteBuilder endPointRouteBuilder)
    {
      endPointRouteBuilder.MapHub<THub>(endPointPath);
    }

    public void ConfigureHubContext(DataConfiguration configuration)
    {
      configuration.ConfigureServiceProviderProxy<IHubContext<THub>>();
    }
  }
}

#endif
