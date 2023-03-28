#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AMCS.ApiService.Support
{
  internal static class HttpUtils
  {
    public static bool ExpectXmlResponse(HttpRequestBase request)
    {
      var accept = AcceptParser.Parse(request.Headers["Accept"]);

      bool hasJsonWeight = accept.TryGetWeight("application/json", out double jsonWeight);
      bool hasXmlWeight = accept.TryGetWeight("text/xml", out double xmlWeight);

      return hasXmlWeight && (!hasJsonWeight || xmlWeight > jsonWeight);
    }
  }
}

#endif
