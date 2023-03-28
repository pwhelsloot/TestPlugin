using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets.Import
{
  /// <summary>
  /// Description of an exception associated with a message.
  /// </summary>
  public class MessageException
  {
    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the exception type.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the stack trace.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string StackTrace { get; }

    /// <summary>
    /// Gets the SQL errors.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<MessageSqlError> SqlErrors { get; }

    /// <summary>
    /// Gets the inner exception.
    /// </summary>
    public MessageException InnerException { get; }

    /// <summary>
    /// Initializes a new <see cref="MessageException"/> instance.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="type">The exception type.</param>
    /// <param name="stackTrace">The stack trace.</param>
    /// <param name="sqlErrors">The SQL errors.</param>
    /// <param name="innerException">The inner exception.</param>
    public MessageException(string message, string type, string stackTrace, List<MessageSqlError> sqlErrors, MessageException innerException)
    {
      Message = message;
      Type = type;
      StackTrace = stackTrace;
      SqlErrors = sqlErrors;
      InnerException = innerException;
    }

    /// <summary>
    /// Creates a <see cref="MessageException"/> from a <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>The message exception.</returns>
    public static MessageException FromException(Exception exception)
    {
      List<MessageSqlError> sqlErrors = null;

      if (exception is SqlException sqlException)
      {
        sqlErrors = new List<MessageSqlError>();

        foreach (SqlError error in sqlException.Errors)
        {
          sqlErrors.Add(new MessageSqlError(
            error.Number,
            error.Message,
            SqlErrorUtils.GetFriendlyErrorMessage(error),
            error.LineNumber,
            error.Source,
            error.Procedure
          ));
        }
      }

      return new MessageException(
        exception.Message,
        exception.GetType().FullName,
        exception.StackTrace,
        sqlErrors,
        exception.InnerException == null ? null : FromException(exception.InnerException)
      );
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      var sb = new StringBuilder();

      for (var exception = this; exception != null; exception = exception.InnerException)
      {
        if (sb.Length > 0)
          sb.AppendLine().AppendLine("=== Caused by ===").AppendLine();

        sb.Append(exception.Message).Append(" (").Append(exception.Type).AppendLine(")");

        if (exception.StackTrace != null)
          sb.AppendLine().AppendLine(exception.StackTrace.TrimEnd()).AppendLine();
      }

      return sb.ToString();
    }
  }
}
