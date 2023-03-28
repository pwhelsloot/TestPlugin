using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.PlatformFramework.CommsServer.Protocols.Server
{
  public class ServerAuthenticationRequest
  {
    public string PrivateKey { get; set; }

    public string Instance { get; set; }
  }
}
