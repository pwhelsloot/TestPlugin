using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Abstractions.CommsServer;

namespace AMCS.ApiService.CommsServer
{
  public interface ICommsServerProtocolService
  {
    void UpdateProtocol(CommsServerEndpoint endpoint, string key, string tenantId, string type);

    void DeleteProtocol(string key, string type);
  }
}
