#if !NETFRAMEWORK

using AMCS.Data.Configuration;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SignalR
{
  public interface IHubRegistration
  {
    void MapHub(IEndpointRouteBuilder endPointRouteBuilder);
    void ConfigureHubContext(DataConfiguration configuration);
  }
}

#endif
