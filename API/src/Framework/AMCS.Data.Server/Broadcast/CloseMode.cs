using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Broadcast
{
  [Flags]
  public enum CloseMode
  {
    /// <summary>
    /// Indicates no close mode.
    /// </summary>
    None = 0,

    /// <summary>
    /// Stops accepting messages to send out through the transport.
    /// </summary>
    Send = 1,

    /// <summary>
    /// Stops receiving messages from the transport.
    /// </summary>
    Receive = 2,

    /// <summary>
    /// Specifies both <see cref="Send"/> and <see cref="Receive"/>.
    /// </summary>
    Both = Send | Receive
  }
}
