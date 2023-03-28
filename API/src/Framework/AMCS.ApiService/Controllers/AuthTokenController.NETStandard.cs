#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Controllers.Responses;
using AMCS.Data;
using AMCS.Data.Server.Services;
using Microsoft.AspNetCore.Mvc;
using ICredentials = AMCS.Data.Server.Services.ICredentials;

namespace AMCS.ApiService.Controllers
{
  [Route("authTokens")]
  public class AuthTokenController : Controller
  {
    [HttpGet]
    public ActionResult Login(string userName, string password)
    {
      var credentials = new AMCS.Data.Server.Services.UserCredentials(userName, password);

      return DataServices.Resolve<IAuthenticationService>().Login(credentials, SessionTokenMode.AuthorizationHeader, HttpContext);
    }

    [HttpPost]
    public ActionResult Login([FromBody] UserCredentials credentials)
    {
      ICredentials authenticationCredentials;

      if (!string.IsNullOrEmpty(credentials.PrivateKey))
        authenticationCredentials = new PrivateKeyCredentials(credentials.PrivateKey);
      else if (!string.IsNullOrEmpty(credentials.TemporaryToken))
        authenticationCredentials = new TokenCredentials(credentials.TemporaryToken);
      else
        authenticationCredentials = new AMCS.Data.Server.Services.UserCredentials(credentials.Username, credentials.Password);

      return DataServices.Resolve<IAuthenticationService>().Login(authenticationCredentials, SessionTokenMode.Cookie, HttpContext);
    }

    [HttpDelete]
    public ActionResult Logout()
    {
      return DataServices.Resolve<IAuthenticationService>().Logout(HttpContext);
    }

    [HttpPost, Route("checkPluginAccess")]
    public ActionResult CheckPluginAccess(Guid sysUserGuid, int? pluginId)
    {
      return DataServices.Resolve<IAuthenticationService>().CheckPluginAccess(sysUserGuid, pluginId);
    }

    [HttpPost, Route("verifyIdentity")]
    public ActionResult VerifyIdentity([FromBody] VerifyIdentityCredentials credentials)
    {
      var authenticationCredentials = new AMCS.Data.Server.Services.UserCredentials(credentials.Username, credentials.Password);

      return DataServices.Resolve<IAuthenticationService>().VerifyIdentity(authenticationCredentials);
    }
  }
}

#endif
