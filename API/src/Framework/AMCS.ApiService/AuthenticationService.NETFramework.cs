#if NETFRAMEWORK

namespace AMCS.ApiService
{
  using System;
  using System.Globalization;
  using System.Linq;
  using System.Net.Http.Headers;
  using System.Web;
  using System.Web.Mvc;
  using Controllers.Responses;
  using Data;
  using Data.Configuration;
  using Data.Server;
  using Data.Server.PlatformCredentials;
  using Data.Server.Services;
  using log4net;
  using Newtonsoft.Json;
  using Support;
  using ICredentials = Data.Server.Services.ICredentials;

  internal class AuthenticationService : IAuthenticationService
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(AuthenticationService));

    private readonly IUserService userService;

    public AuthenticationService(IUserService userService)
    {
      this.userService = userService;
    }

    public ISessionToken Authenticate(HttpContextBase context)
    {
      var platformCredentials = GetPlatformCredentials(context);

      var userToken =
        GetUserToken(context) ??
        TryCoreLogin(context, platformCredentials);

      if (userToken != null)
      {
        SetAuthTokenCookie(context, userToken);

        if (platformCredentials != null)
          context.SetPlatformCredentials(platformCredentials);
      }

      return userToken;
    }

    public ActionResult Initialise(HttpContextBase context)
    {
      // Attempt to load user from context
      var user = context.GetAuthenticatedUser();

      // If user not found, attempt to load from cookie
      if (user == null && userService.SupportsOfflineMode)
      {
        var cookie = context.Request.Cookies.Get(userService.GetApplicationCookieName());
        if (cookie != null && !string.IsNullOrWhiteSpace(cookie.Value))
          userService.TryDeserializeSessionToken(cookie.Value, out user);
      }

      if (user != null)
      {
        var authenticationUser = new AuthenticationUser(
          user.UserName,
          null,
          user.UserId,
          null,
          user.CompanyOutletId,
          null,
          user.IsOfflineModeEnabled,
          user.TenantId,
          user.Language);

        return this.GetCookieLoginResult(authenticationUser, context);
      }

      return new HttpUnauthorizedResult();
    }

    public ActionResult Login(ICredentials credentials, SessionTokenMode mode, HttpContextBase context)
    {
      var result = userService.Authenticate(credentials);

      switch (mode)
      {
        case SessionTokenMode.AuthorizationHeader:
          return GetHeaderLoginResult(result);

        case SessionTokenMode.Cookie:
          return GetCookieLoginResult(result, context);

        default:
          throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
      }
    }

    public ActionResult Logout(HttpContextBase context)
    {
      var userToken = GetUserToken(context);

      Log.Info("Logout");

      if (userToken != null)
      {
        userService.Logout(userToken);

        // If the request has the cookie, set the response cookie to
        // the same with an expiration date of 1970-1-1 to expire the cookie.
        // Algorithm taken from https://stackoverflow.com/questions/12116511/.

        var cookie = context.Request.Cookies.Get(userService.GetApplicationCookieName());

        if (cookie != null)
        {
          Log.Info("Logout - removing auth cookie");

          context.Response.Cookies.Remove(userService.GetApplicationCookieName());

          cookie.Value = null;
          cookie.Expires = new DateTime(1970, 1, 1);

          context.Response.Cookies.Add(cookie);
        }
      }

      return JsonResult(new LogoutResponse
      {
        LoggedOut = "yes"
      });
    }

    public ActionResult CheckPluginAccess(Guid sysUserId, int? pluginId)
    {
      var result = userService.CheckPluginAccess(sysUserId, pluginId);

      return JsonResult(new CheckPluginResponse
      {
        Result = result
      });
    }

    public ActionResult VerifyIdentity(ICredentials credentials)
    {
      var result = userService.Authenticate(credentials);

      switch (result)
      {
        case AuthenticationUser user:
          string identity;

          if (!string.IsNullOrEmpty(user.UserIdentity))
            identity = user.UserIdentity;
          else
            identity = "username:" + user.UserName;

          return JsonResult(new
          {
            result = "ok",
            identity
          });

        case AuthenticationFailure failure:
          string status;

          switch (failure.Status)
          {
            case AuthenticationStatus.InvalidCredentials:
              status = "invalidCredentials";
              break;

            case AuthenticationStatus.Locked:
              status = "accountLocked";
              break;

            case AuthenticationStatus.DuplicateEmail:
              status = "duplicateEmail";
              break;

            default:
              status = "unknown";
              break;
          }

          return JsonResult(new
          {
            result = status
          });

        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private PlatformCredentials GetPlatformCredentials(HttpContextBase context)
    {
      var cookie = context.Request.Cookies.Get(PlatformCredentials.CookieName);
      if (cookie == null)
        return null;

      var platformCredentials = DataServices.Resolve<IPlatformCredentialsTokenManager>().Deserialize(cookie.Value);
      return platformCredentials;
    }

    private ActionResult GetCookieLoginResult(IAuthenticationResult result, HttpContextBase context)
    {
      switch (result)
      {
        case AuthenticationUser user:
          SetAuthTokenCookie(context, userService.CreateSessionToken(user));

          return JsonResult(new LoginResponse
          {
            AuthResult = "ok",
            UserName = user.UserName,
            UserIdentity = user.UserIdentity,
            SysUserId = user.UserId,
            CompanyOutletId = user.CompanyOutletId,
            UserGuid = user.UserGuid,
            CompanyOutletGuid = user.CompanyOutletGuid
          });

        case AuthenticationFailure failure:
          string status;

          switch (failure.Status)
          {
            case AuthenticationStatus.InvalidCredentials:
              status = "invalidCredentials";
              break;

            case AuthenticationStatus.Locked:
              status = "accountLocked";
              break;

            case AuthenticationStatus.DuplicateEmail:
              status = "duplicateEmail";
              break;

            default:
              status = "unknown";
              break;
          }

          return JsonResult(new LoginResponse
          {
            AuthResult = status
          });

        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void SetAuthTokenCookie(HttpContextBase context, ISessionToken token)
    {
      var responseCookie = context.Response.Cookies.Get(userService.GetApplicationCookieName());

      responseCookie.Value = userService.SerializeSessionToken(token);
      responseCookie.Path = new Uri(DataServices.Resolve<IServiceRootResolver>().ServiceRoot).AbsolutePath;
      // The cookie is created as a session cookie, so no expiration. In browsers
      // this ensures that the cookie lives until the browser is closed.
      responseCookie.Expires = DateTime.MinValue;
      responseCookie.HttpOnly = true;
    }

    private ActionResult GetHeaderLoginResult(IAuthenticationResult result)
    {
      switch (result)
      {
        case AuthenticationUser user:
          var userId = userService.CreateSessionToken(user);
          string sessionToken = userService.SerializeSessionToken(userId);

          return new ContentResult
          {
            Content = sessionToken
          };

        default:
          return new HttpUnauthorizedResult();
      }
    }

    private ISessionToken GetUserToken(HttpContextBase context)
    {
      // We don't care how we get the token. Either of the modes is fine.
      // We first check the cookie, because this enables automatic token
      // refresh if the initial authentication was using a bearer token.

      var request = context.Request;

      var cookie = request.Cookies.Get(userService.GetApplicationCookieName());
      if (cookie != null && userService.TryDeserializeSessionToken(cookie.Value, out var sessionToken) == SessionTokenStatus.Valid)
        return sessionToken;

      // Otherwise, check the bearer token.

      var authorizations = request.Headers.GetValues("Authorization");
      if (authorizations?.Length == 1)
      {
        string authorization = authorizations[0];
        const string Prefix = "Bearer ";
        if (authorization.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
        {
          string token = authorization.Substring(Prefix.Length).Trim();
          if (userService.TryDeserializeSessionToken(token, out sessionToken) == SessionTokenStatus.Valid)
            return sessionToken;
        }
      }

      return null;
    }

    private ISessionToken TryCoreLogin(HttpContextBase context, PlatformCredentials platformCredentials)
    {
      if (platformCredentials == null)
        return null;

      var credentials = GetCredentialsForCoreLogin(platformCredentials);

      var loginResult = userService.Authenticate(credentials);
      if (loginResult is AuthenticationUser user)
        return userService.CreateSessionToken(user);

      return null;
    }

    private ICredentials GetCredentialsForCoreLogin(PlatformCredentials credentials)
    {
      var identity = credentials.UserIdentity;
      var tenant = credentials.TenantId;

      // For users that don't have an email address set, the identity will be
      // a user name prefixed with "username:".
      const string userNamePrefix = "username:";
      if (identity.StartsWith(userNamePrefix))
        return new UserNameCredentials(identity.Substring(userNamePrefix.Length), tenant);

      const string appPrefix = "app:";
      if (identity.StartsWith(appPrefix))
        return new AppCredentials(identity.Substring(appPrefix.Length), tenant);

      return new IdentityCredentials(identity, tenant);
    }

    private ActionResult JsonResult(object json)
    {
      return new ContentResult
      {
        Content = JsonConvert.SerializeObject(json),
        ContentType = "application/json"
      };
    }
  }
}

#endif