using System;

namespace AMCS.Data.Entity.Heartbeat
{
    [EntityTable("HeartbeatConnection", "HeartbeatConnectionId", SchemaName = "comms")]
    [Serializable]
    public class HeartbeatConnection : EntityObject
    {
        [EntityMember]
        public int? HeartbeatConnectionId { get; set; }

        [EntityMember]
        public string ProtocolName { get; set; }
        
        [EntityMember]
        public string InstanceName { get; set; }
        
        [EntityMember]
        public HeartbeatConnectionStatus Status { get; set; }
        
        [EntityMember]
        public DateTime Timestamp { get; set; }
        
        [EntityMember]
        public int? HeartbeatLatencyInSeconds { get; set; }
        
        public override int? GetId() => HeartbeatConnectionId;
        
        public string PrintStatusTimeoutAndLatency()
        {
          var latency = HeartbeatLatencyInSeconds == null
            ? "N/A"
            : $"{TimeSpan.FromSeconds(HeartbeatLatencyInSeconds.Value):g}";

          return $"Heartbeat Connection Status: {Status}, Last Activity Timestamp: {Timestamp:g}, Latency: {latency}";
        }
    }
}