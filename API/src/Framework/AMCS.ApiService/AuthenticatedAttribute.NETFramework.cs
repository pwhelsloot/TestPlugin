#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AMCS.ApiService.Support;
using AMCS.Data;

namespace AMCS.ApiService
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public class AuthenticatedAttribute : ActionFilterAttribute
  {
    private readonly IAuthenticationService authenticationService;

    public string RequiredIdentity { get; set; }

    public AuthenticatedAttribute()
    {
      DataServices.TryResolve(out authenticationService);
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      var user = authenticationService?.Authenticate(filterContext.HttpContext);
      if (user == null)
        throw new UnauthorizedAccessException();

      var credentials = filterContext.HttpContext.GetPlatformCredentials();
      if (!string.IsNullOrEmpty(RequiredIdentity) && credentials?.UserIdentity != RequiredIdentity)
        throw new UnauthorizedAccessException("Required Identity Check Failed");

      filterContext.HttpContext.SetAuthenticatedUser(user);
    }
  }
}

#endif
