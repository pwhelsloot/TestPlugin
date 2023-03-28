using System;
using System.Collections.Generic;
using System.Net;
using AMCS.WebDiagnostics;
using System.Threading;
using log4net;

namespace AMCS.Data.Server.Broadcast
{
  public class BroadcastHeartbeatService : IBroadcastHeartbeatService
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(BroadcastHeartbeatService));

    private readonly IBroadcastService broadcastService;
    private readonly TimeSpan heartbeatTimerTimeout;
    private readonly TimeSpan maxHeartbeatLatency;
    private readonly TimeSpan gracePeriod;
    private readonly object syncRoot = new object();
    
    private Timer heartbeatTimer;
    private DateTime lastHeartbeatReceived;
    private TimeSpan lastHeartbeatLatency;
    
    public BroadcastHeartbeatService(
      IBroadcastService broadcastService, TimeSpan heartbeatTimerTimeout,TimeSpan maxHeartbeatLatency)
    {
      this.broadcastService = broadcastService;
      this.heartbeatTimerTimeout = heartbeatTimerTimeout;
      this.maxHeartbeatLatency = maxHeartbeatLatency;
      gracePeriod = TimeSpan.FromMinutes(heartbeatTimerTimeout.TotalMinutes * 2);
    }

    public void Start()
    {
      if (heartbeatTimer != null)
        return;

      heartbeatTimer = new Timer(state => BroadCast(), null, heartbeatTimerTimeout, heartbeatTimerTimeout);

      broadcastService.On<BroadcastHeartbeat>(heartbeat =>
      {
        if (!string.Equals(Dns.GetHostName(), heartbeat.HostName))
          return;

        lock (syncRoot)
        {
          lastHeartbeatReceived = DateTime.UtcNow;
          lastHeartbeatLatency = DateTime.UtcNow - heartbeat.Timestamp;
        }
      });

      BroadCast();

      if (DataServices.TryResolve<IDiagnosticsManager>(out var diagnosticsManager))
        diagnosticsManager.Register(GetDiagnostics);
      
      void BroadCast()
      {
        try
        {
          broadcastService.Broadcast(new BroadcastHeartbeat());
        }
        catch (Exception ex)
        {
          Logger.Error("There was an error executing the broadcast service heartbeat timed job.", ex);
        }
      }
    }

    public IEnumerable<DiagnosticResult> GetDiagnostics()
    {
      if (maxHeartbeatLatency.TotalMilliseconds > 0 && lastHeartbeatLatency > maxHeartbeatLatency)
      {
        yield return new DiagnosticResult.Failure($"Broadcast Service Heartbeat - {Dns.GetHostName()} - Delayed",
          $"Last Heartbeat Received {lastHeartbeatReceived:g} - Latency threshold {lastHeartbeatLatency} was more than the maximum allowed of {maxHeartbeatLatency}.");

        yield break;
      }
      
      if (DateTime.UtcNow - lastHeartbeatReceived > gracePeriod)
      {
        yield return new DiagnosticResult.Failure($"Broadcast Service Heartbeat - {Dns.GetHostName()} - Stale",
          $"Last Heartbeat Received {lastHeartbeatReceived:g} is over the allowed interval of {gracePeriod} - Latency {lastHeartbeatLatency}.");

        yield break;
      }

      yield return new DiagnosticResult.Success($"Broadcast Service Heartbeat - {Dns.GetHostName()}",
        $"Last Heartbeat Received {lastHeartbeatReceived:g} - Latency {lastHeartbeatLatency}.");
    }
    
    public class BroadcastHeartbeat
    {
      public string HostName { get; set; } = Dns.GetHostName();
      public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
  }
}