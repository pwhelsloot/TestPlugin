using System;
using System.Collections.Generic;
using AMCS.Data.Server.Heartbeat;

namespace AMCS.PlatformFramework.Server.Heartbeat
{
  public class ConnectionRegistry : IConnectionRegistry
  {
    public TimeSpan MaxHeartbeatLatency => TimeSpan.FromMinutes(7.5);

    public IList<ICommServerConnection> GetCommServerConnections()
    {
      return new List<ICommServerConnection>();
    }
  }
}