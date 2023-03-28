namespace AMCS.TestPlugin.IntegrationTests
{
  using System.Xml.Linq;
  using AMCS.TestPlugin.Server.Entity;
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
  using AMCS.Data.Server.Heartbeat;
  using AMCS.Data.Server.Plugin;
  using AMCS.Data.Server.Services;
  using AMCS.Data.Server.Util;
  using AMCS.TestPlugin.Server;
  using AMCS.TestPlugin.Server.Configuration;
  using AMCS.TestPlugin.Server.Services;
  using AMCS.WebDiagnostics;
  using Autofac;
  using AMCS.Data.Server.Glossary;
  using Data.Server.WebHook;
  using Language;

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
      XDocument doc;

      using (var stream = typeof(TestServiceSetup).Assembly.GetManifestResourceStream(typeof(TestServiceSetup).Namespace + ".configuration.xml"))
      {
        doc = XDocument.Load(stream);
      }

      var connectionStrings = GetSection<ConnectionStringsConfigurationSection>("connectionStrings");

      var connectionString = GetConnectionString("TestPluginConnectionString", true);

      var azureBlobStorageConnectionString = ConnectionStringEncryption.DecryptFromConfiguration("AzureBlobStorage");

      var entityTypes = TypeManager.FromApplicationPath(
        "AMCS.TestPlugin.Entity.",
        "AMCS.Data.Entity.",
        "AMCS.TestPlugin.IntegrationTests.");

      var serverTypes = TypeManager.FromApplicationPath(
        "AMCS.TestPlugin.Server.",
        "AMCS.Data.Server.",
        "AMCS.TestPlugin.IntegrationTests.");

      var configuration = new DataConfiguration();

      configuration.SetRestrictionsService(new RestrictionService());

      var testPluginLanguageResources = new TestPluginLanguageResources();
      configuration.SetLocalizationConfiguration(testPluginLanguageResources);

      var projectConfiguration = (IProjectConfiguration)ConfigurationManager.GetSection("amcs");

      configuration.SetServiceRootsConfiguration(
        (IServiceRootsConfiguration)ConfigurationManager.GetSection("amcs.serviceRoots"),
        projectConfiguration.ServiceRootName);

      configuration.ConfigureDataServer(entityTypes, serverTypes);
      configuration.SetProjectConfiguration(
        projectConfiguration);

      var serverConfiguration = (IServerConfiguration)ConfigurationManager.GetSection("amcs.server");
      var testPluginConfiguration =
        (ITestPluginConfiguration)ConfigurationManager.GetSection("amcs.test-plugin");

      configuration.SetServerConfiguration(
        serverConfiguration,
        connectionString,
        null,
        null,
        Jobs.PriorityQueue,
        entityTypes,
        testPluginLanguageResources,
        StrictModeType.None);

      configuration.ConfigureBroadcastService(null, null);

      configuration.ConfigureSingleTenantPluginSystem(
        PluginConstants.VendorId.AMCS,
        "testplugin",
        typeof(ServiceSetup).Assembly.GetName().Version?.ToString(),
        "4C0FFDF1-CC0A-4C1F-A023-7B2C0354AE98",
        "http://localhost:26519/");

      configuration.ConfigurePlatform(
        serverConfiguration.Platform,
        serverConfiguration.PlatformUI);

      configuration.ConfigureTestPlugin(
        testPluginConfiguration,
        connectionString);

      // We use the embedded DateTimeZoneProvider to keep unit tests stable
      configuration.ConfigureTimeZones(new EmbeddedDateTimeZoneProviderProvider(), new SystemNeutralTimeZoneIdProvider());

      configuration.SetDiagnostics();

      configuration.Build();

      CreateBasicData();

      return configuration;

      T GetSection<T>(string name) => (T)Activator.CreateInstance(typeof(T), doc.Root.Element(name));

      IConnectionString GetConnectionString(string name, bool decrypt = false)
      {
        foreach (var item in connectionStrings.ConnectionStrings)
        {
          if (string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase))
          {
            string connectionString = item.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
              return null;
            if (decrypt)
              return ConnectionStringEncryption.Decrypt(connectionString);
            return new ConnectionString(connectionString);
          }
        }

        return null;
      }
    }

    private static void CreateBasicData()
    {
      using (var session = BslDataSessionFactory.GetDataSession())
      using (var transaction = session.CreateTransaction())
      {
        var userId = DataServices.Resolve<IUserService>().SystemUserSessionKey;

        // Check whether there's an admin user.

        if (DataServices.Resolve<TestPlugin.Server.User.IUserService>().GetByName(userId, "admin", session) == null)
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

    private static void SetDiagnostics(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<DiagnosticsManager>()
        .SingleInstance()
        .As<IDiagnosticsManager>();
    }

  }
}
