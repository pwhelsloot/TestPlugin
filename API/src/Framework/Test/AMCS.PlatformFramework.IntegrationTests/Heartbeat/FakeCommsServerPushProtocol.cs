using System;
using System.Collections.Generic;
using System.IO;
using AMCS.ApiService.Abstractions.CommsServer;
using AMCS.CommsServer.Serialization;

namespace AMCS.PlatformFramework.IntegrationTests.Heartbeat
{
  public class FakeCommsServerPushProtocol : ICommsServerClient
  {
    public bool IsClientOpened { get; }
    public bool IsSessionCreated { get; }
    public string Protocol { get; }
    public string Endpoint { get; }
    public string ServiceRoot { get; }
    public Exception UnhandledException { get; }
    public CommsServerConnectionState ConnectionState { get; }

    public FakeCommsServerPushProtocol(CommsServerConnectionState connectionState = CommsServerConnectionState.Connected)
    {
      ConnectionState = connectionState;
    }

    public void Publish(params Message[] messages)
    {
      throw new NotImplementedException();
    }

    public void Publish(IEnumerable<Message> messages)
    {
      throw new NotImplementedException();
    }

    public void DownloadBlob(string id, Stream stream, bool compress = true)
    {
      throw new NotImplementedException();
    }

    public string UploadBlob(Stream stream, bool compress = true)
    {
      throw new NotImplementedException();
    }

    public void DeleteBlob(string id)
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
    }
  }
}