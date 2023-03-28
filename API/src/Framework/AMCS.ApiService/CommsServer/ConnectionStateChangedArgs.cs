using System;
using AMCS.ApiService.Abstractions.CommsServer;

namespace AMCS.ApiService.CommsServer
{
  public class ConnectionStateChangedArgs : EventArgs
  {
    public CommsServerConnectionState ConnectionState { get; }

    public ConnectionStateChangedArgs(CommsServerConnectionState connectionState)
    {
      ConnectionState = connectionState;
    }
  }

  public delegate void ConnectionStateChangedHandler(object sender, ConnectionStateChangedArgs e);
}