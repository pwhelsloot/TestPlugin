using System.Linq;
using AMCS.Data;
using AMCS.Data.Entity.Heartbeat;
using AMCS.Data.Server;
using AMCS.Data.Server.Heartbeat;
using NUnit.Framework;

namespace AMCS.PlatformFramework.IntegrationTests.Heartbeat
{
  [TestFixture]
  public class HeartbeatTrackProcessingFixture : HeartbeatTestBase
  {
    [Test]
    public void GivenDuplicate_WhenFlushingCommServerConnection_DuplicateException()
    {
      var unknownConnection1 = new CommServerConnectionBuilder().Build();
      var unknownConnection2 = new CommServerConnectionBuilder()
        .WithProtocol(unknownConnection1.ProtocolName)
        .WithInstance(unknownConnection1.InstanceName)
        .Build();

      var connectionRegistry = DataServices.Resolve<IConnectionRegistry>();
      connectionRegistry.AddCommServerConnection(unknownConnection1);
      connectionRegistry.AddCommServerConnection(unknownConnection2);
      
      var heartbeatService = DataServices.Resolve<IHeartbeatService>();
      Assert.Throws<HeartbeatDuplicateConfigurationException>(() =>
        heartbeatService.SetMessageProcessing(unknownConnection1.ProtocolName, unknownConnection1.InstanceName));
    }
    
    [Test]
    public void GivenProcessing_WhenFlushingCommServerConnection_ThenProcessing()
    {
      var unknownConnection1 = new CommServerConnectionBuilder().Build();
      var readyConnection = new CommServerConnectionBuilder().Build();
      var processingConnection = new CommServerConnectionBuilder().Build();

      var connectionRegistry = DataServices.Resolve<IConnectionRegistry>();
      connectionRegistry.AddCommServerConnection(unknownConnection1);
      connectionRegistry.AddCommServerConnection(readyConnection);
      connectionRegistry.AddCommServerConnection(processingConnection);
      
      readyConnection.ConnectionStatus = HeartbeatConnectionStatus.Ready;
      processingConnection.ConnectionStatus = HeartbeatConnectionStatus.Processing;
      
      var heartbeatService = DataServices.Resolve<IHeartbeatService>();
      heartbeatService.SetMessageProcessing(unknownConnection1.ProtocolName, unknownConnection1.InstanceName);
      heartbeatService.FlushConnectionRegistry();

      WithSession(session =>
      {
        var heartbeatConnections = session
          .GetAll<HeartbeatConnection>(AdminUserId, true)
          .ToList();
        
        Assert.AreEqual(0, heartbeatConnections.Count(connection => connection.Status == HeartbeatConnectionStatus.Unknown));
        Assert.AreEqual(1, heartbeatConnections.Count(connection => connection.Status == HeartbeatConnectionStatus.Ready));
        Assert.AreEqual(2, heartbeatConnections.Count(connection => connection.Status == HeartbeatConnectionStatus.Processing));
      });
    }
    
    [Test]
    public void GivenProcessed_WhenFlushingCommServerConnection_ThenReady()
    {
      var unknownConnection = new CommServerConnectionBuilder().Build();
      var readyConnection = new CommServerConnectionBuilder().Build();
      var processingConnection = new CommServerConnectionBuilder().Build();
      
      var connectionRegistry = DataServices.Resolve<IConnectionRegistry>();
      connectionRegistry.AddCommServerConnection(unknownConnection);
      connectionRegistry.AddCommServerConnection(readyConnection);
      connectionRegistry.AddCommServerConnection(processingConnection);
      
      readyConnection.ConnectionStatus = HeartbeatConnectionStatus.Ready;
      processingConnection.ConnectionStatus = HeartbeatConnectionStatus.Processing;
      
      var heartbeatService = DataServices.Resolve<IHeartbeatService>();
      heartbeatService.SetMessageProcessed(unknownConnection.ProtocolName, unknownConnection.InstanceName);
      heartbeatService.FlushConnectionRegistry();

      WithSession(session =>
      {
        var heartbeatConnections = session
          .GetAll<HeartbeatConnection>(AdminUserId, true)
          .ToList();
        
        Assert.AreEqual(0, heartbeatConnections.Count(connection => connection.Status == HeartbeatConnectionStatus.Unknown));
        Assert.AreEqual(2, heartbeatConnections.Count(connection => connection.Status == HeartbeatConnectionStatus.Ready));
        Assert.AreEqual(1, heartbeatConnections.Count(connection => connection.Status == HeartbeatConnectionStatus.Processing));
      });
    }
  }
}