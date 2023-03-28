using System;
using System.Configuration;
using System.Threading;
using AMCS.Data;
using AMCS.Data.Configuration;
using AMCS.Data.Configuration.Configuration;
using AMCS.Data.Configuration.TimeZones;
using AMCS.Data.Entity.Plugin;
using AMCS.Data.Server;
using AMCS.Data.Server.Broadcast;
using AMCS.Data.Server.Configuration;
using AMCS.Data.Server.Glossary;
using AMCS.Data.Server.Heartbeat;
using AMCS.Data.Server.Plugin;
using AMCS.Data.Server.Services;
using AMCS.Data.Server.Util;
using AMCS.Data.Server.WebHook;
using AMCS.PlatformFramework.Entity;
using AMCS.PlatformFramework.IntegrationTests.Heartbeat;
using AMCS.PlatformFramework.IntegrationTests.Plugin.MetadataRegistry;
using AMCS.PlatformFramework.Server;
using AMCS.PlatformFramework.Server.Configuration;
using AMCS.PlatformFramework.Server.Plugin;
using AMCS.PlatformFramework.Server.Services;
using AMCS.WebDiagnostics;
using Autofac;
using Language;

namespace AMCS.PlatformFramework.IntegrationTests
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
      var azureBlobStorageConnectionString = ConnectionStringEncryption.DecryptFromConfiguration("AzureBlobStorage");

      var entityTypes = TypeManager.FromApplicationPath(
        "AMCS.PlatformFramework.Entity.",
        "AMCS.Data.Entity.",
        "AMCS.PlatformFramework.IntegrationTests.");

      var serverTypes = TypeManager.FromApplicationPath(
        "AMCS.PlatformFramework.Server.",
        "AMCS.Data.Server.",
        "AMCS.PlatformFramework.IntegrationTests.");

      var configuration = new DataConfiguration();

      configuration.SetRestrictionsService(new RestrictionService());
      configuration.SetSettingsService(new SettingsService());

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
        StrictModeType.None);

      configuration.ConfigureBroadcastService(null, null);
      
      configuration.ConfigureSingleTenantPluginSystem(
        PluginConstants.VendorId.AMCS, 
        "platformframework", 
        typeof(ServiceSetup).Assembly.GetName().Version?.ToString(),
        "4C0FFDF1-CC0A-4C1F-A023-7B2C0354AE98",
        "http://localhost:26519/");
      
      configuration.ConfigurePlatform(
        serverConfiguration.Platform,
        serverConfiguration.PlatformUI);

      configuration.ConfigurePlatformFramework(
        platformFrameworkConfiguration,
        connectionString);

      configuration.ConfigureDataSets(serverConfiguration, azureBlobStorageConnectionString, serverTypes);
      configuration.ConfigureTestData(serverTypes);

      // We use the embedded DateTimeZoneProvider to keep unit tests stable
      configuration.ConfigureTimeZones(new EmbeddedDateTimeZoneProviderProvider(), new SystemNeutralTimeZoneIdProvider());
      
      configuration.ConfigureBroadcastService(null, null);
      configuration.ConfigureBslTriggers(Jobs.PriorityQueue, entityTypes, serverTypes);

      configuration.ConfigureBusinessObjects(typeof(UserEntity).Assembly, entityTypes);
      configuration.ConfigureWebHooks();
      configuration.ConfigurePluginMetadata();
      configuration.ConfigureMetadataRegistries();

      configuration.ConfigureHeartbeat();
      configuration.SetDiagnostics();
      configuration.ConfigureGlossary();

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

    private static void ConfigureHeartbeat(this DataConfiguration configuration)
    {
      configuration.ContainerBuilder
        .RegisterType<FakeConnectionRegistry>()
        .SingleInstance()
        .AsSelf()
        .As<IConnectionRegistry>();

      configuration.AddHeartbeat();
    }
    
    private static void SetDiagnostics(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<DiagnosticsManager>()
        .SingleInstance()
        .As<IDiagnosticsManager>();
    }

    private static void ConfigureMetadataRegistries(this DataConfiguration configuration)
    {
      configuration.AddWorkflowActivityRegistry();
      configuration.AddUiComponentsRegistry<TestUiComponentsMetadataRegistryService>();
      
      configuration.ContainerBuilder
        .RegisterType<TestWorkflowActivityMetadataService>()
        .SingleInstance()
        .AutoActivate();
    }

    private static void ConfigureGlossary(this DataConfiguration configuration)
    {
      configuration.ContainerBuilder
        .RegisterType<GlossaryCacheService>()
        .SingleInstance()
        .AsSelf()
        .AutoActivate()
        .As<IGlossaryCacheService>();
    }
  }
}
