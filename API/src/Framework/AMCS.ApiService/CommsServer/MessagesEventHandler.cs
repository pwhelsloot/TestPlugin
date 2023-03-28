using System;
using System.Collections.Generic;
using System.Threading;
using AMCS.CommsServer.Serialization;

namespace AMCS.ApiService.CommsServer
{
  internal class MessagesEventArgs : EventArgs
  {
    public List<Message> Messages { get; }
    public CancellationToken CancellationToken { get; }

    public MessagesEventArgs(List<Message> messages, CancellationToken cancellationToken)
    {
      Messages = messages;
      CancellationToken = cancellationToken;
    }
  }

  internal delegate void MessagesEventHandler(object sender, MessagesEventArgs e);
}
