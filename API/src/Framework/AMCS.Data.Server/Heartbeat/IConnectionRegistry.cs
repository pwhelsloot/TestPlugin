namespace AMCS.Data.Server.Heartbeat
{
  using System;
  using System.Collections.Generic;

  public interface IConnectionRegistry
  {
    TimeSpan MaxHeartbeatLatency { get; }
    IList<ICommServerConnection> GetCommServerConnections();
  }
}
