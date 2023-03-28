using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.CommsServer
{
  public class ProtocolDeleted
  {

    /// <summary>
    /// How the protocol is identified. This could for example be a combination of the protocol name and tenant identifier.
    /// can be null for scenarios like Scale : ERP 1:1 mapping
    /// in SDM would be tenant identifier
    /// combine this key with CommsServerProtocolAttribute.Protocol
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// The type of protocol that should be deleted e.g: TransportDispatchProtocol.FullName
    /// </summary>
    public string Type { get; }

    public ProtocolDeleted(string key, string type)
    {
      this.Key = key;
      this.Type = type;
    }
  }
}
