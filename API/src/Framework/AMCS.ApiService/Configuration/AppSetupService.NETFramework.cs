#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Configuration;
using AMCS.Data.Server.Services;
using Owin;

namespace AMCS.ApiService.Configuration
{
  internal class AppSetupService : IAppSetupService
  {
    private readonly OrderedCallbacks<IAppBuilder> callbacks = new OrderedCallbacks<IAppBuilder>();

    public void Register(Action<IAppBuilder> action, int order)
    {
      callbacks.Register(action, order);
    }

    internal void RaiseSetup(IAppBuilder app)
    {
      callbacks.Raise(app);
    }
  }
}

#endif
