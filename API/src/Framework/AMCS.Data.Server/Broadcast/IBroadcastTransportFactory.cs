using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.Broadcast.Client;
using AMCS.Data.Server.Broadcast.Receiver;

namespace AMCS.Data.Server.Broadcast
{
  public interface IBroadcastTransportFactory : IDisposable
  {
    /// <summary>
    /// Create a new client transport.
    /// </summary>
    /// <returns>A new client transport.</returns>
    IBroadcastClientTransport CreateClientTransport();

    /// <summary>
    /// Create a new receiver transport.
    /// </summary>
    /// <returns>A new receiver transport.</returns>
    IBroadcastReceiverTransport CreateReceiverTransport();
  }
}
