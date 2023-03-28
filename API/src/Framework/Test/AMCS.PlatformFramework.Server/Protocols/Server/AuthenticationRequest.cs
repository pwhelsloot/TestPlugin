using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.PlatformFramework.Server.Protocols.Server
{
  public class AuthenticationRequest
  {
    public string PrivateKey { get; set; }

    public string Instance { get; set; }
  }
}
