using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Abstractions.CommsServer;
using AMCS.ApiService.CommsServer;
using AMCS.Data;
using AMCS.PlatformFramework.Server.Configuration;
using AMCS.PlatformFramework.Server.Protocols.Server;
using Newtonsoft.Json;

namespace AMCS.PlatformFramework.Server.CommsServer
{
  // public class PlatformFrameworkProtocolFactory : ICommsServerProtocolFactory<T>
  public class PlatformFrameworkProtocolFactory : ICommsServerProtocolFactory<ServerProtocol>, IPlatformFrameworkProtocolManager
  {
    private readonly ConcurrentDictionary<string, ServerProtocol> protocols;
    private readonly ICommsServerConfiguration commsServerConfiguration = DataServices.Resolve<IPlatformFrameworkConfiguration>().CommsServer;

    public PlatformFrameworkProtocolFactory()
    {
      this.protocols = new ConcurrentDictionary<string, ServerProtocol>();
    }

    public ServerProtocol CreateProtocol(ICommsServerClient client, string key)
    {
      var protocol = new ServerProtocol(client);

      // add to dictionary
      if (!protocols.TryAdd(key, protocol))
        throw new InvalidOperationException("Protocol already registered");

      // do any other app specific logic you want here.

      return protocol;
    }

    public void DestroyProtocol(ServerProtocol protocol, string key)
    {
      // remove from dictionary
      if (protocols.TryRemove(key, out var platformFrameworkProtocol))
      {
        // do stuff with protocol
        // in theory platformFrameworkProtocol == protocol
        // but we provide it here in case the factory/manager decides not to manage it
        // but still needs to do some clean up etc.
        platformFrameworkProtocol.Dispose();
      }
    }

    public List<(string Key, CommsServerEndpoint Endpoint, string TenantId)> GetStartupProtocols()
    {
      // load from db or whatever
      // don't add to dictionary

      var startupProtocols = new List<(string Key, CommsServerEndpoint Endpoint, string TenantId)>();

      var tenantId = DataServices.Resolve<IPlatformFrameworkConfiguration>().TenantId;
      var endpoint = CreateCommsServerEndpoint(tenantId);
      if (endpoint != null)
        startupProtocols.Add((tenantId, endpoint, tenantId));

      return startupProtocols;
    }

    public ServerProtocol FindByTenant(string tenantId)
    {
      // get returns an error, find returns null
      if (protocols.TryGetValue(tenantId, out var protocol))
        return protocol;

      return null;  
    }

    public void AddProtocolForTenant(string tenantId)
    {
      var endpoint = CreateCommsServerEndpoint(tenantId);
      DataServices.Resolve<ICommsServerProtocolService>().UpdateProtocol(endpoint, tenantId, tenantId, typeof(ServerProtocol).FullName);
    }

    public void RemoveProtocolForTenant(string tenantId)
    {
      DataServices.Resolve<ICommsServerProtocolService>().DeleteProtocol(tenantId, typeof(ServerProtocol).FullName);
    }

    private CommsServerEndpoint CreateCommsServerEndpoint(string tenantId)
    {
      string url = commsServerConfiguration.Server?.Url;
      if (string.IsNullOrEmpty(url))
        return null;

      var protocol = CommsServerProtocol.Default;

      string protocolName = commsServerConfiguration.Server.Protocol;
      if (!string.IsNullOrEmpty(protocolName))
        protocol = (CommsServerProtocol)Enum.Parse(typeof(CommsServerProtocol), protocolName, true);
      
      var authenticationPayload = new AuthenticationRequest
      {
        PrivateKey = commsServerConfiguration.Server.AuthKey,
        Instance = tenantId
      };

      return new CommsServerEndpoint(
        url,
        JsonConvert.SerializeObject(authenticationPayload),
        protocol,
        !string.IsNullOrEmpty(commsServerConfiguration.Server.AzureServiceBusConnectionStringName) ? commsServerConfiguration.Server.AzureServiceBusConnectionStringName : null);
    }
  }
}