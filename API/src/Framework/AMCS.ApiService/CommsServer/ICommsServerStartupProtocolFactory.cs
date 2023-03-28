using System.Collections.Generic;
using AMCS.ApiService.Abstractions.CommsServer;

namespace AMCS.ApiService.CommsServer
{
  public interface ICommsServerStartupProtocolFactory<T>
  {
    List<(string Key, CommsServerEndpoint Endpoint)> GetStartupEndpoints();
  }
}