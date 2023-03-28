using System;
using System.Collections.Generic;
using AMCS.Data;
using AMCS.Data.Server.Heartbeat;

namespace AMCS.PlatformFramework.IntegrationTests.Heartbeat
{
  public class FakeConnectionRegistry : IConnectionRegistry
  {
    private readonly List<ICommServerConnection> commServerConnections = new List<ICommServerConnection>();
    
    public TimeSpan MaxHeartbeatLatency => TimeSpan.FromMinutes(7.5);
    
    public void AddCommServerConnections(ICommServerConnection commServerConnection)
    {
      commServerConnections.Add(commServerConnection);
    }
    
    public void ClearCommServerConnections()
    {
      commServerConnections.Clear();
    }
    
    public void RemoveCommServerConnections(ICommServerConnection commServerConnection)
    {
      commServerConnections.Remove(commServerConnection);
    }
    
    public IList<ICommServerConnection> GetCommServerConnections()
    {
      return commServerConnections;
    }
  }

  public static class FakeConnectionRegistryExtensions
  {
    public static void AddCommServerConnection(
      this IConnectionRegistry connectionRegistry,
      ICommServerConnection commServerConnection)
    {
      ((FakeConnectionRegistry)connectionRegistry).AddCommServerConnections(commServerConnection);
      DataServices.Resolve<IHeartbeatService>().SyncConnectionRegistry();
    }
    
    public static void RemoveCommServerConnection(
      this IConnectionRegistry connectionRegistry,
      ICommServerConnection commServerConnection)
    {
      ((FakeConnectionRegistry)connectionRegistry).RemoveCommServerConnections(commServerConnection);
      DataServices.Resolve<IHeartbeatService>().SyncConnectionRegistry();
    }
  }
}