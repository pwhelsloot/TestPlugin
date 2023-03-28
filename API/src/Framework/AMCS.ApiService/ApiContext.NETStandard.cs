#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AMCS.ApiService.Support;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace AMCS.ApiService
{
  internal class ApiContext : IApiContext
  {
    private readonly HttpContext context;

    public string BaseUrl => GetRealBaseUrl();

    public ApiContext(HttpContext context)
    {
      this.context = context;
    }

    public Exception CreateHttpException(HttpStatusCode statusCode, string status = null)
    {
      return new HttpException((int)statusCode, status);
    }

    private string GetRealBaseUrl()
    {
      // Detect the base URL of the application. This enforces standard X-Forwarded-*
      // headers to get the correct public facing URL.
      //
      // See e.g. https://www.nginx.com/resources/wiki/start/topics/examples/forwarded/ for more information.

      var forwardedProto = (string)context.Request.Headers["X-Forwarded-Proto"];
      string proto = forwardedProto ?? context.Request.Scheme;

      string host = (string)context.Request.Headers["X-Forwarded-Host"] ?? context.Request.Host.Host;

      // Getting the port is a bit fiddly:
      //
      // * If we have an X-Forwarded-Port, use that;
      // * If the host (from X-Forwarded-For; Uri doesn't do this) has a ':', take the port from that;
      // * If X-Forwarded-Proto was set, infer the port from that. Uri.Port may not be correct;
      // * If all else fails (basically we didn't have X-Forwarded-* headers), take the port from Uri.

      int hostPortPos = host.LastIndexOf(':');

      int? port;

      string portValue = context.Request.Headers["X-Forwarded-Port"];
      if (portValue != null)
        port = int.Parse(portValue);
      else if (hostPortPos != -1)
        port = int.Parse(host.Substring(hostPortPos + 1));
      else
        port = context.Request.Host.Port;

      // Strip the port from the host if there is one.

      if (hostPortPos != -1)
        host = host.Substring(0, hostPortPos);

      var sb = new StringBuilder();

      sb.Append(proto).Append("://").Append(host);

      if (port.HasValue && port.Value != (proto == "http" ? 80 : 443))
        sb.Append(':').Append(port.Value);

      sb.Append('/');

      context.Request.GetDisplayUrl();
      var applicationPath = context.Request.PathBase + context.Request.Path + context.Request.QueryString;
      if (!string.IsNullOrEmpty(applicationPath))
      {
        applicationPath = applicationPath.Trim('/');
        if (applicationPath.Length > 0)
          sb.Append(applicationPath).Append('/');
      }

      return sb.ToString();
    }
  }
}

#endif
