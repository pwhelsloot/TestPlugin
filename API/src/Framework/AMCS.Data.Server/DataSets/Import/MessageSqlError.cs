using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  /// <summary>
  /// Represents an SQL error.
  /// </summary>
  public class MessageSqlError
  {
    /// <summary>
    /// Gets the number of the error.
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the friendly error description.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the line number.
    /// </summary>
    public int LineNumber { get; }

    /// <summary>
    /// Gets the source.
    /// </summary>
    public string Source { get; }

    /// <summary>
    /// Gets the procedure name.
    /// </summary>
    public string Procedure { get; }

    /// <summary>
    /// Initializes a new <see cref="MessageSqlError"/>.
    /// </summary>
    /// <param name="number">The number of the error.</param>
    /// <param name="message">The message.</param>
    /// <param name="description">The description.</param>
    /// <param name="lineNumber">The line number.</param>
    /// <param name="source">The source.</param>
    /// <param name="procedure">The procedure name.</param>
    public MessageSqlError(int number, string message, string description, int lineNumber, string source, string procedure)
    {
      Number = number;
      Message = message;
      Description = description;
      LineNumber = lineNumber;
      Source = source;
      Procedure = procedure;
    }
  }
}
