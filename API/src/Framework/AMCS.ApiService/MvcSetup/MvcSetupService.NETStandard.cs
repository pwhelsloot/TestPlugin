#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Abstractions.MvcSetup;
using AMCS.Data.Configuration;
using AMCS.Data.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AMCS.ApiService.MvcSetup
{
  internal class MvcSetupService : IMvcSetupService
  {
    private readonly OrderedCallbacks<IMvcBuilder> setupCallbacks = new OrderedCallbacks<IMvcBuilder>();
    private readonly OrderedCallbacks<MvcOptions> optionsCallbacks = new OrderedCallbacks<MvcOptions>();
    private readonly OrderedCallbacks<IRouteBuilder> routesCallbacks = new OrderedCallbacks<IRouteBuilder>();

    public MvcSetupService(IAppSetupService setupService)
    {
      setupService.RegisterConfigureServices(services =>
      {
        var builder = services.AddMvc(p =>
        {
          // TODO: This should be removed. See https://docs.microsoft.com/en-us/aspnet/core/migration/22-to-30?view=aspnetcore-2.2&tabs=visual-studio
          p.EnableEndpointRouting = false;

          optionsCallbacks.Raise(p);
        });

        setupCallbacks.Raise(builder);
      });

      setupService.RegisterConfigure(app =>
      {
        app.UseMvc(p =>
        {
          routesCallbacks.Raise(p);
        });
      });
    }

    public void RegisterSetup(Action<IMvcBuilder> callback, int order = 0)
    {
      setupCallbacks.Register(callback, order);
    }

    public void RegisterOptions(Action<MvcOptions> callback, int order = 0)
    {
      optionsCallbacks.Register(callback, order);
    }

    public void RegisterRoutes(Action<IRouteBuilder> callback, int order = 0)
    {
      routesCallbacks.Register(callback, order);
    }
  }
}

#endif
