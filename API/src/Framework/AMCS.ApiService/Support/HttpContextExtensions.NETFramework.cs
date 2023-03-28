#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AMCS.Data;
using AMCS.Data.Server.PlatformCredentials;

namespace AMCS.ApiService.Support
{
  public static class HttpContextExtensions
  {
    private static readonly NamedObject AuthenticatedUserKey = new NamedObject("AuthenticatedUser");
    private static readonly NamedObject PlatformCredentialsKey = new NamedObject("PlatformCredentials");

    public static ISessionToken GetAuthenticatedUser(this HttpContextBase self)
    {
      return (ISessionToken)self.Items[AuthenticatedUserKey];
    }

    public static void SetAuthenticatedUser(this HttpContextBase self, ISessionToken user)
    {
      self.Items[AuthenticatedUserKey] = user;
    }

    public static PlatformCredentials GetPlatformCredentials(this HttpContextBase self)
    {
      return (PlatformCredentials)self.Items[PlatformCredentialsKey];
    }

    public static void SetPlatformCredentials(this HttpContextBase self, PlatformCredentials credentials)
    {
      self.Items[PlatformCredentialsKey] = credentials;
    }
  }
}

#endif
