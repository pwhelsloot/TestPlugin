using System;
using System.Configuration;
using System.Threading;
using AMCS.Data;
using AMCS.Data.Configuration;
using AMCS.Data.Configuration.Configuration;
using AMCS.Data.Configuration.TimeZones;
using AMCS.Data.Server;
using AMCS.Data.Server.Broadcast;
using AMCS.Data.Server.Configuration;
using AMCS.Data.Server.Services;
using AMCS.Data.Server.Util;
using AMCS.PlatformFramework.Entity;
using AMCS.PlatformFramework.Server;
using AMCS.PlatformFramework.Server.Configuration;
using AMCS.PlatformFramework.Server.Services;
using Language;

namespace AMCS.PlatformFramework.CommsServer.IntegrationTests
{
  public static class TestServiceSetup
  {
    private static readonly Lazy<DataConfiguration> Configuration =
      new Lazy<DataConfiguration>(DoSetup, LazyThreadSafetyMode.ExecutionAndPublication);

    public static DataConfiguration Setup()
    {
      return Configuration.Value;
    }

    private static DataConfiguration DoSetup()
    {
      var connectionString = ConnectionStringEncryption.DecryptFromConfiguration("PlatformFrameworkConnectionString");

      var entityTypes = TypeManager.FromApplicationPath("AMCS.PlatformFramework.Entity.", "AMCS.Data.Entity.",
        "AMCS.PlatformFramework.IntegrationTests.");
      var serverTypes = TypeManager.FromApplicationPath("AMCS.PlatformFramework.Server.", "AMCS.Data.Server.",
        "AMCS.PlatformFramework.IntegrationTests.");

      var configuration = new DataConfiguration();

      configuration.SetRestrictionsService(new RestrictionService());

      var platformFrameworkLanguageResources = new PlatformFrameworkLanguageResources();
      configuration.SetLocalizationConfiguration(platformFrameworkLanguageResources);

      var projectConfiguration = (IProjectConfiguration)ConfigurationManager.GetSection("amcs");

      configuration.SetServiceRootsConfiguration(
        (IServiceRootsConfiguration)ConfigurationManager.GetSection("amcs.serviceRoots"),
        projectConfiguration.ServiceRootName);

      configuration.ConfigureDataServer(entityTypes, serverTypes);
      configuration.SetProjectConfiguration(
        projectConfiguration);

      var serverConfiguration = (IServerConfiguration)ConfigurationManager.GetSection("amcs.server");
      var platformFrameworkConfiguration =
        (IPlatformFrameworkConfiguration)ConfigurationManager.GetSection("amcs.platform-framework");

      configuration.SetServerConfiguration(
        serverConfiguration,
        connectionString,
        null,
        null,
        Jobs.PriorityQueue,
        entityTypes,
        platformFrameworkLanguageResources,
        StrictModeType.All);

      configuration.ConfigurePlatform(
        serverConfiguration.Platform,
        serverConfiguration.PlatformUI);

      configuration.ConfigurePlatformFramework(
        platformFrameworkConfiguration,
        connectionString);

      // We use the embedded DateTimeZoneProvider to keep unit tests stable.
      configuration.ConfigureTimeZones(new EmbeddedDateTimeZoneProviderProvider());

      configuration.ConfigureBroadcastService(
        null, null);
      configuration.Build();

      CreateBasicData();

      return configuration;
    }

    private static void CreateBasicData()
    {
      using (var session = BslDataSessionFactory.GetDataSession())
      using (var transaction = session.CreateTransaction())
      {
        var userId = DataServices.Resolve<IUserService>().SystemUserSessionKey;

        // Check whether there's an admin user.

        if (DataServices.Resolve<PlatformFramework.Server.User.IUserService>().GetByName(userId, "admin", session) == null)
        {
          // And create one with a default password if none yet exists.

          session.Save(
            userId,
            new UserEntity
            {
              UserName = "admin",
              Password = PasswordHashing.Hash("admin", "admin"),
              EmailAddress = "admin@amcsgroup.com"
            });
        }

        transaction.Commit();
      }
    }
  }
}
