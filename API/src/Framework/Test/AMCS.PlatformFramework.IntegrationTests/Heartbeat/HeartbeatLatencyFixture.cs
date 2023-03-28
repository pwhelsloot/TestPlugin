using System;
using System.Linq;
using AMCS.Data;
using AMCS.Data.Entity.Heartbeat;
using AMCS.Data.Server;
using AMCS.Data.Server.Heartbeat;
using NUnit.Framework;

namespace AMCS.PlatformFramework.IntegrationTests.Heartbeat
{
  [TestFixture]
  public class HeartbeatLatencyFixture : HeartbeatTestBase
  {
    [Test]
    public void GivenHeartbeat_WhenFlushingCommServerConnection_ThenLatencyIsSaved()
    {
      var withLatencyConnection = new CommServerConnectionBuilder().Build();
      var noLatencyConnection = new CommServerConnectionBuilder().Build();

      var connectionRegistry = DataServices.Resolve<IConnectionRegistry>();
      connectionRegistry.AddCommServerConnection(withLatencyConnection);
      connectionRegistry.AddCommServerConnection(noLatencyConnection);

      var payload = new HeartbeatLatencyMessage { Created = DateTime.UtcNow.AddMinutes(-1) }.ToString();
      
      var heartbeatService = DataServices.Resolve<IHeartbeatService>();
      heartbeatService.SetMessageProcessing(withLatencyConnection.ProtocolName, withLatencyConnection.InstanceName, payload);
      heartbeatService.SetMessageProcessing(noLatencyConnection.ProtocolName, noLatencyConnection.InstanceName);
      
      heartbeatService.FlushConnectionRegistry();

      WithSession(session =>
      {
        var heartbeatConnections = session
          .GetAll<HeartbeatConnection>(AdminUserId, true)
          .ToList();

        var latencyHeartbeat = heartbeatConnections.FirstOrDefault(connection =>
          connection.ProtocolName == withLatencyConnection.ProtocolName);
        
        Assert.IsNotNull(latencyHeartbeat);
        Assert.IsTrue(latencyHeartbeat.HeartbeatLatencyInSeconds > 0, "latencyHeartbeat.HeartbeatLatency > 0");
        
        Assert.AreEqual(2,
          heartbeatConnections.Count(connection => connection.Status == HeartbeatConnectionStatus.Processing));
        Assert.IsNull(heartbeatConnections
          .FirstOrDefault(connection => connection.ProtocolName == noLatencyConnection.ProtocolName)?.HeartbeatLatencyInSeconds);
      });
    }
    
    [Test]
    public void GivenMessage_WhenFlushingCommServerConnection_ThenLatencyIsNotOverwritten()
    {
      var commServerConnection = new CommServerConnectionBuilder().Build();

      var connectionRegistry = DataServices.Resolve<IConnectionRegistry>();
      connectionRegistry.AddCommServerConnection(commServerConnection);

      var payload = new HeartbeatLatencyMessage { Created = DateTime.UtcNow.AddMinutes(-1) }.ToString();
      var heartbeatService = DataServices.Resolve<IHeartbeatService>();
      
      // first heartbeat to set latency
      heartbeatService.SetMessageProcessing(commServerConnection.ProtocolName, commServerConnection.InstanceName, payload);
      heartbeatService.SetMessageProcessed(commServerConnection.ProtocolName, commServerConnection.InstanceName);
      heartbeatService.FlushConnectionRegistry();

      // non heartbeat message without latency
      heartbeatService.SetMessageProcessing(commServerConnection.ProtocolName, commServerConnection.InstanceName);
      heartbeatService.SetMessageProcessed(commServerConnection.ProtocolName, commServerConnection.InstanceName);
      heartbeatService.FlushConnectionRegistry();
      
      WithSession(session =>
      {
        var heartbeatConnections = session
          .GetAll<HeartbeatConnection>(AdminUserId, true)
          .ToList();

        var heartbeatConnection = heartbeatConnections.FirstOrDefault(connection =>
          connection.ProtocolName == commServerConnection.ProtocolName);
        
        Assert.IsNotNull(heartbeatConnection);
        Assert.IsTrue(heartbeatConnection.HeartbeatLatencyInSeconds > 0, "latencyHeartbeat.HeartbeatLatency > 0");
      });
    }
    
    [Test]
    public void GivenMessage_WhenFlushingCommServerConnection_ThenNoLatencyIsSaved()
    {
      var commServerConnection = new CommServerConnectionBuilder().Build();

      var connectionRegistry = DataServices.Resolve<IConnectionRegistry>();
      connectionRegistry.AddCommServerConnection(commServerConnection);

      var heartbeatService = DataServices.Resolve<IHeartbeatService>();
      
      heartbeatService.SetMessageProcessing(commServerConnection.ProtocolName, commServerConnection.InstanceName);
      heartbeatService.SetMessageProcessed(commServerConnection.ProtocolName, commServerConnection.InstanceName);
      heartbeatService.FlushConnectionRegistry();
      
      WithSession(session =>
      {
        var heartbeatConnections = session
          .GetAll<HeartbeatConnection>(AdminUserId, true)
          .ToList();

        var heartbeatConnection = heartbeatConnections.FirstOrDefault(connection =>
          connection.ProtocolName == commServerConnection.ProtocolName);
        
        Assert.IsNotNull(heartbeatConnection);
        Assert.IsNull(heartbeatConnection.HeartbeatLatencyInSeconds);
      });
    }
  }
}