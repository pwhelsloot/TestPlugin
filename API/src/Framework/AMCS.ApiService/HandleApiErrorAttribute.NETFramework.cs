#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using log4net;
using Newtonsoft.Json;

namespace AMCS.ApiService
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  internal class HandleApiErrorAttribute : FilterAttribute, IExceptionFilter
  { 
	private static readonly ILog Log = LogManager.GetLogger(typeof(HandleApiErrorAttribute));
    public void OnException(ExceptionContext filterContext)
    {
      if (filterContext.IsChildAction || filterContext.ExceptionHandled)
        return;
      
      int httpCode = new HttpException(null, filterContext.Exception).GetHttpCode();
      
      switch (httpCode)
      {
        case 401:
          filterContext.ExceptionHandled = true;
          SetResponseUnauthorised(filterContext);
          break;
        case 404:
          filterContext.ExceptionHandled = true;
          SetResponseNotFound(filterContext);
          break;
        case 500:
          filterContext.ExceptionHandled = true;
          SetResponseInternalError(filterContext);
          break;
        default:
          return;
      }
      Log.Error("Exception caught", filterContext.Exception);
    }

    private void SetResponseUnauthorised(ExceptionContext filterContext)
    {
      var response = filterContext.HttpContext.Response;

      response.Clear();
      response.StatusCode = 401;
      response.TrySkipIisCustomErrors = true;
      response.Write("Unauthorised");
    }

    private void SetResponseInternalError(ExceptionContext filterContext)
    {
      var response = filterContext.HttpContext.Response;

      response.Clear();
      response.StatusCode = 500;
      response.TrySkipIisCustomErrors = true;

      using (var writer = new StringWriter())
      {
        using (var json = new JsonTextWriter(writer))
        {
          ErrorResponseWriter.WriteError(json, filterContext.Exception, filterContext.HttpContext.IsCustomErrorEnabled);
        }

        response.ContentType = "application/json";
        response.Write(writer.ToString());
      }
    }

    private void SetResponseNotFound(ExceptionContext filterContext)
    {
      var response = filterContext.HttpContext.Response;

      response.Clear();
      response.StatusCode = 404;
      response.TrySkipIisCustomErrors = true;

      using (var writer = new StringWriter())
      {
        using (var json = new JsonTextWriter(writer))
        {
          ErrorResponseWriter.WriteError(json, filterContext.Exception, filterContext.HttpContext.IsCustomErrorEnabled);
        }

        response.ContentType = "application/json";
        response.Write(writer.ToString());
      }
    }
  }
}

#endif
