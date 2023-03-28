namespace AMCS.ApiService.CommsServer
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Text;
  using System.Threading;
  using AMCS.ApiService.Abstractions.CommsServer;
  using AMCS.CommsServer.PushClient;
  using AMCS.CommsServer.PushClient.Transport.AzureServiceBus;
  using AMCS.CommsServer.Serialization;
  using AzureServiceBusSupport.RetryUtils;
  using log4net;

  internal class CommsServerPushProtocol : ICommsServerClient
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(CommsServerProtocol));
    private static readonly BackoffProfile BackoffProfile = new BackoffProfile(TimeSpan.FromSeconds(1), 5, TimeSpan.FromMinutes(1));
    private static readonly TimeSpan OpenedTimeout = TimeSpan.FromSeconds(30);

    private CommsServerPushClient client;
    private readonly ManualResetEventSlim openedEvent = new ManualResetEventSlim();
    private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
    private readonly object syncRoot = new object();
    private bool isClientOpened;
    private bool isSessionCreated;
    private Exception unhandledException;
    private bool disposed;
    private CommsServerConnectionState connectionState;

    public string Key { get; }

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
          if (unhandledException != null)
            return unhandledException;
        }

        return client?.UnhandledException;
      }
    }

    public event MessagesEventHandler MessagesReceived;
    public event ConnectionStateChangedHandler ConnectionStateChanged;

    public CommsServerPushProtocol(CommsServerProtocolConfiguration configuration)
    {
      if (configuration.Key == null)
        throw new ArgumentException("Key is required");

      Key = configuration.Key;
      Endpoint = configuration.Endpoint.Url;
      Protocol = configuration.Endpoint.Protocol;
      ServiceRoot = configuration.BaseUrl;

      var connectionString = configuration.AzureServiceBusConnectionString?.GetConnectionString();
      
      try
      {
        UnRegisterOldClient(configuration, connectionString);
      }
      catch (ArgumentNullException ex)
      {
        Logger.Info("Could not unregister old client", ex);
      }

      RegisterNewClient(configuration, connectionString);
    }

    private void UnRegisterOldClient(CommsServerProtocolConfiguration configuration, string connectionString)
    {
      var oldCallbackStringBuilder = new StringBuilder()
        .Append(configuration.BaseUrl.TrimEnd('/'))
        .Append("/cs-push/2019-01?protocol=")
        .Append(Uri.EscapeDataString(configuration.Endpoint.Protocol))
        .Append("&key=")
        .Append(Uri.EscapeDataString(configuration.Key));

      if (!string.IsNullOrEmpty(configuration.Instance))
      {
        oldCallbackStringBuilder
          .Append("&instance=")
          .Append(Uri.EscapeDataString(configuration.Instance));
      }

      var oldCallback = oldCallbackStringBuilder.ToString();

      var oldClient = CreateClient(configuration, connectionString, oldCallback);

      oldClient.DisableCallback();
    }

    private void RegisterNewClient(CommsServerProtocolConfiguration configuration, string connectionString)
    {
      var sb = new StringBuilder()
        .Append("?tenantId=")
        .Append(Uri.EscapeDataString(configuration.TenantId))
        .Append("&protocol=")
        .Append(Uri.EscapeDataString(configuration.Endpoint.Protocol));

      if (!string.IsNullOrEmpty(configuration.Instance))
      {
        sb
          .Append("&instance=")
          .Append(Uri.EscapeDataString(configuration.Instance));
      }

      var callback = sb.ToString();

      client = CreateClient(configuration, connectionString, callback);

      client.Received += Client_Received;
      client.ConnectionStateChanged += Client_ConnectionState_Changed;
    }

    private CommsServerPushClient CreateClient(CommsServerProtocolConfiguration configuration, string connectionString, string callback)
    {
      var pushClientConfiguration = new CommsServerPushClientConfiguration
      {
        Endpoint = configuration.Endpoint,
        EnableCompression = configuration.EnableCompression,
        Callback = callback
      };

      if (!string.IsNullOrEmpty(connectionString))
      {
        pushClientConfiguration.AlternateTransportFactories.Add(new AzureServiceBusPushClientTransportFactory
        {
          ConnectionString = connectionString
        });
      }

      return new CommsServerPushClient(pushClientConfiguration);
    }


    private void Client_Received(object sender, AMCS.CommsServer.PushClient.MessageEventArgs e)
    {
      OnMessagesReceived(new MessagesEventArgs(new List<Message> { e.Message }, e.CancellationToken));
    }

    private void Client_ConnectionState_Changed(object sender, ConnectionStateEventArgs e)
    {
      
      switch (e.ConnectionState)
      {
        case PushClientConnectionState.Connected:
          connectionState = CommsServerConnectionState.Connected;
          break;
        case PushClientConnectionState.NotConnected:
          connectionState = CommsServerConnectionState.NotConnected;
          break;
        case PushClientConnectionState.NotSupported:
          connectionState = CommsServerConnectionState.NotSupported;
          break;
        default:
          throw new InvalidOperationException("Unknown connection state passed");
      }
      
      ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedArgs(connectionState));
    }

    internal void ReceiveMessages(Stream stream)
    {
      var messages = client.Parse(stream);

      OnMessagesReceived(new MessagesEventArgs(messages, default(CancellationToken)));
    }

    public void Open()
    {
      ThreadPool.QueueUserWorkItem(_ => StartOpen());
    }

    private void StartOpen()
    {
      try
      {
        var result = RetryPolicy
          .Handle<Exception>(ex => Logger.Debug("Failed to connect"))
          .Backoff(BackoffProfile)
          .Retry(DoOpen, tokenSource.Token);

        if (result)
        {
          lock (syncRoot)
          {
            isSessionCreated = true;
            isClientOpened = true;
          }

          openedEvent.Set();
        }
        else
        {
          Logger.Error("Failed to open Comms Server protocol");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to open Comms Server protocol", ex);
      }

      void DoOpen()
      {
        try
        {
          client.Open();

          lock (syncRoot)
          {
            unhandledException = null;
          }
        }
        catch (Exception ex)
        {
          lock (syncRoot)
          {
            unhandledException = ex;
          }
          throw;
        }
      }
    }

    private void EnsureOpen()
    {
      if (!openedEvent.Wait(OpenedTimeout))
        throw new InvalidOperationException("Comms Server is not available");
    }

    public void Publish(params Message[] messages)
    {
      EnsureOpen();

      client.Publish(messages);
    }

    public void Publish(IEnumerable<Message> messages)
    {
      EnsureOpen();

      client.Publish(messages);
    }

    public void DownloadBlob(string id, Stream stream, bool compress = true)
    {
      EnsureOpen();

      client.DownloadBlob(id, stream, compress);
    }

    public string UploadBlob(Stream stream, bool compress = true)
    {
      EnsureOpen();

      return client.UploadBlob(stream, compress);
    }

    public void DeleteBlob(string id)
    {
      EnsureOpen();

      client.DeleteBlob(id);
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

        tokenSource.Cancel();
        disposed = true;
      }
    }
  }
}
