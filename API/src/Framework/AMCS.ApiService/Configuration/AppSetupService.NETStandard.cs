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

namespace AMCS.ApiService.Configuration
{
  internal class AppSetupService : IAppSetupService
  {
    private readonly OrderedCallbacks<IApplicationBuilder> callbacks = new OrderedCallbacks<IApplicationBuilder>();
    private readonly OrderedCallbacks<IServiceCollection> serviceCallbacks = new OrderedCallbacks<IServiceCollection>();

    public void RegisterConfigure(Action<IApplicationBuilder> action, int order = 0)
    {
      callbacks.Register(action, order);
    }

    public void RegisterConfigureServices(Action<IServiceCollection> action, int order = 0)
    {
      serviceCallbacks.Register(action, order);
    }

    internal void RaiseConfigure(IApplicationBuilder app)
    {
      callbacks.Raise(app);
    }

    internal void RaiseConfigureServices(IServiceCollection services)
    {
      serviceCallbacks.Raise(services);
    }
  }
}

#endif
