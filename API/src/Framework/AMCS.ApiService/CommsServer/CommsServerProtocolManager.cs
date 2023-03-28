 using System;
 using System.Collections.Concurrent;
 using System.Collections.Generic;
 using System.Reflection;
 using AMCS.ApiService.Abstractions.CommsServer;
 using AMCS.CommsServer.PushClient;
 using AMCS.Data;
 using AMCS.Data.Configuration;
 using AMCS.Data.Server.Broadcast;
 using AMCS.Data.Server.Configuration;
 using AMCS.WebDiagnostics;
 
 namespace AMCS.ApiService.CommsServer
{
  public class CommsServerProtocolManager : IDisposable, ICommsServerProtocolManager, IDelayedStartup
  {
    private readonly TypeManager protocolTypes;
    private readonly ICommsServerProtocolsConfiguration configuration;
    private readonly ConcurrentDictionary<(Type type, string key), ICommsServerProtocol> protocols = new ConcurrentDictionary<(Type, string), ICommsServerProtocol>();
    private readonly bool allowAlternateTransport;
    private readonly IBroadcastService broadcastService;
    private bool disposed;
    private readonly string baseUrl;

    public CommsServerProtocolManager(TypeManager protocolTypes, ICommsServerProtocolsConfiguration configuration,
      IServiceRootResolver serviceRoots, IDiagnosticsManager diagnosticsManager, bool allowAlternateTransport, IBroadcastService broadcastService)
    {
      this.protocolTypes = protocolTypes;
      this.configuration = configuration;
      this.allowAlternateTransport = allowAlternateTransport;
      this.broadcastService = broadcastService;
      this.baseUrl = serviceRoots.GetServiceRoot(configuration.ServiceRoot);

      diagnosticsManager.Register(GetDiagnostics);
    }

    public void Start()
    {
      broadcastService.On<ProtocolChanged>(this.UpdateProtocol);

      broadcastService.On<ProtocolDeleted>(this.DeleteProtocol);
    }

    internal void UpdateProtocol(ProtocolChanged protocolChanged)
    {
      var protocolType = protocolTypes.GetType(protocolChanged.Type);

      var factoryCaller = GetCommsServerProtocolFactoryCaller(protocolType);

      // This should be TO-BE state so if there is already a protocol registered, delete it and add it again.
      
      DoDeleteProtocol((protocolType, protocolChanged.Key), factoryCaller);

      var protocolName = GetProtocolName(protocolType);

      var protocolConfiguration = new CommsServerProtocolConfiguration
      {
        BaseUrl = baseUrl,
        Endpoint = new Endpoint(protocolChanged.Endpoint.Url, protocolName, protocolChanged.Endpoint.AuthenticationPayload),
        EnableCompression = true,
        Key = configuration.Key,
        AzureServiceBusConnectionString = allowAlternateTransport
          ? DataServices.Resolve<IConnectionStringResolver>().GetConnectionString(protocolChanged.Endpoint.AzureServiceBusConnectionStringName)
          : null,
        Instance = protocolChanged.Key,
        TenantId = protocolChanged.TenantId,
      };

      if (protocolChanged.Endpoint.Protocol == CommsServerProtocol.WebSocket)
      {
        var client = new CommsServerWebSocketProtocol(protocolConfiguration);

        var protocol = factoryCaller.CreateProtocol(client, protocolChanged.Key);

        if (!protocols.TryAdd((protocolType, protocolChanged.Key), protocol))
          throw new InvalidOperationException($"Protocol already registered for: { (protocolType.Name, protocolChanged.Key) }");

        client.MessagesReceived += (s, e) =>
        {
          protocol.ProcessMessages(e.Messages, e.CancellationToken);
        };

        client.ConnectionStateChanged += (s, e) => { protocol.StateChanged(e.ConnectionState); };

        client.Open();
      }
      else
      {
        if (protocolConfiguration.AzureServiceBusConnectionString == null)
          throw new InvalidOperationException("Alternate Transport must be enabled when using Push");

        var client = new CommsServerPushProtocol(protocolConfiguration);
        
        var protocol = factoryCaller.CreateProtocol(client, protocolChanged.Key);
        
        if (!protocols.TryAdd((protocolType, protocolChanged.Key), protocol))
          throw new InvalidOperationException($"Protocol already registered for: { (protocolType.Name, protocolChanged.Key) }");

        client.MessagesReceived += (s, e) =>
        {
          protocol.ProcessMessages(e.Messages, e.CancellationToken);
        };

        client.ConnectionStateChanged += (s, e) => { protocol.StateChanged(e.ConnectionState); };

        client.Open();
      }
    }

    internal void DeleteProtocol(ProtocolDeleted protocolDeleted)
    {
      var protocolType = protocolTypes.GetType(protocolDeleted.Type);

      var factoryCaller = GetCommsServerProtocolFactoryCaller(protocolType);

      DoDeleteProtocol((protocolType, protocolDeleted.Key), factoryCaller);
    }

    private void DoDeleteProtocol((Type type, string key) key, ICommsServerProtocolFactoryCaller factoryCaller)
    {
      if (!protocols.TryRemove(key, out var protocol))
        return;

      factoryCaller.DestroyProtocol(protocol, key.key);

      if (protocol is IDisposable disposable)
        disposable.Dispose();
    }

    private static ICommsServerProtocolFactoryCaller GetCommsServerProtocolFactoryCaller(Type protocolType)
    {
      if (!DataServices.TryResolve(typeof(ICommsServerProtocolFactory<>).MakeGenericType(protocolType), out var resolvedFactory))
        throw new InvalidOperationException($"Factory not found for protocol type { protocolType.Name }");

      return (ICommsServerProtocolFactoryCaller)Activator.CreateInstance(typeof(CommsServerProtocolFactoryCaller<>).MakeGenericType(protocolType), resolvedFactory);
    }

    private static string GetProtocolName(Type type)
    {
      var attribute = type.GetCustomAttribute<CommsServerProtocolAttribute>();
      if (attribute == null)
        throw new InvalidOperationException($"Protocol type {type} does not specify a CommsServerProtocol attribute");

      return attribute.Protocol;
    }

    private IEnumerable<DiagnosticResult> GetDiagnostics()
    {
      foreach (var protocol in protocols)
      {
        var sessionCreated = protocol.Value.Client.IsSessionCreated;
        var clientOpened = protocol.Value.Client.IsClientOpened;
        var unhandledException = protocol.Value.Client.UnhandledException;

        if (sessionCreated && clientOpened && unhandledException == null)
        {
          yield return new DiagnosticResult.Success($"Comms Server Protocol - {protocol.Key}");
        }
        else
        {
          var message = $"Session created: {sessionCreated}, client opened: {clientOpened}";
          if (unhandledException != null)
            message += $", unhandled exception: {unhandledException.Message}";

          yield return new DiagnosticResult.Failure($"Comms Server Protocol - {protocol.Key}", message, unhandledException);
        }
      }
    }

    public void Dispose()
    {
      if (!disposed)
      {
        foreach (var protocol in protocols.Values)
        {
          if (protocol is IDisposable disposable)
            disposable.Dispose();
        }
        protocols.Clear();

        disposed = true;
      }
    }
  }
}
