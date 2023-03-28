using System;
using System.Linq;
using AMCS.ApiService.Abstractions.CommsServer;
using AMCS.Data;
using AMCS.Data.Entity.Heartbeat;
using AMCS.Data.Server;
using AMCS.Data.Server.Heartbeat;
using NUnit.Framework;

namespace AMCS.PlatformFramework.IntegrationTests.Heartbeat
{
  [TestFixture]
  public class HeartbeatStorageFixture : HeartbeatTestBase
  {
    [Test]
    public void GivenAllConnectionStates_WhenFlushingCommServerConnection_ThenUpdated()
    {
      var readyConnection = new CommServerConnectionBuilder().Build();
      var processingConnection = new CommServerConnectionBuilder().Build();
      var unknownConnection = new CommServerConnectionBuilder()
        .WithCommsServerConnectionState(CommsServerConnectionState.NotConnected)
        .Build();
      
      var connectionRegistry = DataServices.Resolve<IConnectionRegistry>();
      connectionRegistry.AddCommServerConnection(unknownConnection);
      connectionRegistry.AddCommServerConnection(readyConnection);
      connectionRegistry.AddCommServerConnection(processingConnection);
      
      var heartbeatService = DataServices.Resolve<IHeartbeatService>();
      heartbeatService.SetMessageProcessing(processingConnection.ProtocolName, processingConnection.InstanceName);
      heartbeatService.SetMessageProcessed(readyConnection.ProtocolName, readyConnection.InstanceName);
      
      heartbeatService.FlushConnectionRegistry();

      WithSession(session =>
      {
        var heartbeatConnections = session
          .GetAll<HeartbeatConnection>(AdminUserId, true)
          .ToList();
        
        Assert.AreEqual(1, heartbeatConnections.Count(connection => connection.Status == HeartbeatConnectionStatus.Unknown));
        Assert.AreEqual(1, heartbeatConnections.Count(connection => connection.Status == HeartbeatConnectionStatus.Ready));
        Assert.AreEqual(1, heartbeatConnections.Count(connection => connection.Status == HeartbeatConnectionStatus.Processing));
      });
    }
    
    [Test]
    public void GivenNotConnected_WhenFlushingCommServerConnection_ThenConnectionSkipped()
    {
      var timeStamp = DateTime.UtcNow.AddHours(-1);
      
      var readyConnection = new CommServerConnectionBuilder()
        .WithTimestamp(timeStamp)
        .Build();
      
      var notConnectedConnection = new CommServerConnectionBuilder()
        .WithTimestamp(timeStamp)
        .WithCommsServerConnectionState(CommsServerConnectionState.NotConnected)
        .Build();
      
      var connectionRegistry = DataServices.Resolve<IConnectionRegistry>();
      connectionRegistry.AddCommServerConnection(readyConnection);
      connectionRegistry.AddCommServerConnection(notConnectedConnection);
      
      var heartbeatService = DataServices.Resolve<IHeartbeatService>();
      heartbeatService.SetMessageProcessed(readyConnection.ProtocolName, readyConnection.InstanceName);
      heartbeatService.SetMessageProcessed(notConnectedConnection.ProtocolName, notConnectedConnection.InstanceName);
      heartbeatService.FlushConnectionRegistry();
    
      WithSession(session =>
      {
        var heartbeatConnections = session
          .GetAll<HeartbeatConnection>(AdminUserId, true)
          .ToList();
        
        Assert.AreEqual(1, heartbeatConnections.Count(connection => connection.Status == HeartbeatConnectionStatus.Unknown));
        Assert.AreEqual(1, heartbeatConnections.Count(connection => connection.Status == HeartbeatConnectionStatus.Ready));
      });
    }
  }
}