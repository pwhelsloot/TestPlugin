#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AMCS.ApiService
{
  internal class ApiVersionAttribute : ActionFilterAttribute
  {
    private readonly ApiVersionProvider apiVersionProvider;

    public ApiVersionAttribute(ApiVersionProvider apiVersionProvider)
    {
      this.apiVersionProvider = apiVersionProvider;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      var apiVersionQueryString = filterContext.RequestContext.HttpContext.Request.QueryString["api-version"];
      var apiVersionHeader = filterContext.RequestContext.HttpContext.Request.Headers["X-Api-Version"];
      
      if (!string.IsNullOrEmpty(apiVersionQueryString) && !string.IsNullOrEmpty(apiVersionHeader))
      {
        throw new HttpException((int)HttpStatusCode.BadRequest, "Please provide api version as a header or query string parameter");
      }

      if (!string.IsNullOrEmpty(apiVersionQueryString))
      {
        var apiVersion = GetApiVersionFromRequest(apiVersionQueryString);
        if (!IsValidVersion(apiVersion))
        {
          throw new HttpException((int)HttpStatusCode.BadRequest, $"Invalid Api Version: {apiVersion}");
        }

        filterContext.RequestContext.HttpContext.Request.Headers.Add("X-Api-Version", apiVersion);
      }
      else if (!string.IsNullOrEmpty(apiVersionHeader))
      {
        var apiVersion = GetApiVersionFromRequest(apiVersionHeader);
        if (!IsValidVersion(apiVersion))
        {
          throw new HttpException((int)HttpStatusCode.BadRequest, $"Invalid Api Version: {apiVersion}");
        }

        filterContext.RequestContext.HttpContext.Request.Headers["X-Api-Version"] = apiVersion;
      }
      else
      {
        filterContext.RequestContext.HttpContext.Request.Headers.Add("X-Api-Version", apiVersionProvider.LatestVersion.ToString());
      }

      base.OnActionExecuting(filterContext);
    }

    private string GetApiVersionFromRequest(string apiVersion)
    {
      return apiVersion == "latest" ? apiVersionProvider.LatestVersion.ToString() : apiVersion;
    }

    private bool IsValidVersion(string apiVersion)
    {
      var version = new Version(apiVersion);
      return version >= apiVersionProvider.CurrentVersion && version <= apiVersionProvider.LatestVersion;
    }
  }
}

#endif
