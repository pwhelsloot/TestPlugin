using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Configuration;

namespace AMCS.Data.Schema.Configuration
{
  internal class SchemaDiscoveryConfiguration : ISchemaDiscoveryConfiguration
  {
    public IConnectionString ConnectionString { get; }

    public SchemaDiscoveryConfiguration(IConnectionString connectionString)
    {
      ConnectionString = connectionString;
    }
  }
}
