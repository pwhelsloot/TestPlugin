#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Configuration;
using AMCS.Data;
using AMCS.Data.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AMCS.ApiService
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services
      .AddCors(options =>
      {
        options.AddDefaultPolicy(policyBuilder =>
        {
          policyBuilder
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
      });

      if (DataServices.TryResolve<IAppSetupService>(out var service))
        ((AppSetupService)service).RaiseConfigureServices(services);
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseCors();

      if (DataServices.TryResolve<IAppSetupService>(out var service))
        ((AppSetupService)service).RaiseConfigure(app);
    }
  }
}

#endif
