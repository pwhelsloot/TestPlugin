namespace AMCS.PlatformFramework.Server.Configuration
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;
  using AMCS.Data.Configuration;
  using AMCS.Data.Server.Configuration;
  using AMCS.Data.Server.Heartbeat;
  using AMCS.Data.Server.Services;
  using AMCS.PlatformFramework.Entity;
  using AMCS.PlatformFramework.Entity.Api;
  using AMCS.ApiService;
  using AMCS.ApiService.Configuration;
  using AMCS.PlatformFramework.Server.Heartbeat;
  using AMCS.PlatformFramework.Server.Services;
  using AMCS.WebDiagnostics;
  using Autofac;
  using Data.Server.Configuration.DynamicAppConfig;
  using Data.Server.Configuration.DynamicAppConfig.Providers;
  using Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration;
  using Data.Server.Configuration.DynamicAppConfig.Providers.DefaultValue;

  public static partial class DataConfigurationExtensions
  {
    [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Suppressed for the template project only.")]
    public static void ConfigurePlatformFramework(this DataConfiguration self, IPlatformFrameworkConfiguration configuration, IConnectionString connectionString)
    {
      self.ContainerBuilder
        .RegisterInstance(configuration)
        .As<IPlatformFrameworkConfiguration>();

      self.ContainerBuilder
        .RegisterType<UserService>()
        .As<IUserService>()
        .SingleInstance();

      self.ConfigureEntityObjectMapper();
    }

    public static void ConfigurePlatform(this DataConfiguration self, IPlatformConfiguration configuration, IPlatformUIConfiguration uiConfiguration)
    {
      self.ContainerBuilder
        .RegisterInstance(configuration)
        .As<IPlatformConfiguration>();

      self.ContainerBuilder
        .RegisterInstance(uiConfiguration)
        .As<IPlatformUIConfiguration>();
    }

    public static void SetApiServices(this DataConfiguration self, TypeManager serverTypes)
    {
      self.ConfigureApiServices(
        serverTypes,
        LoadApiVersions(),
        LoadApiMetadata());
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

    public static void SetDiagnostics(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<DiagnosticsManager>()
        .SingleInstance()
        .As<IDiagnosticsManager>();
    }

    public static void ConfigureEntityObjectMapper(this DataConfiguration self)
    {
      self.ConfigureEntityObjectMapper(builder => builder
        .CreateMap<UserEntity, ApiUserEntity>(p => p
          .Map(nameof(ApiUserEntity.User), cfg => cfg.MapFrom(nameof(UserEntity.UserName))))
        .CreateMap<ApiUserEntity, UserEntity>(p => p
          .Map(nameof(UserEntity.UserName), cfg => cfg.MapFrom(nameof(ApiUserEntity.User))))
        .Build()
      );
    }

    public static void ConfigureDataSets(
      this DataConfiguration self, 
      IServerConfiguration serverConfiguration, 
      IConnectionString azureBlobStorageConnectionString, 
      TypeManager serverTypes)
    {
      self.ConfigureEntityObjectMapper(DataSetsBuildMapper);

      self.ConfigureDataSetManager(
        serverConfiguration.DataSets,
        azureBlobStorageConnectionString,
        serverTypes);
    }

    public static void ConfigureHeartbeat(DataConfiguration configuration)
    {
      configuration.ContainerBuilder
        .RegisterType<ConnectionRegistry>()
        .SingleInstance()
        .As<IConnectionRegistry>();

      configuration.AddHeartbeat();
    }

    public static void ConfigureConfigurationServices(
      this DataConfiguration self, 
      IAzureAppConfigurationConfiguration azureAppConfiguration,
      IConnectionString azureAppConfigurationEndpoint,
      IConnectionString azureAppConfigurationServiceBus,
      params Type[] configurationTypes)
    {
      var configurationSources = new List<IBaseConfigurationSource>
      {
        new DefaultValueSource()
      };

      if (azureAppConfigurationEndpoint != null && azureAppConfigurationServiceBus != null)
      {
        var azureAppConfigurationGlobalSource = new AzureAppConfigurationGlobalSource();
        var azureAppConfigurationTenantSource = new AzureAppConfigurationTenantSource();
        configurationSources.Add(azureAppConfigurationGlobalSource);
        configurationSources.Add(azureAppConfigurationTenantSource);

        self.ConfigureAzureAppConfigurationService(
          azureAppConfigurationGlobalSource,
          azureAppConfigurationTenantSource,
          azureAppConfiguration, 
          azureAppConfigurationEndpoint, 
          azureAppConfigurationServiceBus);
      }

      self.ContainerBuilder
        .Register(context => new ConfigurationService(
          new ConfigurationSourceManager(configurationSources),
          context.Resolve<ISetupService>(),
          configurationTypes))
        .SingleInstance()
        .AutoActivate()
        .AsSelf()
        .As<IConfigurationService>();
    }
  }
}
