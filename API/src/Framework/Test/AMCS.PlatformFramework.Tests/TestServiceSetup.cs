using System;
using System.Configuration;
using System.Threading;
using AMCS.Data.Configuration;
using AMCS.Data.Server.Configuration;

namespace AMCS.PlatformFramework.Tests
{
  public static class TestServiceSetup
  {
    private static readonly Lazy<DataConfiguration> Configuration = new Lazy<DataConfiguration>(DoSetup, LazyThreadSafetyMode.ExecutionAndPublication);

    public static DataConfiguration Setup()
    {
      return Configuration.Value;
    }

    private static DataConfiguration DoSetup()
    {
      var entityTypes = TypeManager.FromApplicationPath("AMCS.PlatformFramework.Entity.", "AMCS.Data.Entity.");
      var serverTypes = TypeManager.FromApplicationPath("AMCS.PlatformFramework.Server.", "AMCS.Data.Server.");

      var configuration = new DataConfiguration();

      configuration.ConfigureFakeDataSessionFactory();

      configuration.ConfigureDataServer(entityTypes, serverTypes);

      configuration.Build();

      return configuration;
    }
  }
}
