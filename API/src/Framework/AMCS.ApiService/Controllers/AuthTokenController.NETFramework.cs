#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AMCS.ApiService.Controllers.Responses;
using AMCS.Data;
using AMCS.Data.Server.Services;
using Swashbuckle.Swagger.Annotations;
using ICredentials = AMCS.Data.Server.Services.ICredentials;

namespace AMCS.ApiService.Controllers
{
  [Route("authTokens")]
  public class AuthTokenController : Controller
  {
    [HttpGet]
    [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LoginResponse))]
    public ActionResult Login(string userName, string password)
    {
      var credentials = new AMCS.Data.Server.Services.UserCredentials(userName, password);

      return DataServices.Resolve<IAuthenticationService>().Login(credentials, SessionTokenMode.AuthorizationHeader, HttpContext);
    }

    [HttpPost]
    [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LoginResponse))]
    public ActionResult Login(UserCredentials credentials)
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
    [Route("authTokens")]
    [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LogoutResponse))]
    public ActionResult Logout()
    {
      return DataServices.Resolve<IAuthenticationService>().Logout(HttpContext);
    }

    [HttpPost, Route("authTokens/checkPluginAccess")]
    [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CheckPluginResponse))]
    public ActionResult CheckPluginAccess(Guid sysUserGuid, int? pluginId)
    {
      return DataServices.Resolve<IAuthenticationService>().CheckPluginAccess(sysUserGuid, pluginId);
    } 

    [HttpPost, Route("authTokens/verifyIdentity")]
    [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VerifyIdentityResponse))]
    public ActionResult VerifyIdentity(VerifyIdentityCredentials credentials)
    {
      var authenticationCredentials = new AMCS.Data.Server.Services.UserCredentials(credentials.Username, credentials.Password);

      return DataServices.Resolve<IAuthenticationService>().VerifyIdentity(authenticationCredentials);
    }
  }
}

#endif
