using System;
using AMCS.ApiService.Abstractions.CommsServer;
using AMCS.PlatformFramework.Server.Heartbeat;

namespace AMCS.PlatformFramework.IntegrationTests.Heartbeat
{
  public class CommServerConnectionBuilder
  {
    private string protocolName = Guid.NewGuid().ToString();
    private string instanceName;
    private DateTime timestamp = DateTime.UtcNow;
    private CommsServerConnectionState connectionState = CommsServerConnectionState.Connected;
    
    public CommServerConnection Build()
    {
      var commServerConnection = new CommServerConnection(protocolName, instanceName, 
        new FakeCommsServerPushProtocol(connectionState));
      commServerConnection.TimeStamp = timestamp;
      
      return commServerConnection;
    }
    
    public CommServerConnectionBuilder WithProtocol(string value)
    {
      protocolName = value;
      return this;
    }
    
    public CommServerConnectionBuilder WithInstance(string value)
    {
      instanceName = value;
      return this;
    }
    
    public CommServerConnectionBuilder WithTimestamp(DateTime value)
    {
      timestamp = value;
      return this;
    }
    
    public CommServerConnectionBuilder WithCommsServerConnectionState(CommsServerConnectionState value)
    {
      connectionState = value;
      return this;
    }
  }
}