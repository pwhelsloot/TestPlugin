using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Abstractions.CommsServer;

namespace AMCS.ApiService.CommsServer
{
  internal class CommsServerProtocolFactoryCaller<T> : ICommsServerProtocolFactoryCaller where T : ICommsServerProtocol
  {
    private readonly ICommsServerProtocolFactory<T> factory;

    public CommsServerProtocolFactoryCaller(ICommsServerProtocolFactory<T> factory)
    {
      this.factory = factory;
    }

    public ICommsServerProtocol CreateProtocol(ICommsServerClient client, string key)
    {
      return factory.CreateProtocol(client, key);
    }

    public void DestroyProtocol(ICommsServerProtocol protocol, string key)
    {
      factory.DestroyProtocol((T)protocol, key);
    }

    public List<(string Key, CommsServerEndpoint Endpoint, string TenantId)> GetStartupProtocols()
    {
      return factory.GetStartupProtocols();
    }
  }
}