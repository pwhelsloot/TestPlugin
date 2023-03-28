using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using AMCS.Channels;

namespace AMCS.Data.Server.Broadcast.Receiver
{
  public interface IBroadcastReceiverTransport : IDisposable
  {
    event EventHandler Opened;

    event ExceptionEventHandler Closed;

    event MessageReceivedEventHandler MessageReceived;

    void Open();

    void Close(CloseMode mode);
  }
}
