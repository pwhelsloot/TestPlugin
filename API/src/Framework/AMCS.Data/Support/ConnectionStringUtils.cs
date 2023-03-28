using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Configuration;
using Microsoft.Azure.Services.AppAuthentication;

namespace AMCS.Data.Support
{
  public static class ConnectionStringUtils
  {
    private static readonly AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

    public static bool HasCredentials(string connectionString)
    {
      var builder = new SqlConnectionStringBuilder(connectionString);

      return
        !string.IsNullOrEmpty(builder.Password) ||
        builder.IntegratedSecurity;
    }

    public static SqlConnection OpenSqlConnection(string connectionString)
    {
      var connection = new SqlConnection(connectionString);

      return OpenConnection(connectionString, connection);
    }
    
    public static SqlConnection OpenSqlConnection(IConnectionString connectionString)
    {
      var connection = new SqlConnection(connectionString.GetConnectionString());

      return OpenConnection(connectionString.GetConnectionString(), connection);
    }

    private static SqlConnection OpenConnection(string connectionString, SqlConnection connection)
    {
      if (!HasCredentials(connectionString))
        connection.AccessToken = azureServiceTokenProvider.GetAccessTokenAsync("https://database.windows.net/").Result;

      connection.Open();

      return connection;
    }
  }
}
