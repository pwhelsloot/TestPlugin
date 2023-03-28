using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.PlatformFramework.Server.Protocols.Server;

namespace AMCS.PlatformFramework.Server.CommsServer
{
  public interface IPlatformFrameworkProtocolManager
  {
    ServerProtocol FindByTenant(string tenantId);

    void AddProtocolForTenant(string tenantId);

    void RemoveProtocolForTenant(string tenantId);
  }
}