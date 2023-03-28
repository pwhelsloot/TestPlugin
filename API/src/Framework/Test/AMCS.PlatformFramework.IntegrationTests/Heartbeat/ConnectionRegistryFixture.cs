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
  public class ConnectionRegistryFixture : HeartbeatTestBase
  {
    [Test]
    public void GivenNew_WhenAddingCommServerConnection_ThenAdded()
    {
      var connectionRegistry = DataServices.Resolve<IConnectionRegistry>();
      var commServerConnection = new CommServerConnectionBuilder().Build();
      connectionRegistry.AddCommServerConnection(commServerConnection);

      WithSession(session =>
      {
        var heartbeatConnection = session.GetByTemplate(AdminUserId, new HeartbeatConnection
        {
          ProtocolName = commServerConnection.ProtocolName,
          InstanceName = commServerConnection.InstanceName
        }, true, false);
        
        Assert.IsNotNull(heartbeatConnection);
        Assert.AreEqual(HeartbeatConnectionStatus.Unknown, heartbeatConnection.Status);
      });
    }
    
    [Test]
    public void GivenDuplicate_WhenAddingCommServerConnection_ThenNotAdded()
    {
      var connectionRegistry = DataServices.Resolve<IConnectionRegistry>();
      var commServerConnection1 = new CommServerConnectionBuilder().Build();
      var commServerConnection2 = new CommServerConnectionBuilder()
        .WithProtocol(commServerConnection1.ProtocolName)
        .Build();

      connectionRegistry.AddCommServerConnection(commServerConnection1);
      connectionRegistry.AddCommServerConnection(commServerConnection2);

      WithSession(session =>
      {
        var heartbeatConnection = session.GetByTemplate(AdminUserId, new HeartbeatConnection
        {
          ProtocolName = commServerConnection1.ProtocolName,
          InstanceName = null
        }, true, false);
        
        Assert.IsNotNull(heartbeatConnection);
        Assert.AreEqual(HeartbeatConnectionStatus.Unknown, heartbeatConnection.Status);
      });
    }
    
    [Test]
    public void GivenExisting_WhenRemovingCommServerConnection_ThenRemoved()
    {
      var commServerConnection = new CommServerConnectionBuilder().Build();

      var connectionRegistry = DataServices.Resolve<IConnectionRegistry>();
      connectionRegistry.AddCommServerConnection(commServerConnection);
      connectionRegistry.RemoveCommServerConnection(commServerConnection);

      WithSession(session =>
      {
        var heartbeatConnection = session.GetByTemplate(AdminUserId, new HeartbeatConnection
        {
          ProtocolName = commServerConnection.ProtocolName,
          InstanceName = commServerConnection.InstanceName
        }, true, false);
        
        Assert.IsNull(heartbeatConnection);
      });
    }

    [Test]
    public void GivenExistingRegistry_WhenAddingCommServerConnection_ThenAllSynced()
    {
      var protocolName = $"Existing {Guid.NewGuid():N}";
      var instanceName = $"Existing {Guid.NewGuid():N}";
      int? existingId = null;
      
      WithSession(session =>
      {
        existingId = session.Save(AdminUserId, new HeartbeatConnection
        {
          ProtocolName = protocolName,
          InstanceName = instanceName,
          Status = HeartbeatConnectionStatus.Ready,
          Timestamp = DateTime.UtcNow
        });
      });

      var connectionRegistry = DataServices.Resolve<IConnectionRegistry>();
      connectionRegistry.AddCommServerConnection(new CommServerConnectionBuilder()
        .WithProtocol(protocolName)
        .WithInstance(instanceName)
        .Build());

      connectionRegistry.AddCommServerConnection(new CommServerConnectionBuilder().Build());
      connectionRegistry.AddCommServerConnection(new CommServerConnectionBuilder().Build());
      connectionRegistry.AddCommServerConnection(new CommServerConnectionBuilder().Build());

      WithSession(session =>
      {
        var heartbeatConnections = session.GetAll<HeartbeatConnection>(AdminUserId, true);
        var existingConnection = heartbeatConnections.FirstOrDefault(connection => connection.Id == existingId);

        Assert.IsNotNull(existingConnection);
        Assert.AreEqual(HeartbeatConnectionStatus.Ready, existingConnection.Status);
        Assert.AreEqual(connectionRegistry.GetCommServerConnections().Count, heartbeatConnections.Count);
      });
    }
  }
}