#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AMCS.ApiService.Support
{
  public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      CorsConfiguration.Configure(new HeaderAccess(filterContext));

      base.OnActionExecuting(filterContext);
    }

    private class HeaderAccess : CorsConfiguration.IHeaderAccess
    {
      private readonly ActionExecutingContext filterContext;

      public HeaderAccess(ActionExecutingContext filterContext)
      {
        this.filterContext = filterContext;
      }

      public string GetRequestHeader(string name)
      {
        return filterContext.RequestContext.HttpContext.Request.Headers[name];
      }

      public void SetResponseHeader(string name, string value)
      {
        filterContext.RequestContext.HttpContext.Response.AddHeader(name, value);
      }
    }
  }
}

#endif
