#if !NETFRAMEWORK

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  public class ServiceProviderProxy<T>
  {
    private IServiceProvider serviceProvider;

    public ServiceProviderProxy(IAppSetupService appSetupService)
    {
      appSetupService.RegisterConfigure(app =>
      {
        serviceProvider = app.ApplicationServices;
      });
    }

    public T Resolve()
    {
      return serviceProvider.GetRequiredService<T>();
    }
  }
}

#endif