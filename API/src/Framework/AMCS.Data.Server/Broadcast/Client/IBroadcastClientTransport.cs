using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Channels;

namespace AMCS.Data.Server.Broadcast.Client
{
  public interface IBroadcastClientTransport : IDisposable
  {
    event EventHandler Opened;

    event ExceptionEventHandler Closed;

    void Open();

    void SendMessage<T>(BroadcastMessage<T> broadcastMessage);
  }
}
