using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AMCS.CommsServer.Client;
using AMCS.CommsServer.Client.Storage.Memory;
using AMCS.CommsServer.Client.Transport.WebSockets;
using AMCS.CommsServer.Serialization;
using AMCS.PlatformFramework.CommsServer.EndToEndIntegrationTest.Support;
using AMCS.PlatformFramework.CommsServer.Messages;
using log4net;
using Newtonsoft.Json;

namespace AMCS.PlatformFramework.CommsServer.EndToEndIntegrationTest.Client
{
  public class Client
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(Client));

    private readonly Arguments arguments;
    private readonly TimeSpan timeout;

    public Client(Arguments arguments, TimeSpan timeout)
    {
      this.arguments = arguments;
      this.timeout = timeout;
    }

    public void Run(string tenantId)
    {
      // Create the authentication payload. The Comms Server expects a string.
      // In our protocols, we're using the ClientAuthenticationRequest class
      // the model the authentication payload, which we create here and send
      // JSON serialized.

      string authenticationPayload = JsonConvert.SerializeObject(new ClientAuthenticationRequest
      {
        Instance = tenantId,
        PrivateKey = "let-me-in"
      });
      
      // Setup a Comms Server client. The configuration below uses in memory
      // storage. This is fine for a demo, but in production you'd use either
      // an SQLite storage mechanism for e.g. an Android client, or a custom
      // SQL Server storage mechanism if an SQL Server database is available.
      // See Scale for a sample implementation.

      var configuration = new CommsServerClientConfiguration
      {
        Endpoint = new Endpoint(arguments.Endpoint, "platform-framework-client", authenticationPayload),
        Storage = new InMemoryStorage(),
        TransportFactory = new WebSocketTransportFactory
        {
          EnableCompression = true
        }
      };

      using (var client = new CommsServerClient(configuration))
      using (var @event = new ManualResetEventSlim())
      {
        string payload = Guid.NewGuid().ToString();

        // The client.Receiver.Received event is raised when a message is received.
        // Here we deserialize the message and check the payload against what we
        // originally sent.

        client.Receiver.Received += (s, e) =>
        {
          // We use the Type of the message as a full type name of the message.
          // Here we do this just as a demo. Other mechanisms like message numbers
          // are also feasible.

          var message = MessageSerializer.Deserialize(e.Message.Type, e.Message.Body);

          if (
            message is LoginResponseMessage loginResponse &&
            loginResponse.Pong == payload
          )
          {
            Log.Info("Received login response");
            @event.Set();
          }
        };

        client.Open();

        Log.Info("Sending login request");
        
        // The client.Sender.Publish method sends a message to the message
        // Comms Server. We do the same thing we expect above, using the full
        // type name as the message type and the JSON serialized message
        // as the message body.

        var loginRequest = new LoginRequestMessage
        {
          Ping = payload
        };

        client.Sender.Publish(new Message(
          Message.NewId(),
          MessageSerializer.GetMessageType(loginRequest),
          MessageSerializer.Serialize(loginRequest),
          null
        ));

        Log.Info("Waiting for response");

        // Wait for the receiver to receive the message, making a full roundtrip
        // through the server.

        @event.WaitForCompletion(timeout, true);
      }
    }
  }
}
