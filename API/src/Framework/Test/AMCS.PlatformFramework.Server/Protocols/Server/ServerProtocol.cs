namespace AMCS.PlatformFramework.Server.Protocols.Server
{
  using System.Collections.Generic;
  using System.Threading;
  using AMCS.ApiService.Abstractions.CommsServer;
  using AMCS.ApiService.CommsServer;
  using AMCS.CommsServer.Serialization;
  using AMCS.PlatformFramework.CommsServer.Messages;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  
  [CommsServerProtocol(Name)]
  public class ServerProtocol : ProtocolHandler
  {
    public const string Name = "platform-framework-server";

    public ServerProtocol(ICommsServerClient client) : base(client)
    {
    }

    protected override void DoProcessMessage(Message message, CancellationToken cancellationToken)
    {
      var serverMessage = JsonConvert.DeserializeObject<ServerMessage>(message.Body);
      var clientMessage = MessageSerializer.Deserialize(message.Type, (string)serverMessage.Data.Value);

      var responseMessages = new List<Message>();

      if (clientMessage is LoginRequestMessage loginRequest)
      {
        var loginResponse = new LoginResponseMessage
        {
          Pong = loginRequest.Ping
        };

        responseMessages.Add(CreateMessage(serverMessage.Client, loginResponse));
      }

      Publish(responseMessages);
    }

    private static Message CreateMessage(string client, IMessage message)
    {
      var dto = new ServerMessage
      {
        Client = client,
        Data = new JRaw(JsonConvert.SerializeObject(message))
      };

      // We're not using MessageSerializer.Serialize because we're not
      // serializing a message, we're serializing a DTO.

      return new Message(
        Message.NewId(),
        MessageSerializer.GetMessageType(message),
        JsonConvert.SerializeObject(dto),
        null
      );
    }
  }
}
