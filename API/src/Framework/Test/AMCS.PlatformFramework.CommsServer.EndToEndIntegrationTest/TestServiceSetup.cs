#pragma warning disable CA1506
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using AMCS.ApiService.Configuration;
using AMCS.ApiService.Documentation.Abstractions.Swagger;
using AMCS.ApiService.Documentation.NETCore.Configuration;
using AMCS.Data;
using AMCS.Data.Configuration;
using AMCS.Data.Configuration.Configuration;
using AMCS.Data.Configuration.TimeZones;
using AMCS.Data.Server;
using AMCS.Data.Server.Broadcast;
using AMCS.Data.Server.Configuration;
using AMCS.Data.Server.Services;
using AMCS.Data.Server.Util;
using AMCS.JobSystem.Agent;
using AMCS.PlatformFramework.Entity;
using AMCS.PlatformFramework.Server;
using AMCS.PlatformFramework.Server.ApiDocumentation;
using AMCS.PlatformFramework.Server.Configuration;
using AMCS.PlatformFramework.Server.Services;
using Language;

namespace AMCS.PlatformFramework.CommsServer.EndToEndIntegrationTest
{
  public static class TestServiceSetup
  {
    private static readonly Lazy<DataConfiguration> Configuration = new Lazy<DataConfiguration>(DoSetup, LazyThreadSafetyMode.ExecutionAndPublication);

    public static DataConfiguration Setup()
    {
      return Configuration.Value;
    }

    public static DataConfiguration DoSetup()
    {
      XDocument doc;

      string configurationFilePath = Path.Combine(Path.GetDirectoryName(typeof(TestServiceSetup).Assembly.Location), "configuration.xml");
      using (var stream = File.OpenRead(configurationFilePath))
      {
        doc = XDocument.Load(stream);
      }

      if (doc.Root == null)
        throw new InvalidOperationException("Could not load configuration file");

      var connectionStringsConfiguration =
        new ConnectionStringsConfigurationSection(doc.Root.Element("connectionStrings"));

      var connectionStringDictionary = new Dictionary<string, IConnectionString>();
      var platformFrameworkConnectionString = GetConnectionString("PlatformFrameworkConnectionString", true);
      connectionStringDictionary.Add("PlatformFrameworkConnectionString", platformFrameworkConnectionString);
      var broadcastServiceBus = GetConnectionString("BroadcastServiceBusConnectionString");
      connectionStringDictionary.Add("BroadcastServiceBusConnectionString", broadcastServiceBus);
      var platformFrameworkServiceBus = GetConnectionString("PlatformFrameworkServiceBusConnectionString");
      connectionStringDictionary.Add("PlatformFrameworkServiceBusConnectionString", platformFrameworkServiceBus);

      return Setup(new SetupConfiguration
      {
        ConnectionStrings =
        {
          PlatformFramework = platformFrameworkConnectionString,
          BroadcastServiceBus = broadcastServiceBus,
          PlatformFrameworkServiceBus = platformFrameworkServiceBus
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

    private static DataConfiguration Setup(SetupConfiguration config)
    {
      var entityTypes = TypeManager.FromApplicationPath("AMCS.PlatformFramework.Entity.", "AMCS.Data.Entity.");
      var serverTypes = TypeManager.FromApplicationPath("AMCS.PlatformFramework.Server.", "AMCS.Data.Server.");
      var protocolTypes = TypeManager.FromApplicationPath("AMCS.PlatformFramework.Server.");

      var configuration = new DataConfiguration();

      configuration.SetRestrictionsService(new RestrictionService());

      var platformFrameworkLanguageResources = new PlatformFrameworkLanguageResources();
      configuration.SetLocalizationConfiguration(platformFrameworkLanguageResources);

      configuration.SetConnectionStringsConfiguration(
        config.ConnectionStringDictionary);

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

      configuration.ConfigureApiExplorer(new ApiDocumentationConfiguration
      {
        Versions =
        {
          new ApiDocumentationVersion { Title = "Platform Template API", Version = "platform" },
          new ApiDocumentationVersion { Title = "External API", Version = "external" }
        },
        MarkdownDocumentationLocation = typeof(ApiDocumentation),
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

      if (!string.IsNullOrEmpty(config.ServerConfiguration.CommsServerProtocols.ServiceRoot))
      {
        configuration.ConfigureSetupServices();

        configuration.ConfigureCommsServerProtocols(
          protocolTypes,
          config.ServerConfiguration.CommsServerProtocols,
          config.ServerConfiguration.CommsServer.AllowAlteranteTransport);
      }

      configuration.ConfigureTimeZones(
        new OnlineDateTimeZoneProviderProvider(config.ConnectionStrings.PlatformFramework));

      configuration.SetDiagnostics();

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

    private class SetupConfiguration
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

        public IConnectionString BroadcastServiceBus { get; set; }

        public IConnectionString PlatformFrameworkServiceBus { get; set; }
      }
    }
  }
}
