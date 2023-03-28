#if !NETFRAMEWORK

using System;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AMCS.ApiService
{
  internal class ApiVersionAttribute : ActionFilterAttribute
  {
    private readonly ApiVersionProvider apiVersionProvider;

    public ApiVersionAttribute(ApiVersionProvider apiVersionProvider)
    {
      this.apiVersionProvider = apiVersionProvider;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
      var apiVersionQueryString = context.HttpContext.Request.Query["api-version"];
      var apiVersionHeader = context.HttpContext.Request.Headers["X-Api-Version"];

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

        context.HttpContext.Request.Headers.Add("X-Api-Version", apiVersion);
      }
      else if (!string.IsNullOrEmpty(apiVersionHeader))
      {
        var apiVersion = GetApiVersionFromRequest(apiVersionHeader);
        if (!IsValidVersion(apiVersion))
        {
          throw new HttpException((int)HttpStatusCode.BadRequest, $"Invalid Api Version: {apiVersion}");
        }

        context.HttpContext.Request.Headers["X-Api-Version"] = apiVersion;
      }
      else
      {
        context.HttpContext.Request.Headers.Add("X-Api-Version", apiVersionProvider.LatestVersion.ToString());
      }

      base.OnActionExecuting(context);
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
