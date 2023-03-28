using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Configuration;

namespace AMCS.Data.Schema.Configuration
{
  public interface ISchemaDiscoveryConfiguration
  {
    IConnectionString ConnectionString { get; }
  }
}
