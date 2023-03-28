#if NETFRAMEWORK

using System;
using System.Web;
using System.Web.Mvc;
using AMCS.Data;
using AMCS.Data.Server.Services;

namespace AMCS.ApiService
{
  public interface IAuthenticationService
  {
    ISessionToken Authenticate(HttpContextBase context);

    ActionResult Initialise(HttpContextBase context);

    ActionResult Login(ICredentials credentials, SessionTokenMode mode, HttpContextBase context);

    ActionResult Logout(HttpContextBase context);

    ActionResult CheckPluginAccess(Guid sysUserId, int? pluginId);

    ActionResult VerifyIdentity(ICredentials credentials);
  }
}

#endif
