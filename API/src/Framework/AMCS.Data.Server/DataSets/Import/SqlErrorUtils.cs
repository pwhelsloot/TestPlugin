using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import
{
  /// <summary>
  /// Provides methods for working with <see cref="SqlError"/> instances.
  /// </summary>
  internal class SqlErrorUtils
  {
    /// <summary>
    /// Gets a friendly error message for an SQL exception.
    /// </summary>
    /// <param name="sqlException">The exception to get the friendly error message for.</param>
    /// <param name="firstOnly">Whether or not to get the friendly error message for just the first error.</param>
    /// <returns>The friendly error message.</returns>
    public static string GetFriendlyErrorMessage(SqlException sqlException, bool firstOnly = true)
    {
      if (sqlException == null || sqlException.Errors.Count == 0)
        return null;

      if (firstOnly || sqlException.Errors.Count == 1)
        return GetFriendlyErrorMessage(sqlException.Errors[0]);

      var sb = new StringBuilder();

      foreach (SqlError err in sqlException.Errors)
      {
        sb.AppendLine(GetFriendlyErrorMessage(err));
      }

      return sb.ToString();
    }

    /// <summary>
    /// Gets a friendly translation of error number code.
    /// </summary>
    /// <param name="error">The error to get the message for.</param>
    /// <returns>The friendly error message.</returns>
    public static string GetFriendlyErrorMessage(SqlError error)
    {
      var sb = new StringBuilder();

      switch (error.Number)
      {
        case 49920:
          sb.AppendLine("Cannot process request. Too many operations in progress for subscription '%ld'.");
          sb.AppendLine("The service is busy processing multiple requests for this subscription.");
          sb.AppendLine("Requests are currently blocked for resource optimization. Query sys.dm_operation_status for operation status.");
          sb.AppendLine("Wait until pending requests are complete or delete one of your pending requests and retry your request later.");
          break;
        case 49919:
          sb.AppendLine("Cannot process create or update request. Too many create or update operations in progress for subscription '%ld'.");
          sb.AppendLine("The service is busy processing multiple create or update requests for your subscription or server.");
          sb.AppendLine("Requests are currently blocked for resource optimization. Query sys.dm_operation_status for pending operations.");
          sb.AppendLine("Wait till pending create or update requests are complete or delete one of your pending requests and");
          sb.AppendLine("retry your request later.");
          break;
        case 49918:
          sb.AppendLine("Cannot process request. Not enough resources to process request.");
          sb.AppendLine("The service is currently busy.Please retry the request later.");
          break;
        case 41839:
          sb.AppendLine("Transaction exceeded the maximum number of commit dependencies.");
          break;
        case 41325:
          sb.AppendLine("The current transaction failed to commit due to a serializable validation failure.");
          break;
        case 41305:
          sb.AppendLine("The current transaction failed to commit due to a repeatable read validation failure.");
          break;
        case 41302:
          sb.AppendLine("The current transaction attempted to update a record that has been updated since the transaction started.");
          break;
        case 41301:
          sb.AppendLine("A previous transaction that the current transaction took a dependency on has aborted,");
          sb.AppendLine("and the current transaction can no longer commit");
          break;
        case 40613:
          sb.AppendLine("Database XXXX on server YYYY is not currently available. Please retry the connection later.");
          sb.AppendLine("If the problem persists, contact customer support, and provide them the session tracing ID of ZZZZZ.");
          break;
        case 40501:
          sb.AppendLine("The service is currently busy. Retry the request after 10 seconds. Code: (reason code to be decoded)");
          break;
        case 40197:
          sb.AppendLine("The service has encountered an error processing your request. Please try again.");
          break;
        case 10929:
          sb.AppendLine("Resource ID: %d. The %s minimum guarantee is %d, maximum limit is %d and the current usage for the database is %d.");
          sb.AppendLine("However, the server is currently too busy to support requests greater than %d for this database.");
          sb.AppendLine("For more information, see http://go.microsoft.com/fwlink/?LinkId=267637. Otherwise, please try again.");
          break;
        case 10928:
          sb.AppendLine("Resource ID: %d. The %s limit for the database is %d and has been reached. For more information,");
          sb.AppendLine("see http://go.microsoft.com/fwlink/?LinkId=267637.");
          break;
        case 10060:
          sb.AppendLine("A network-related or instance-specific error occurred while establishing a connection to SQL Server.");
          sb.AppendLine("The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server");
          sb.AppendLine("is configured to allow remote connections. (provider: TCP Provider, error: 0 - A connection attempt failed");
          sb.AppendLine("because the connected party did not properly respond after a period of time, or established connection failed");
          sb.AppendLine("because connected host has failed to respond.)'}");
          break;
        case 10054:
          sb.AppendLine("A transport-level error has occurred when sending the request to the server.");
          sb.AppendLine("(provider: TCP Provider, error: 0 - An existing connection was forcibly closed by the remote host.)");
          break;
        case 10053:
          sb.AppendLine("A transport-level error has occurred when receiving results from the server.");
          sb.AppendLine("An established connection was aborted by the software in your host machine.");
          break;
        case 1205:
          sb.AppendLine("Deadlock");
          break;
        case 233:
          sb.AppendLine("The client was unable to establish a connection because of an error during connection initialization process before login.");
          sb.AppendLine("Possible causes include the following: the client tried to connect to an unsupported version of SQL Server;");
          sb.AppendLine("the server was too busy to accept new connections; or there was a resource limitation (insufficient memory or maximum");
          sb.AppendLine("allowed connections) on the server. (provider: TCP Provider, error: 0 - An existing connection was forcibly closed by");
          sb.AppendLine("the remote host.)");
          break;
        case 121:
          sb.AppendLine("The semaphore timeout period has expired");
          break;
        case 64:
          sb.AppendLine("A connection was successfully established with the server, but then an error occurred during the login process.");
          sb.AppendLine("(provider: TCP Provider, error: 0 - The specified network name is no longer available.");
          break;
        case 20:
          sb.AppendLine("The instance of SQL Server you attempted to connect to does not support encryption.");
          break;
#if DEBUG
        case -2:
          sb.AppendLine("These are enabled to simplify debug testing. Just stopping the network interface");
          sb.AppendLine("on the database will trigger these.");
          sb.AppendLine("This exception can be thrown even if the operation completed successfully, so it's safer to let the application fail");
          sb.AppendLine("Timeout expired. The timeout period elapsed prior to completion of the operation or the server is not responding. The statement has been terminated.");
          break;
        case 53:
          sb.AppendLine("A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not");
          sb.AppendLine("found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote");
          sb.AppendLine("connections. (provider: Named Pipes Provider, Could not open a connection to SQL Server)");
          break;
        case 40:
          sb.AppendLine("Could not open a connection to SQL Server");
          break;
#endif
        default:
          sb.AppendLine($"{error.Number} {error.Message}");
          break;
      }

      return sb.ToString();
    }
  }
}
