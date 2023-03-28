using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Abstractions.CommsServer;

namespace AMCS.ApiService.CommsServer
{
  public class ProtocolChanged
  {
    public CommsServerEndpoint Endpoint { get; }

    /// <summary>
    /// How the protocol is identified. This could for example be a combination of the protocol name and tenant identifier.
    /// can be null for scenarios like Scale : ERP 1:1 mapping
    /// in SDM would be tenant identifier
    /// combine this key with CommsServerProtocolAttribute.Protocol
    /// </summary>
    public string Key { get; }

    public string TenantId { get; }

    /// <summary>
    /// The type of protocol that should be created e.g: TransportDispatchProtocol.FullName
    /// </summary>
    public string Type { get; }

    public ProtocolChanged(CommsServerEndpoint endpoint, string key, string tenantId, string type)
    {
      this.Endpoint = endpoint;
      this.Key = key;
      this.TenantId = tenantId;
      this.Type = type;
    }
  }
}
