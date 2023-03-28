using System.Collections.Generic;
using AMCS.ApiService.Abstractions.CommsServer;

namespace AMCS.ApiService.CommsServer
{ 
  // This is a generic interface and will exist in the Framework
  public interface ICommsServerProtocolFactory<T> : ICommsServerProtocolFactory
    where T : ICommsServerProtocol
  {
    T CreateProtocol(ICommsServerClient client, string key);

    void DestroyProtocol(T protocol, string key);

    List<(string Key, CommsServerEndpoint Endpoint, string TenantId)> GetStartupProtocols();
  }

  public interface ICommsServerProtocolFactory
  {
  }
}