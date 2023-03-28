#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace AMCS.ApiService
{
  public class ApiAuthorizeAttribute : AuthorizeAttribute
  {
    public ApiAuthorizeAttribute()
    {
      AuthenticationSchemes = ApiServiceAuthenticationHandler.SchemeName;
    }
  }
}

#endif
