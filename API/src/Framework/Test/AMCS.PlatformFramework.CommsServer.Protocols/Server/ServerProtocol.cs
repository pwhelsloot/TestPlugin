using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.CommsServer.Serialization;
using AMCS.CommsServer.Server;
using AMCS.CommsServer.Server.Support;
using AMCS.PlatformFramework.CommsServer.Protocols.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMCS.PlatformFramework.CommsServer.Protocols.Server
{
  //
  // This is a sample server protocol implementation. This implementation
  // expects at most one server connected at any time, and will forward all
  // messages coming from a client to that server.
  //
  // The messages we get from the client aren't Comms Server Message instances,
  // but instances of ClientMessage instead. This allows us to piggyback the
  // client identifier with the message, we we use here to wrap the message
  // into a wrapper that contains the client identifier. When we receive
  // a message from the server, we expect that to also be in a wrapper with
  // the client identifier, and use that to convert the message to a
  // ClientMessage again, which the client protocol uses to direct the message
  // to the correct client.
  //
  // In this protocol, since we only have one server connected, we only
  // need a single sender, which we open when the protocol is opened.
  //
  [Protocol(Name)]
  public class ServerProtocol : ISessionProtocol
  {
    public const string Name = "platform-framework-server";

    private IProtocolContext contextValue;
    private Properties propertiesValue;
    private IRouter router;
    private SenderManager senderManager;
    private bool disposed;

    public void Open(IProtocolContext context, Properties properties)
    {
      this.contextValue = context;
      this.propertiesValue = properties;

      router = context.CreateRouter(ClientProtocol.Name);

      senderManager = new SenderManager(context);
    }

    public Session CreateSession(string authenticationPayload)
    {
      var request = JsonConvert.DeserializeObject<ServerAuthenticationRequest>(authenticationPayload);

      if (propertiesValue[Constants.AuthenticationKeyPropertyName] == request.PrivateKey)
      {
        var state = new ServerState(request.Instance);
        return Session.Create(Name, JsonConvert.SerializeObject(state));
      }

      return null;
    }

    public IReceiver OpenSession(IConnection connection)
    {
      var state = JsonConvert.DeserializeObject<ServerState>(connection.SessionState);

      ServerState.Set(connection.Properties, state);

      return contextValue.CreateReceiver(state.Instance);
    }

    public void Accept(IRequest request, Message message)
    {
      var dto = JsonConvert.DeserializeObject<ServerMessageDto>(message.Body);

      var clientMessage = new ClientMessage(message.Id, message.Type, (string)dto.Data.Value, message.CorrelationId, dto.Client);

      router.Route(request, clientMessage);
    }

    public void Publish(IOperationScope scope, Message message)
    {
      string client = ((ClientMessage)message).Client;

      var dto = new ServerMessageDto
      {
        Client = client,
        Data = new JRaw(message.Body)
      };

      senderManager.GetSender(client).Publish(scope, message.Id, message.Type, JsonConvert.SerializeObject(dto), message.CorrelationId);
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
