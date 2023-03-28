using System;
using System.Collections.Generic;
using System.IO;
using AMCS.CommsServer.Serialization;

namespace AMCS.ApiService.Abstractions.CommsServer
{
  public interface ICommsServerClient : IDisposable
  {
    bool IsClientOpened { get; }

    bool IsSessionCreated { get; }
    
    string Protocol { get; }
    
    string Endpoint { get; }
    
    string ServiceRoot { get; }

    Exception UnhandledException { get; }

    CommsServerConnectionState ConnectionState { get; }

    void Publish(params Message[] messages);

    void Publish(IEnumerable<Message> messages);

    void DownloadBlob(string id, Stream stream, bool compress = true);

    string UploadBlob(Stream stream, bool compress = true);

    void DeleteBlob(string id);
  }
}
