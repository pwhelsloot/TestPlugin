using System;
using System.Collections.Generic;

namespace AMCS.Data.Configuration
{
  internal class ConnectionStringResolver : IConnectionStringResolver
  {
    private readonly Dictionary<string, IConnectionString> connectionStrings;

    public ConnectionStringResolver(Dictionary<string, IConnectionString> connectionStrings)
    {
      this.connectionStrings = connectionStrings;
    }

    public IConnectionString GetConnectionString(string name)
    {
      if (connectionStrings.TryGetValue(name, out IConnectionString connectionString))
        return connectionString;

      return null;
    }
  }
}