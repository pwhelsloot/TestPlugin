#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMCS.ApiService
{
  public interface IAuthenticationService
  {
    ISessionToken Authenticate(HttpContext context);

    ActionResult Initialise(HttpContext context);

    ActionResult Login(ICredentials credentials,  SessionTokenMode mode, HttpContext context);

    ActionResult Logout(HttpContext context);

    ActionResult CheckPluginAccess(Guid sysUserId, int? pluginId);

    ActionResult VerifyIdentity(ICredentials credentials);
  }
}

#endif
