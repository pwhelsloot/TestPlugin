using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Abstractions.CommsServer;

namespace AMCS.ApiService.CommsServer
{
  internal interface ICommsServerProtocolFactoryCaller
  {
    ICommsServerProtocol CreateProtocol(ICommsServerClient client, string key);

    void DestroyProtocol(ICommsServerProtocol protocol, string key);

    List<(string Key, CommsServerEndpoint Endpoint, string TenantId)> GetStartupProtocols();
  }
}