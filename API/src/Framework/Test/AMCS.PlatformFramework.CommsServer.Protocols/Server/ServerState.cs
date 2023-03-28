using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.PlatformFramework.CommsServer.Protocols.Server
{
  //
  // This helper class manages state associated with a server connection.
  // The static methods here expect a string map, which is available on the
  // IConnection. Using these static methods, we get and set the current
  // client state instance, which provides access to the server identifier
  // of the connected server.
  //
  public class ServerState
  {
    public static void Set(IDictionary<string, object> properties, ServerState clientState)
    {
      properties[typeof(ServerState).FullName] = clientState;
    }

    public static ServerState Get(IDictionary<string, object> properties)
    {
      return (ServerState)properties[typeof(ServerState).FullName];
    }

    public string Instance { get; }

    public ServerState(string instance)
    {
      Instance = instance;
    }
  }
}
