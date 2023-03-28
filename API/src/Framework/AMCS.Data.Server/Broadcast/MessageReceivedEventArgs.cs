using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Broadcast
{
  public class MessageReceivedEventArgs : EventArgs
  {
    public BroadcastReceiveMessage BroadcastReceiveMessage { get; }

    public MessageReceivedEventArgs(BroadcastReceiveMessage broadcastReceiveMessage)
    {
      BroadcastReceiveMessage = broadcastReceiveMessage;
    }
  }

  public delegate void MessageReceivedEventHandler(MessageReceivedEventArgs e);
}
