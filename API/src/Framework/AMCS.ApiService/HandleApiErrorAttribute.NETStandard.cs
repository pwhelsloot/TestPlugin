#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using log4net;
using Newtonsoft.Json;

namespace AMCS.ApiService
{
  internal class HandleApiErrorAttribute : IAsyncExceptionFilter
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(HandleApiErrorAttribute));

    private readonly bool showErrorDetails;

    public HandleApiErrorAttribute(bool showErrorDetails = false)
    {
      this.showErrorDetails = showErrorDetails;
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
      if (context.ExceptionHandled)
        return;
      
      int httpCode = new HttpException(null, context.Exception).GetHttpCode();
      
      switch (httpCode)
      {
        case 401:
          context.ExceptionHandled = true;
          await SetResponseUnauthorisedAsync(context);
          break;
        case 404:
          context.ExceptionHandled = true;
          await SetResponseNotFoundAsync(context);
          break;
        case 500:
          context.ExceptionHandled = true;
          await SetResponseInternalErrorAsync(context);
          break;
        default:
          return;
      }
      Log.Error("Exception caught", context.Exception);
    }

    private async Task SetResponseUnauthorisedAsync(ExceptionContext filterContext)
    {
      var response = filterContext.HttpContext.Response;

      response.Clear();
      response.StatusCode = 401;
      await response.WriteAsync("Unauthorised");
    }

    private async Task SetResponseInternalErrorAsync(ExceptionContext filterContext)
    {
      var response = filterContext.HttpContext.Response;

      response.Clear();
      response.StatusCode = 500;

      using (var writer = new StringWriter())
      {
        using (var json = new JsonTextWriter(writer))
        {
          ErrorResponseWriter.WriteError(json, filterContext.Exception, showErrorDetails);
        }

        response.ContentType = "application/json";
        await response.WriteAsync(writer.ToString());
      }
    }

    private async Task SetResponseNotFoundAsync(ExceptionContext filterContext)
    {
      var response = filterContext.HttpContext.Response;

      response.Clear();
      response.StatusCode = 404;

      using (var writer = new StringWriter())
      {
        using (var json = new JsonTextWriter(writer))
        {
          ErrorResponseWriter.WriteError(json, filterContext.Exception, showErrorDetails);
        }

        response.ContentType = "application/json";
        await response.WriteAsync(writer.ToString());
      }
    }
  }
}

#endif
