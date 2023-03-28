using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  /// <summary>
  /// Specifies the message type.
  /// </summary>
  public enum MessageType
  {
    /// <summary>
    /// Indicates an informational message.
    /// </summary>
    Info,
    /// <summary>
    /// Indicates a warning.
    /// </summary>
    Warning,
    /// <summary>
    /// Indicates an error.
    /// </summary>
    Error
  }
}
