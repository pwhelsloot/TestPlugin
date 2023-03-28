using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.PlatformFramework.CommsServer.Messages
{
  public class LoginRequestMessage : IMessage
  {
    public string Ping { get; set; }
  }
}
