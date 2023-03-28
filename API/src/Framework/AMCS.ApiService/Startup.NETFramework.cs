#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Configuration;
using AMCS.Data;
using AMCS.Data.Server.Services;
using Owin;

namespace AMCS.ApiService
{
  public class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      if (DataServices.TryResolve<IAppSetupService>(out var service))
        ((AppSetupService)service).RaiseSetup(app);
    }
  }
}

#endif
