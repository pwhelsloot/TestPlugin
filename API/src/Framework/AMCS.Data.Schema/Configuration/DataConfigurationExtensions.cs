using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Configuration;
using Autofac;

namespace AMCS.Data.Schema.Configuration
{
  public static class DataConfigurationExtensions
  {
    public static void ConfigureSchemaDiscovery(this DataConfiguration self, IConnectionString connectionString)
    {
      self.ContainerBuilder
        .RegisterInstance(new SchemaDiscoveryConfiguration(connectionString))
        .As<ISchemaDiscoveryConfiguration>();
    }
  }
}
