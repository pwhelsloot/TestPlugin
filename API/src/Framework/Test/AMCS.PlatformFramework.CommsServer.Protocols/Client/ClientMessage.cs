using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.CommsServer.Serialization;

namespace AMCS.PlatformFramework.CommsServer.Protocols.Client
{
  public class ClientMessage : Message
  {
    public string Client { get; }

    public ClientMessage(string id, string type, string body, string correlationId, string client)
      : base(id, type, body, correlationId)
    {
      Client = client;
    }
  }
}
