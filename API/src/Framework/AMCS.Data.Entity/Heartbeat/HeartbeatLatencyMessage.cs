using System;
using Newtonsoft.Json;

namespace AMCS.Data.Entity.Heartbeat
{
  public class HeartbeatLatencyMessage
  {
    public DateTime Created { get; set; } = DateTime.UtcNow;

    public static string CreatePayload()
    {
      return new HeartbeatLatencyMessage().ToString();
    }

    public static HeartbeatLatencyMessage ReadPayload(string payload)
    {
      if (string.IsNullOrWhiteSpace(payload))
        return null;

      var heartbeat = JsonConvert.DeserializeObject<HeartbeatLatencyMessage>(payload);
      return heartbeat;
    }
    
    public override string ToString()
    {
      return JsonConvert.SerializeObject(this);
    }
  }
}