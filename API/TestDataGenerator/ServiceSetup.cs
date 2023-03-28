namespace TestDataGenerator
{
  using System.Linq;
  using System.Xml.Linq;
  using AMCS.ApiService;
  using AMCS.ApiService.Configuration;
  using AMCS.Data;
  using AMCS.Data.Cache.Configuration;
  using AMCS.Data.Configuration;
  using AMCS.Data.Configuration.Configuration;
  using AMCS.Data.Server.Configuration;
  using AMCS.JobSystem.Agent;
  using AMCS.TestPlugin.Entity;
  using AMCS.TestPlugin.Server;
  using AMCS.TestPlugin.Server.Configuration;
  using AMCS.TestPlugin.Server.Services;
  using Language;
  using static AMCS.TestPlugin.Server.Services.ServiceSetup;

  public static class ServiceSetup
  {
    public static DataConfiguration Setup(IConnectionString connectionString, XDocument doc)
    {
      var connectionStringsConfiguration =
        new ConnectionStringsConfigurationSection(doc.Root.Element("connectionStrings"));

      return DoSetup(new SetupConfiguration
      {
        ConnectionStrings =
        {
          TestPlugin = connectionString,
          JobSystemServiceBus = GetConnectionString("JobSystemServiceBusConnectionString"),
          BroadcastServiceBus = GetConnectionString("BroadcastServiceBusConnectionString"),
          AzureBlobStorage = GetConnectionString("AzureBlobStorage")
        },
        TestPluginConfiguration =
          new TestPluginConfigurationSection(doc.Root.Element("amcs.test-plugin")),
        ServiceRootsConfiguration = new ServiceRootsConfigurationSection(doc.Root.Element("amcs.serviceRoots")),
        CacheConfiguration = new CacheConfigurationSection(doc.Root.Element("amcs.cache")),
        ProjectConfiguration = new ProjectConfigurationSection(doc.Root.Element("amcs")),
        ServerConfiguration = new ServerConfigurationSection(doc.Root.Element("amcs.server")),
        AgentConfiguration = new AgentConfigurationSection(doc.Root.Element("agentConfiguration"))
      });

      IConnectionString GetConnectionString(string name, bool decrypt = false)
      {
        string connectionString = connectionStringsConfiguration.ConnectionStrings.FirstOrDefault(p => p.Name == name)
          ?.ConnectionString;
        if (string.IsNullOrEmpty(connectionString))
          return null;
        if (decrypt)
          return ConnectionStringEncryption.Decrypt(connectionString);
        return new ConnectionString(connectionString);
      }
    }

    private static DataConfiguration DoSetup(SetupConfiguration config)
    {
      var entityTypes = TypeManager.FromApplicationPath("AMCS.TestPlugin.Entity.", "AMCS.Data.Entity.");
      var serverTypes = TypeManager.FromApplicationPath("AMCS.TestPlugin.Server.", "AMCS.Data.Server.");

      var configuration = new DataConfiguration();

      configuration.SetRestrictionsService(new RestrictionService());

      var testPluginLanguageResources = new TestPluginLanguageResources();
      configuration.SetLocalizationConfiguration(testPluginLanguageResources);

      configuration.SetConnectionStringsConfiguration(
        config.ConnectionStringDictionary);

      configuration.SetServiceRootsConfiguration(config.ServiceRootsConfiguration, config.ProjectConfiguration.ServiceRootName);

      configuration.SetCacheConfiguration(config.CacheConfiguration);

      configuration.ConfigureDataServer(entityTypes, serverTypes);
      configuration.SetProjectConfiguration(config.ProjectConfiguration);

      configuration.SetServerConfiguration(
        config.ServerConfiguration,
        config.ConnectionStrings.TestPlugin,
        null,
        null,
        Jobs.PriorityQueue,
        entityTypes,
        testPluginLanguageResources,
        StrictModeType.All);

      configuration.SetApiServices(serverTypes);

      configuration.ConfigurePlatform(
        config.ServerConfiguration.Platform,
        config.ServerConfiguration.PlatformUI);

      configuration.ConfigureTestPlugin(
        config.TestPluginConfiguration,
        config.ConnectionStrings.TestPlugin);

      configuration.ConfigureJobSystem(
        config.ServerConfiguration.JobSystem,
        config.AgentConfiguration,
        config.ConnectionStrings.TestPlugin,
        config.ConnectionStrings.JobSystemServiceBus,
        serverTypes);

      configuration.EnsureWebSocketsEnabled();

      configuration.ConfigureJobSystemStatusMonitor();

      configuration.SetDiagnostics();

      configuration.ConfigureSystemConfiguration(typeof(AMCS.TestPlugin.Server.SystemConfiguration.Configuration));

      configuration.SetDiagnosticsRenderer(new PlatformDiagnosticsRenderer());

      configuration.ConfigureDataSets(config.ServerConfiguration, config.ConnectionStrings.AzureBlobStorage, serverTypes);
      configuration.ConfigureTestData(serverTypes);

      configuration.Build();

      return configuration;
    }
  }
}
