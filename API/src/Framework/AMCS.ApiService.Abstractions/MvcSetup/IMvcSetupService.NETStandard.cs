#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AMCS.ApiService.Abstractions.MvcSetup
{
  public interface IMvcSetupService
  {
    void RegisterOptions(Action<MvcOptions> callback, int order = 0);

    void RegisterRoutes(Action<IRouteBuilder> callback, int order = 0);

    void RegisterSetup(Action<IMvcBuilder> callback, int order = 0);
  }
}

#endif
