using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  public class AuthenticationFailure : IAuthenticationResult
  {
    public AuthenticationStatus Status { get; }

    public AuthenticationFailure(AuthenticationStatus status)
    {
      Status = status;
    }
  }
}
