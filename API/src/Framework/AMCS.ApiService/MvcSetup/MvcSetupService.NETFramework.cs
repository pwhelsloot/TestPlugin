#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using AMCS.ApiService.Abstractions.MvcSetup;
using AMCS.ApiService.Support;
using AMCS.Data.Configuration;

namespace AMCS.ApiService.MvcSetup
{
  internal class MvcSetupService : IMvcSetupService, IDelayedStartup
  {
    private readonly OrderedCallbacks<IMvcSetup> callbacks = new OrderedCallbacks<IMvcSetup>();

    public void Register(Action<IMvcSetup> callback, int order = 0)
    {
      callbacks.Register(callback, order);
    }

    public void Start()
    {
      AreaRegistration.RegisterAllAreas();
      RouteTable.Routes.MapMvcAttributeRoutes();

      callbacks.Raise(new MvcSetup(RouteTable.Routes, GlobalFilters.Filters));
    }

    private class MvcSetup : IMvcSetup
    {
      public RouteCollection Routes { get; }

      public GlobalFilterCollection Filters { get; }

      public MvcSetup(RouteCollection routes, GlobalFilterCollection filters)
      {
        Routes = routes;
        Filters = filters;
      }
    }
  }
}

#endif
