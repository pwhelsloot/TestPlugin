namespace AMCS.PlatformFramework.CommsServer.Protocols.Client
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  //
  // This helper class manages state associated with a client connection.
  // The static methods here expect a string map, which is available on the
  // IConnection. Using these static methods, we get and set the current
  // client state instance, which provides access to the client identifier
  // of the connected client.
  //
  public class ClientState
  {
    public static void Set(IDictionary<string, object> properties, ClientState clientState)
    {
      properties[typeof(ClientState).FullName] = clientState;
    }

    public static ClientState Get(IDictionary<string, object> properties)
    {
      return (ClientState)properties[typeof(ClientState).FullName];
    }

    public string Instance { get; }

    public ClientState(string instance)
    {
      Instance = instance;
    }
  }
}
