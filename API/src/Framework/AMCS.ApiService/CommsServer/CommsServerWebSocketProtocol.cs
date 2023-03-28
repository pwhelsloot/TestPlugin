namespace AMCS.ApiService.CommsServer
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using AMCS.ApiService.Abstractions.CommsServer;
  using AMCS.CommsServer.Client;
  using AMCS.CommsServer.Client.Transport.WebSockets;
  using AMCS.CommsServer.Serialization;
  using Data;
  using Data.Server.Services;
  using Endpoint = AMCS.CommsServer.Client.Endpoint;
  using log4net;

  internal partial class CommsServerWebSocketProtocol : ICommsServerClient
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(CommsServerWebSocketProtocol));

    private static readonly TimeSpan OpenedTimeout = TimeSpan.FromSeconds(30);

    private CommsServerClient client;
    private readonly ManualResetEventSlim openedEvent = new ManualResetEventSlim();
    private readonly object syncRoot = new object();
    private bool isClientOpened;
    private bool isSessionCreated;
    private Exception unhandledException;
    private bool disposed;
    private CommsServerConnectionState connectionState;

    public string Endpoint { get; }

    public string Protocol { get; }

    public string ServiceRoot { get; }

    public CommsServerConnectionState ConnectionState
    {
      get
      {
        lock (syncRoot)
        {
          return connectionState;
        }
      }
    }

    public bool IsClientOpened
    {
      get
      {
        lock (syncRoot)
        {
          return isClientOpened;
        }
      }
    }

    public bool IsSessionCreated
    {
      get
      {
        lock (syncRoot)
        {
          return isSessionCreated;
        }
      }
    }

    public Exception UnhandledException
    {
      get
      {
        lock (syncRoot)
        {
          return unhandledException;
        }
      }
    }

    public event MessagesEventHandler MessagesReceived;
    public event ConnectionStateChangedHandler ConnectionStateChanged;

    public CommsServerWebSocketProtocol(CommsServerProtocolConfiguration configuration)
    {
      var userId = DataServices.Resolve<IUserService>().SystemUserSessionKey;

      client = new CommsServerClient(new CommsServerClientConfiguration
      {
        Endpoint = new Endpoint(configuration.Endpoint.Url, configuration.Endpoint.Protocol, configuration.Endpoint.AuthenticationPayload),
        Storage = new SqlServerStorage(userId),
        Logger = new Log4NetLogger(),
        TransportFactory = new WebSocketTransportFactory { EnableCompression = true }
      });

      Endpoint = configuration.Endpoint.Url;
      Protocol = configuration.Endpoint.Protocol;
      ServiceRoot = configuration.BaseUrl;

      client.Receiver.Received += Receiver_Received;
      client.SessionCreated += SetSession;
      client.Opened += SetOpened;
      client.Closed += Client_Closed;
    }

    private void Client_Closed(object sender, CloseEventArgs e)
    {
      lock (syncRoot)
      {
        unhandledException = e.Exception;
        connectionState = CommsServerConnectionState.NotConnected;
      }

      ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedArgs(connectionState));
    }

    public void Open()
    {
      client.Open();
    }

    private void SetSession(object sender, EventArgs e)
    {
      lock (syncRoot)
      {
        isSessionCreated = true;
      }
    }

    private void SetOpened(object sender, EventArgs e)
    {
      lock (syncRoot)
      {
        connectionState = CommsServerConnectionState.Connected;
        isClientOpened = true;
        unhandledException = null;
      }

      ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedArgs(connectionState));
      openedEvent.Set();
    }

    private void Receiver_Received(object sender, MessageEventArgs e)
    {
      OnMessagesReceived(new MessagesEventArgs(new List<Message> { e.Message }, default(CancellationToken)));
    }

    public void Publish(params Message[] messages)
    {
      // We're not waiting for the connection to be opened here
      // because the web socket client does local queueing.

      client.Sender.Publish(messages);
    }

    public void Publish(IEnumerable<Message> messages)
    {
      // We're not waiting for the connection to be opened here
      // because the web socket client does local queueing.

      client.Sender.Publish(messages);
    }

    private void EnsureOpen()
    {
      if (!openedEvent.Wait(OpenedTimeout))
        throw new InvalidOperationException("Comms Server is not available");
    }

    public void DownloadBlob(string id, Stream stream, bool compress = true)
    {
      EnsureOpen();
      client.DownloadBlobAsync(id, stream, compress).Wait();
    }

    public string UploadBlob(Stream stream, bool compress = true)
    {
      EnsureOpen();

      try
      {
        return Task.Run(() => client.UploadBlobAsync(stream, compress)).Result;
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to upload blob", ex);
        throw;
      }
    }

    public void DeleteBlob(string id)
    {
      EnsureOpen();
      client.DeleteBlobAsync(id).Wait();
    }

    protected virtual void OnMessagesReceived(MessagesEventArgs e)
    {
      MessagesReceived?.Invoke(this, e);
    }

    public void Dispose()
  {
    if (!disposed)
    {
      if (client != null)
      {
        client.Dispose();
        client = null;
      }

      disposed = true;
    }
  }
}
 }
