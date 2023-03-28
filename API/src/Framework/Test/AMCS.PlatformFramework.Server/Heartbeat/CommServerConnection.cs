using System;
using AMCS.ApiService.Abstractions.CommsServer;
using AMCS.Data.Entity.Heartbeat;
using AMCS.Data.Server.Heartbeat;

namespace AMCS.PlatformFramework.Server.Heartbeat
{
  public class CommServerConnection : ICommServerConnection
  {
    public string ProtocolName { get; }
    public string InstanceName { get; }
    public ICommsServerClient CommsServerClient { get; }
    public HeartbeatConnectionStatus ConnectionStatus { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    public int? HeartbeatLatencyInSeconds { get; set; }
    
    public CommServerConnection(string protocolName, string instanceName, ICommsServerClient commsServerClient)
    {
      ProtocolName = protocolName;
      InstanceName = instanceName;
      CommsServerClient = commsServerClient;
    }

    public void SendHeartbeat(string payload)
    {
    }
  }
}