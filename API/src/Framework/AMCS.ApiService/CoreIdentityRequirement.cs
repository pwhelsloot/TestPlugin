namespace AMCS.ApiService
{
  using System;
  using System.Linq;
  using System.Security.Claims;
  using System.Threading.Tasks;
  using AMCS.Data.Server;
  using Microsoft.AspNetCore.Authorization;
  
  public class CoreIdentityRequirement : AuthorizationHandler<CoreIdentityRequirement>, IAuthorizationRequirement
  {
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CoreIdentityRequirement requirement)
    {
      if (!context.User.HasClaim(ctx => ctx.Type == ClaimTypes.Email || ctx.Type == ClaimTypes.Name))
      {
        context.Fail();
        return Task.CompletedTask;
      }
      
      var emailClaim = context.User.Claims
        .SingleOrDefault(ctx => ctx.Type == ClaimTypes.Email)?.Value;

      var nameClaim = context.User.Claims
        .SingleOrDefault(ctx => ctx.Type == ClaimTypes.Name)?.Value;
      
      if (string.Equals(WellKnownIdentities.CoreApp, emailClaim, StringComparison.InvariantCultureIgnoreCase) ||
          string.Equals(WellKnownIdentities.CoreApp, nameClaim, StringComparison.InvariantCultureIgnoreCase))
      {
        context.Succeed(requirement);
        return Task.CompletedTask;
      }

      context.Fail();
      return Task.CompletedTask;
    }
  }
}