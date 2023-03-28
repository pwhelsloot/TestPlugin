using System.Collections.Generic;
using System.Threading;
using AMCS.CommsServer.Serialization;

namespace AMCS.ApiService.Abstractions.CommsServer
{
  public interface ICommsServerProtocol
  {
    ICommsServerClient Client { get; }

    void ProcessMessages(IList<Message> messages, CancellationToken cancellationToken);

    void StateChanged(CommsServerConnectionState connectionState);
  }
}
