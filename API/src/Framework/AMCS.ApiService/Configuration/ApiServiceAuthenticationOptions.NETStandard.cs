#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace AMCS.ApiService.Configuration
{
  public class ApiServiceAuthenticationOptions : AuthenticationSchemeOptions
  {
    public IAuthenticationService AuthenticationService { get; set; }
  }
}

#endif
