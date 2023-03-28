namespace AMCS.TestPlugin.Server.Services
{
#pragma warning disable CA1506
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml.Linq;
  using AMCS.ApiService;
  using AMCS.ApiService.Configuration;
  using AMCS.ApiService.Documentation.Abstractions.Swagger;
  using AMCS.ApiService.Documentation.NETCore.Configuration;
  using AMCS.Data;
  using AMCS.Data.Configuration;
  using AMCS.Data.Configuration.Configuration;
  using AMCS.Data.Configuration.TimeZones;
  using AMCS.Data.Entity.Plugin;
  using AMCS.Data.Server.Broadcast;
  using AMCS.Data.Server.Configuration;
  using AMCS.Data.Server.Plugin;
  using AMCS.TestPlugin.Server;
  using AMCS.TestPlugin.Server.Configuration;
  using AMCS.TestPlugin.Server.Entity;
  using AMCS.TestPlugin.Server.Plugin;
  using Language;

  public static class ServiceSetup
  {
    public static DataConfiguration Setup(XDocument doc)
    {
      var connectionStringsConfiguration =
        new ConnectionStringsConfigurationSection(doc.Root.Element("connectionStrings"));

      var connectionStringDictionary = new Dictionary<string, IConnectionString>();
      var testPluginConnectionString = GetConnectionString("TestPluginConnectionString", true);
      connectionStringDictionary.Add("TestPluginConnectionString", testPluginConnectionString);

      var broadcastServiceBus = GetConnectionString("BroadcastServiceBusConnectionString");
      connectionStringDictionary.Add("BroadcastServiceBusConnectionString", broadcastServiceBus);
      
      var testPluginServiceBus = GetConnectionString("TestPluginServiceBusConnectionString");
      connectionStringDictionary.Add("TestPluginServiceBusConnectionString", testPluginServiceBus);
      
      var azureBlobStorage = GetConnectionString("AzureBlobStorage");
      connectionStringDictionary.Add("AzureBlobStorage", azureBlobStorage);

      return Setup(new SetupConfiguration
      {
        ConnectionStrings =
        {
          TestPlugin = testPluginConnectionString,
          BroadcastServiceBus = broadcastServiceBus,
          TestPluginServiceBus = testPluginServiceBus,
          AzureBlobStorage = azureBlobStorage
        },
        ConnectionStringDictionary = connectionStringDictionary,
        TestPluginConfiguration =
          new TestPluginConfigurationSection(doc.Root.Element("amcs.test-plugin")),
        ServiceRootsConfiguration = new ServiceRootsConfigurationSection(doc.Root.Element("amcs.serviceRoots")),
        ProjectConfiguration = new ProjectConfigurationSection(doc.Root.Element("amcs")),
        ServerConfiguration = new ServerConfigurationSection(doc.Root.Element("amcs.server"))
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

    private static DataConfiguration Setup(SetupConfiguration config)
    {
      var entityTypes = TypeManager.FromApplicationPath("AMCS.TestPlugin.Server.", "AMCS.Data.Entity.");
      var serverTypes = TypeManager.FromApplicationPath("AMCS.TestPlugin.Server.", "AMCS.Data.Server.");

      var configuration = new DataConfiguration();

      configuration.SetRestrictionsService(new RestrictionService());

      var testPluginLanguageResources = new TestPluginLanguageResources();
      configuration.SetLocalizationConfiguration(testPluginLanguageResources);

      configuration.SetConnectionStringsConfiguration(
        config.ConnectionStringDictionary);

      configuration.SetServiceRootsConfiguration(config.ServiceRootsConfiguration, config.ProjectConfiguration.ServiceRootName);
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

      var coreServiceRoot = config.ServiceRootsConfiguration.GetServiceRoots()
        .Single(root => root.Name == "CoreServiceRoot").Url;

      configuration.ConfigureSingleTenantPluginSystem(
        PluginConstants.VendorId.AMCS, 
        "testplugin", 
        typeof(ServiceSetup).Assembly.GetName().Version?.ToString(),
        config.TestPluginConfiguration.TenantId, 
        coreServiceRoot);

      configuration.ConfigureBusinessObjects(typeof(UserEntity).Assembly, entityTypes);

      configuration.ConfigurePlugin(config.TestPluginConfiguration.TenantId);
      configuration.ConfigurePluginMetadata(config.TestPluginConfiguration.TenantId);

      configuration.ConfigureApiServices(
        serverTypes,
        LoadApiVersions(),
        LoadApiMetadata());

      configuration.ConfigureApiExplorer(new ApiDocumentationConfiguration
      {
        Versions =
        {
          new ApiDocumentationVersion { Title = "TestPlugin API", Version = "platform" },
          new ApiDocumentationVersion { Title = "External API", Version = "external" }
        },
        ServiceRoot = "TestPluginServiceRoot",
        ErrorCodeTemplate = "Template.html"
      });

      configuration.ConfigurePlatform(
        config.ServerConfiguration.Platform,
        config.ServerConfiguration.PlatformUI);

      configuration.ConfigureTestPlugin(
        config.TestPluginConfiguration,
        config.ConnectionStrings.TestPlugin);

      configuration.ConfigureBroadcastService(
        config.ServerConfiguration.Broadcast, 
        config.ConnectionStrings.BroadcastServiceBus);

      configuration.ConfigureTimeZones(
        new OnlineDateTimeZoneProviderProvider(config.ConnectionStrings.TestPlugin));

      configuration.SetConnectionStringsConfiguration(
        config.ConnectionStringDictionary);

      configuration.SetDiagnostics();

      configuration.SetDiagnosticsRenderer(new PlatformDiagnosticsRenderer());

      configuration.Build();

      return configuration;
    }

    private static IControllerProviderFactory LoadApiMetadata()
    {
      return new ResourceControllerProviderFactory(
        typeof(UserEntity).Assembly,
        typeof(ErrorCode).Namespace + ".ApiMetadata.xml");
    }

    private static ApiVersionProvider LoadApiVersions()
    {
      return ApiVersionProvider.LoadVersions(
        typeof(ErrorCode).Assembly,
        typeof(ErrorCode).Namespace + ".ApiMetadata.xml");
    }

    public class SetupConfiguration
    {
      public ConnectionStringsConfiguration ConnectionStrings { get; } = new ConnectionStringsConfiguration();

      public Dictionary<string, IConnectionString> ConnectionStringDictionary { get; set; } = new Dictionary<string, IConnectionString>();

      public ITestPluginConfiguration TestPluginConfiguration { get; set; }

      public IServiceRootsConfiguration ServiceRootsConfiguration { get; set; }

      public IProjectConfiguration ProjectConfiguration { get; set; }

      public IServerConfiguration ServerConfiguration { get; set; }

      public class ConnectionStringsConfiguration
      {
        public IConnectionString TestPlugin { get; set; }

        public IConnectionString BroadcastServiceBus { get; set; }
        
        public IConnectionString TestPluginServiceBus { get; set; }

        public IConnectionString AzureBlobStorage { get; set; }
      }
    }
  }
}
