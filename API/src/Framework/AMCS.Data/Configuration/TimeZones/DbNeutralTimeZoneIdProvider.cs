using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Support;
using Microsoft.Azure.Services.AppAuthentication;

namespace AMCS.Data.Configuration.TimeZones
{
  public class DbNeutralTimeZoneIdProvider : INeutralTimeZoneIdProvider
  {
    public const string DefaultQuery = "select [NeutralTimeZoneId] from [dbo].[DBTimeZoneConfiguration]";

    private readonly IConnectionString connectionString;
    private readonly string query;

    public string GetNeutralTimeZoneId()
    {
      using (var connection = ConnectionStringUtils.OpenSqlConnection(connectionString))
      using (var command = connection.CreateCommand())
      {
        command.CommandText = query;

        return (string)command.ExecuteScalar();
      }
    }

    public DbNeutralTimeZoneIdProvider(IConnectionString connectionString)
      : this(connectionString, DefaultQuery)
    {
    }

    public DbNeutralTimeZoneIdProvider(IConnectionString connectionString, string query)
    {
      this.connectionString = connectionString;
      this.query = query;
    }
  }
}
