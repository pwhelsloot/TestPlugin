using System;
using System.Collections.Generic;
using System.Linq;
using AMCS.ApiService.Abstractions.CommsServer;
using AMCS.Data;
using AMCS.Data.Configuration;
using AMCS.Data.Server.Broadcast;

namespace AMCS.ApiService.CommsServer
{
  public class CommsServerProtocolService : ICommsServerProtocolService, IDelayedStartup
  {
    private readonly CommsServerProtocolManager commsServerProtocolManager;
    private readonly IBroadcastService broadcastService;
    private readonly TypeManager protocolTypes;

    public CommsServerProtocolService(CommsServerProtocolManager commsServerProtocolManager, IBroadcastService broadcastService, TypeManager protocolTypes)
    {
      this.commsServerProtocolManager = commsServerProtocolManager;
      this.broadcastService = broadcastService;
      this.protocolTypes = protocolTypes;
    }

    public void Start()
    {
      this.LoadInitialProtocols();
    }

    private void LoadInitialProtocols()
    {
      var seen = new HashSet<Type>();

      foreach (var protocolType in protocolTypes.GetTypes().Where(p => typeof(ICommsServerProtocol).IsAssignableFromGeneric(p)))
      {
        var factoryCaller = GetCommsServerProtocolFactoryCaller(protocolType);

        if (factoryCaller == null)
          continue;

        // If the factory has an interface, and it's also registered as
        // that interface, we'll see the factory twice. Deduplicate here.
        if (!seen.Add(factoryCaller.GetType()))
          continue;

        var protocolConfigurations = factoryCaller.GetStartupProtocols();
        foreach (var protocolConfiguration in protocolConfigurations)
        {
          commsServerProtocolManager.UpdateProtocol(new ProtocolChanged(
            protocolConfiguration.Endpoint, 
            protocolConfiguration.Key, 
            protocolConfiguration.TenantId, 
            protocolType.FullName)
          );
        }
      }
    }

    private static ICommsServerProtocolFactoryCaller GetCommsServerProtocolFactoryCaller(Type protocolType)
    {
      if (!DataServices.TryResolve(typeof(ICommsServerProtocolFactory<>).MakeGenericType(protocolType), out var resolvedFactory))
        return null;

      return (ICommsServerProtocolFactoryCaller)Activator.CreateInstance(typeof(CommsServerProtocolFactoryCaller<>).MakeGenericType(protocolType), resolvedFactory);
    }

    public void UpdateProtocol(CommsServerEndpoint endpoint, string key, string tenantId, string type)
    {
      broadcastService.Broadcast(new ProtocolChanged(endpoint, key, tenantId, type));
    }

    public void DeleteProtocol(string key, string type)
    {
      broadcastService.Broadcast(new ProtocolDeleted(key, type));
    }
  }
}