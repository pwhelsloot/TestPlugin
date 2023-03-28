#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace AMCS.ApiService.Abstractions.MvcSetup
{
  public interface IMvcSetup
  {
    RouteCollection Routes { get; }

    GlobalFilterCollection Filters { get; }
  }
}

#endif
