#pragma warning disable CA1506
using System.Collections.Generic;
using System.Linq;
#if NETFRAMEWORK
using System.Configuration;
using AMCS.ApiService.Documentation.NETFramework.Configuration;
#endif
#if !NETFRAMEWORK
using System.Xml.Linq;
using AMCS.ApiService.Documentation.NETCore.Configuration;
#endif
using AMCS.ApiService.Configuration;
using AMCS.Data;
using AMCS.Data.Configuration;
using AMCS.Data.Configuration.Configuration;
using AMCS.Data.Configuration.TimeZones;
using AMCS.Data.Entity.Plugin;
using AMCS.Data.Server.Broadcast;
using AMCS.Data.Server.Configuration;
using AMCS.Data.Server.Plugin;
using AMCS.Data.Server.WebHook;
using AMCS.Data.Server.Glossary;
using AMCS.JobSystem.Agent;
using AMCS.PlatformFramework.Server.Configuration;
using Language;
using AMCS.ApiService;
using AMCS.ApiService.Documentation.Abstractions.Swagger;
using AMCS.PlatformFramework.Entity;
using AMCS.PlatformFramework.Server.Plugin;

namespace AMCS.PlatformFramework.Server.Services
{
  public static class ServiceSetup
  {
#if NETFRAMEWORK

    public static DataConfiguration Setup()
    {
      Dictionary<string, IConnectionString> connectionStringDictionary = new Dictionary<string, IConnectionString>();
      var platformFrameworkConnectionString = GetConnectionString("PlatformFrameworkConnectionString", true);
      connectionStringDictionary.Add("PlatformFrameworkConnectionString", platformFrameworkConnectionString);
      var jobSystemServiceBus = GetConnectionString("JobSystemServiceBusConnectionString");
      connectionStringDictionary.Add("JobSystemServiceBusConnectionString", jobSystemServiceBus); 
      var broadcastServiceBus = GetConnectionString("BroadcastServiceBusConnectionString");
      connectionStringDictionary.Add("BroadcastServiceBusConnectionString", broadcastServiceBus);
      var platformFrameworkServiceBus = GetConnectionString("PlatformFrameworkServiceBusConnectionString");
      connectionStringDictionary.Add("PlatformFrameworkServiceBusConnectionString", platformFrameworkServiceBus);

      return Setup(new SetupConfiguration
      {
        ConnectionStrings =
        {
          PlatformFramework = platformFrameworkConnectionString,
          JobSystemServiceBus = jobSystemServiceBus,
          BroadcastServiceBus = broadcastServiceBus,
          PlatformFrameworkServiceBus = platformFrameworkServiceBus,
          AzureBlobStorage = GetConnectionString("AzureBlobStorage")
        },
        ConnectionStringDictionary = connectionStringDictionary,
        PlatformFrameworkConfiguration = (IPlatformFrameworkConfiguration)ConfigurationManager.GetSection("amcs.platform-framework"),
        ServiceRootsConfiguration = (IServiceRootsConfiguration)ConfigurationManager.GetSection("amcs.serviceRoots"),
        ProjectConfiguration = (IProjectConfiguration)ConfigurationManager.GetSection("amcs"),
        ServerConfiguration = (IServerConfiguration)ConfigurationManager.GetSection("amcs.server"),
        AgentConfiguration = (IAgentConfiguration)ConfigurationManager.GetSection("agentConfiguration")
      });

      IConnectionString GetConnectionString(string name, bool decrypt = false)
      {
        var connectionString = ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
        if (string.IsNullOrEmpty(connectionString))
          return null;
        if (decrypt)
          return ConnectionStringEncryption.Decrypt(connectionString);
        return new ConnectionString(connectionString);
      }
    }

#else

