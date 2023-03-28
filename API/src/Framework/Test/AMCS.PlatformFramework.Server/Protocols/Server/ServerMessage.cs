using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AMCS.PlatformFramework.Server.Protocols.Server
{
  public class ServerMessage
  {
    public string Client { get; set; }

    public JRaw Data { get; set; }
  }
}
