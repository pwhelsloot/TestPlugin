using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Support
{
  internal static class CorsConfiguration
  {
    public static void Configure(IHeaderAccess headers)
    {
      headers.SetResponseHeader("Access-Control-Allow-Origin", headers.GetRequestHeader("Origin") ?? "*");
      headers.SetResponseHeader("Access-Control-Allow-Credentials", "true");
      headers.SetResponseHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
      headers.SetResponseHeader("Access-Control-Allow-Headers", "AccessKey, Authorization, Content-Type");
    }

    public interface IHeaderAccess
    {
      string GetRequestHeader(string name);

      void SetResponseHeader(string name, string value);
    }
  }
}
