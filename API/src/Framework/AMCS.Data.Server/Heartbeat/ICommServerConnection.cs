namespace AMCS.Data.Server.Heartbeat
{
  using System;
  using AMCS.ApiService.Abstractions.CommsServer;
  using AMCS.Data.Entity.Heartbeat;

  public interface ICommServerConnection
    {
        string ProtocolName { get; }
        string InstanceName { get; }
        HeartbeatConnectionStatus ConnectionStatus { get; set; }
        DateTime TimeStamp { get; set; }
        int? HeartbeatLatencyInSeconds { get; set; }
        ICommsServerClient CommsServerClient { get; }
        void SendHeartbeat(string payload);
    }
}
