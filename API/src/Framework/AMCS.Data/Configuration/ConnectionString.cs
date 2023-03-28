using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Configuration
{
  public class ConnectionString : IConnectionString
  {
    private readonly string connectionString;

    public ConnectionString(string connectionString)
    {
      this.connectionString = connectionString;
    }

    public string GetConnectionString()
    {
      return connectionString;
    }
  }
}
