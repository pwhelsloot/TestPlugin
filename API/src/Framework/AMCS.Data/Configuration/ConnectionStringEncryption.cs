using System.Configuration;
using System.Data.SqlClient;
using AMCS.Data.Support.Security;

namespace AMCS.Data.Configuration
{
  public static class ConnectionStringEncryption
  {
    public static IConnectionString DecryptFromConfiguration(string name)
    {
      var connectionString = ConfigurationManager.ConnectionStrings[name];
      if (connectionString != null && !string.IsNullOrEmpty(connectionString.ConnectionString))
        return Decrypt(connectionString.ConnectionString);

      return null;
    }

    public static IConnectionString Decrypt(string connectionString)
    {
      var builder = new SqlConnectionStringBuilder(connectionString);

      if (!string.IsNullOrEmpty(builder.Password))
      {
        builder.Password = StringEncryptor.DefaultEncryptor.Decrypt(builder.Password);
        connectionString = builder.ToString();
      }

      return new ConnectionString(connectionString);
    }
  }
}
