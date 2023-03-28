#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AMCS.ApiService.Support;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AMCS.ApiService.Configuration
{
  public class ApiServiceAuthenticationHandler : AuthenticationHandler<ApiServiceAuthenticationOptions>
  {
    public const string SchemeName = "sessiontoken";

    public ApiServiceAuthenticationHandler(IOptionsMonitor<ApiServiceAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
      : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      var user = Options.AuthenticationService?.Authenticate(Context);
      if (user == null)
      {
        return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
      }

      Context.SetAuthenticatedUser(user);

      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
      };

      if (user.UserIdentity != null)
        claims.Add(new Claim(ClaimTypes.Email, user.UserIdentity));

      var identity = new ClaimsIdentity(claims, Scheme.Name);
      var principal = new ClaimsPrincipal(identity);
      var ticket = new AuthenticationTicket(principal, Scheme.Name);

      return Task.FromResult(AuthenticateResult.Success(ticket));
    }
  }
}

#endif
