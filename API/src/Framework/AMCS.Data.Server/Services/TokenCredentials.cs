using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  public class TokenCredentials : ICredentials
  {
    public string Token { get; }

    public TokenCredentials(string token)
    {
      Token = token;
    }
  }
}
