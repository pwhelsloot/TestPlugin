#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AMCS.Data.Server.Services
{
  public interface IAppSetupService
  {
    void RegisterConfigure(Action<IApplicationBuilder> action, int order = 0);

    void RegisterConfigureServices(Action<IServiceCollection> action, int order = 0);
  }
}

#endif
