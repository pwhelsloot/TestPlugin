namespace AMCS.PlatformFramework.IntegrationTests.Framework
{
  using System;
  using System.Linq;
  using System.Net;
  using System.Threading.Tasks;
  using AMCS.Data;
  using AMCS.Data.Server.Broadcast;
  using AMCS.WebDiagnostics;
  using NUnit.Framework;
  
  [TestFixture]
  public class HeartbeatLatencyFixture : TestBase
  {
    [Test]
    public async Task GivenHeartbeat_WhenGettingDiagnostics_ThenDiagnosticResultSuccess()
    {
      DataServices.Resolve<IBroadcastService>().Broadcast(new BroadcastHeartbeatService.BroadcastHeartbeat());

      await Task.Delay(TimeSpan.FromSeconds(1));
      
      var diagnostics = DataServices.Resolve<IBroadcastHeartbeatService>()
        .GetDiagnostics()
        .ToList();
      
      Assert.AreEqual(1, diagnostics.Count);
      Assert.AreEqual(typeof(DiagnosticResult.Success), diagnostics.First().GetType());
      Assert.AreEqual($"Broadcast Service Heartbeat - {Dns.GetHostName()}", diagnostics.First().Title);
    }
    
    [Test]
    public async Task GivenHeartbeat_WhenGettingDiagnostics_ThenDiagnosticResultFailure()
    {
      DataServices.Resolve<IBroadcastService>().Broadcast(new BroadcastHeartbeatService.BroadcastHeartbeat
      {
        HostName = Dns.GetHostName(),
        Timestamp = DateTime.UtcNow.AddHours(-1)
      });

      await Task.Delay(TimeSpan.FromSeconds(4));
      
      var diagnostics = DataServices.Resolve<IBroadcastHeartbeatService>()
        .GetDiagnostics()
        .ToList();
      
      Assert.AreEqual(1, diagnostics.Count);
      Assert.AreEqual(typeof(DiagnosticResult.Failure), diagnostics.First().GetType());
      Assert.AreEqual($"Broadcast Service Heartbeat - {Dns.GetHostName()} - Delayed", diagnostics.First().Title);
    }
  }
}