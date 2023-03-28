#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AMCS.ApiService.Support
{
  public class CorsOptionsModule : IHttpModule
  {
    public void Init(HttpApplication context)
    {
      context.BeginRequest += Context_BeginRequest;
    }

    private void Context_BeginRequest(object sender, EventArgs e)
    {
      var context = ((HttpApplication)sender).Context;

      if (context.Request.HttpMethod == "OPTIONS")
      {
        CorsConfiguration.Configure(new HeaderAccess(context));
        context.ApplicationInstance.CompleteRequest();
      }
    }

    public void Dispose()
    {
    }

    private class HeaderAccess : CorsConfiguration.IHeaderAccess
    {
      private readonly HttpContext context;

      public HeaderAccess(HttpContext context)
      {
        this.context = context;
      }

      public string GetRequestHeader(string name)
      {
        return context.Request.Headers[name];
      }

      public void SetResponseHeader(string name, string value)
      {
        context.Response.Headers.Add(name, value);
      }
    }
  }
}

#endif
