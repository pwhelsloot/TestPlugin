#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.Services;
using Microsoft.AspNetCore.Builder;

namespace AMCS.ApiService.Configuration
{
  internal class WebSocketsService
  {
    public WebSocketsService(IAppSetupService setupService)
    {
      setupService.RegisterConfigure(
        app =>
        {
          app.UseWebSockets();
        },
        -1000);
    }
  }
}

#endif
