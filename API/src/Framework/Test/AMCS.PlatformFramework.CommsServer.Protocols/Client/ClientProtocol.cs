using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.CommsServer.Serialization;
using AMCS.CommsServer.Server;
using AMCS.CommsServer.Server.Support;
using AMCS.PlatformFramework.CommsServer.Protocols.Server;
using Newtonsoft.Json;

namespace AMCS.PlatformFramework.CommsServer.Protocols.Client
{
  //
  // This is a sample client protocol implementation. The purpose of this protocol
  // is to accept connections from one or more clients, and forward messages to the
  // server.
  //
  // As part of the authentication, clients send a client identifier. In the protocol,
  // we use this to distinguish clients. When we receive a message from a client,
  // we wrap the message in a new ClientMessage class, passing the client identifier
  // with the message. The server protocol then uses this identifier to wrap the
  // message in a new message, passing the client identifier back to the server.
  //
  // Since this end of the protocol supports multiple connections, we need to manage
  // the queues for those connections. The SenderManager is built to support that.
  // That class will keep a map of opened senders, and will reuse those if a new
  // message becomes available for a client.
  //
  [Protocol(Name)]
  public class ClientProtocol : ISessionProtocol
  {
    public const string Name = "platform-framework-client";

    private IProtocolContext contextValue;
    private Properties propertiesValue;
    private IRouter router;
    private SenderManager senderManager;
    private bool disposed;

    public void Open(IProtocolContext context, Properties properties)
    {
      this.contextValue = context;
      this.propertiesValue = properties;

      router = context.CreateRouter(ServerProtocol.Name);

      senderManager = new SenderManager(context);
    }

    public Session CreateSession(string authenticationPayload)
    {
      var request = JsonConvert.DeserializeObject<ClientAuthenticationRequest>(authenticationPayload);
      if (propertiesValue[Constants.AuthenticationKeyPropertyName] == request.PrivateKey)
      {
        var state = new ClientState(request.Instance);
        return Session.Create(Name, JsonConvert.SerializeObject(state));
      }

      return null;
    }

    public IReceiver OpenSession(IConnection connection)
    {
      var state = JsonConvert.DeserializeObject<ClientState>(connection.SessionState);

      ClientState.Set(connection.Properties, state);

      return contextValue.CreateReceiver(state.Instance);
    }

    public void Accept(IRequest request, Message message)
    {
      var state = ClientState.Get(request.Connection.Properties);

      var clientMessage = new ClientMessage(message.Id, message.Type, message.Body, message.CorrelationId, state.Instance);

      router.Route(request, clientMessage);
    }

    public void Publish(IOperationScope scope, Message message)
    {
      string client = ((ClientMessage)message).Client;

      senderManager.GetSender(client).Publish(scope, message);
    }

    public void Dispose()
    {
      if (!disposed)
      {
        if (senderManager != null)
        {
          senderManager.Dispose();
          senderManager = null;
        }

        disposed = true;
      }
    }
  }
}