    public static DataConfiguration Setup(XDocument doc)
    {
      var connectionStringsConfiguration =
        new ConnectionStringsConfigurationSection(doc.Root.Element("connectionStrings"));

      var connectionStringDictionary = new Dictionary<string, IConnectionString>();
      var platformFrameworkConnectionString = GetConnectionString("PlatformFrameworkConnectionString", true);
      connectionStringDictionary.Add("PlatformFrameworkConnectionString", platformFrameworkConnectionString);
      
      var jobSystemServiceBus = GetConnectionString("JobSystemServiceBusConnectionString");
      connectionStringDictionary.Add("JobSystemServiceBusConnectionString", jobSystemServiceBus);
     
      var broadcastServiceBus = GetConnectionString("BroadcastServiceBusConnectionString");
      connectionStringDictionary.Add("BroadcastServiceBusConnectionString", broadcastServiceBus);
      
      var platformFrameworkServiceBus = GetConnectionString("PlatformFrameworkServiceBusConnectionString");
      connectionStringDictionary.Add("PlatformFrameworkServiceBusConnectionString", platformFrameworkServiceBus);
      
      var azureAppConfigurationEndpoint = GetConnectionString("AzureAppConfigurationEndpointConnectionString");
      connectionStringDictionary.Add("AzureAppConfigurationEndpointConnectionString", azureAppConfigurationEndpoint);
      
      var azureAppConfigurationServiceBus = GetConnectionString("AzureAppConfigurationServiceBusConnectionString");
      connectionStringDictionary.Add("AzureAppConfigurationServiceBusConnectionString", azureAppConfigurationServiceBus);
      
      var azureBlobStorage = GetConnectionString("AzureBlobStorage");
      connectionStringDictionary.Add("AzureBlobStorage", azureBlobStorage);

      return Setup(new SetupConfiguration
      {
        ConnectionStrings =
        {
          PlatformFramework = platformFrameworkConnectionString,
          JobSystemServiceBus = jobSystemServiceBus,
          BroadcastServiceBus = broadcastServiceBus,
          PlatformFrameworkServiceBus = platformFrameworkServiceBus,
          AzureBlobStorage = azureBlobStorage,
          AzureAppConfigurationEndpoint = azureAppConfigurationEndpoint,
          AzureAppConfigurationServiceBus = azureAppConfigurationServiceBus,
        },
        ConnectionStringDictionary = connectionStringDictionary,
        PlatformFrameworkConfiguration =
          new PlatformFrameworkConfigurationSection(doc.Root.Element("amcs.platform-framework")),
        ServiceRootsConfiguration = new ServiceRootsConfigurationSection(doc.Root.Element("amcs.serviceRoots")),
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

#endif

    private static DataConfiguration Setup(SetupConfiguration config)
    {
      var entityTypes = TypeManager.FromApplicationPath("AMCS.PlatformFramework.Entity.", "AMCS.Data.Entity.");
      var serverTypes = TypeManager.FromApplicationPath("AMCS.PlatformFramework.Server.", "AMCS.Data.Server.");
      var protocolTypes = TypeManager.FromApplicationPath("AMCS.PlatformFramework.Server.");

      var configuration = new DataConfiguration();

      configuration.SetRestrictionsService(new RestrictionService());
      configuration.SetSettingsService(new SettingsService());

      var platformFrameworkLanguageResources = new PlatformFrameworkLanguageResources();
      configuration.SetLocalizationConfiguration(platformFrameworkLanguageResources);

      configuration.SetServiceRootsConfiguration(config.ServiceRootsConfiguration, config.ProjectConfiguration.ServiceRootName);

      configuration.ConfigureDataServer(entityTypes, serverTypes);
      configuration.SetProjectConfiguration(config.ProjectConfiguration);

      configuration.SetServerConfiguration(
        config.ServerConfiguration,
        config.ConnectionStrings.PlatformFramework,
        null,
        null,
        Jobs.PriorityQueue,
        entityTypes,
        platformFrameworkLanguageResources,
        StrictModeType.All);

      var coreServiceRoot = config.ServiceRootsConfiguration.GetServiceRoots()
        .Single(root => root.Name == "CoreServiceRoot").Url;

      configuration.ConfigureSingleTenantPluginSystem(
        PluginConstants.VendorId.AMCS, 
        "platformframework", 
        typeof(ServiceSetup).Assembly.GetName().Version?.ToString(),
        config.PlatformFrameworkConfiguration.TenantId, 
        coreServiceRoot);

      configuration.ConfigureBusinessObjects(typeof(UserEntity).Assembly, entityTypes);
      configuration.ConfigureWebHooks();
      configuration.ConfigurePluginMetadata();

      configuration.ConfigureApiServices(
        serverTypes,
        LoadApiVersions(),
        LoadApiMetadata());

      configuration.ConfigureApiExplorer(new ApiDocumentationConfiguration
      {
        Versions =
        {
          new ApiDocumentationVersion { Title = "Platform Template API", Version = "platform" },
          new ApiDocumentationVersion { Title = "External API", Version = "external" }
        },
        MarkdownDocumentationLocation = typeof(ApiDocumentation.ApiDocumentation),
        ServiceRoot = "PlatformFrameworkServiceRoot",
        ErrorCodeTemplate = "Template.html"
      });

      configuration.ConfigurePlatform(
        config.ServerConfiguration.Platform,
        config.ServerConfiguration.PlatformUI);

      configuration.ConfigurePlatformFramework(
        config.PlatformFrameworkConfiguration,
        config.ConnectionStrings.PlatformFramework);

      configuration.ConfigureBroadcastService(
        config.ServerConfiguration.Broadcast, 
        config.ConnectionStrings.BroadcastServiceBus);
      
      configuration.ConfigureJobSystem(
        config.ServerConfiguration.JobSystem,
        config.AgentConfiguration,
        config.ConnectionStrings.PlatformFramework,
        config.ConnectionStrings.JobSystemServiceBus,
        serverTypes);
      
      configuration.ConfigureBslTriggers(Jobs.PriorityQueue, entityTypes, serverTypes);

      configuration.EnsureWebSocketsEnabled();

      configuration.ConfigureJobSystemStatusMonitor();

      configuration.ConfigureDataSets(config.ServerConfiguration, config.ConnectionStrings.AzureBlobStorage, serverTypes);
      configuration.ConfigureTestData(serverTypes);

      if (!string.IsNullOrEmpty(config.ServerConfiguration.CommsServerProtocols.ServiceRoot))
      {
        configuration.ConfigureCommsServerProtocols(
          protocolTypes,
          config.ServerConfiguration.CommsServerProtocols,
          config.ServerConfiguration.CommsServer.AllowAlteranteTransport);
      }

      configuration.ConfigureTimeZones(
        new OnlineDateTimeZoneProviderProvider(config.ConnectionStrings.PlatformFramework));

      configuration.ConfigureConfigurationServices(
        config.ServerConfiguration.AzureAppConfiguration,
        config.ConnectionStrings.AzureAppConfigurationEndpoint,
        config.ConnectionStrings.AzureAppConfigurationServiceBus,
        typeof(PlatformFrameworkConfigurations));

      configuration.SetConnectionStringsConfiguration(
        config.ConnectionStringDictionary);

      configuration.ConfigureGlossary(coreServiceRoot, config.PlatformFrameworkConfiguration.TenantId);
      
      configuration.SetDiagnostics();

      configuration.ConfigureSystemConfiguration(typeof(SystemConfiguration.Configuration));

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

      public IPlatformFrameworkConfiguration PlatformFrameworkConfiguration { get; set; }

      public IServiceRootsConfiguration ServiceRootsConfiguration { get; set; }

      public IProjectConfiguration ProjectConfiguration { get; set; }

      public IServerConfiguration ServerConfiguration { get; set; }

      public IAgentConfiguration AgentConfiguration { get; set; }

      public class ConnectionStringsConfiguration
      {
        public IConnectionString PlatformFramework { get; set; }

        public IConnectionString JobSystemServiceBus { get; set; }
        
        public IConnectionString BroadcastServiceBus { get; set; }
        
        public IConnectionString PlatformFrameworkServiceBus { get; set; }

        public IConnectionString AzureBlobStorage { get; set; }

        public IConnectionString AzureAppConfigurationEndpoint { get; set; }

        public IConnectionString AzureAppConfigurationServiceBus { get; set; }
      }
    }
  }
}
