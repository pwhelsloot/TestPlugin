namespace AMCS.ApiService.Tests
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using Autofac;
  using Configuration.DynamicAppConfig;
  using Data;
  using Data.Configuration;
  using Data.Configuration.TimeZones;
  using Data.Server.Configuration;
  using Data.Server.Configuration.DynamicAppConfig;
  using Data.Server.Configuration.DynamicAppConfig.Providers;
  using Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration;
  using Data.Server.Configuration.DynamicAppConfig.Providers.DefaultValue;
  using Data.Server.Configuration.DynamicAppConfig.Tools;

  public static class TestServiceSetup
  {
    private static readonly Lazy<DataConfiguration> Configuration = new Lazy<DataConfiguration>(DoSetup, LazyThreadSafetyMode.ExecutionAndPublication);

    public static DataConfiguration Setup()
    {
      return Configuration.Value;
    }

    private static DataConfiguration DoSetup()
    {
      var configuration = new DataConfiguration();

      StrictMode.SetStrictMode(StrictModeType.None);
      configuration.ConfigureTimeZones(new EmbeddedDateTimeZoneProviderProvider(), new SystemNeutralTimeZoneIdProvider());
      
      configuration.ConfigureConfigurationServices();

      configuration.Build();

      return configuration;
    }

    public static void ConfigureConfigurationServices(this DataConfiguration self)
    {
      var configurationSources = new List<IBaseConfigurationSource>
      {
        new DefaultValueSource(),
        new AzureAppConfigurationGlobalSource(),
        new AzureAppConfigurationTenantSource()
      };
      
      self.ContainerBuilder
        .Register(context => new ConfigurationService(
          new ConfigurationSourceManager(configurationSources),
          context.Resolve<ISetupService>(),
          TypeManager.FromAssemblies(typeof(TestConfigurations).Assembly).GetTypes().ToArray()))
        .SingleInstance()
        .AutoActivate()
        .AsSelf()
        .As<IConfigurationService>();

      var azureAppConfigurationPrefixes = new List<KeyPrefixConfiguration>
      {
        new KeyPrefixConfiguration { Prefix = "%TENANT_ID%|%PLUGIN_ID%|", Type = "tenant" },
        new KeyPrefixConfiguration { Prefix = "%PLUGIN_ID%|", Type = "global" },
        new KeyPrefixConfiguration { Prefix = "%ENVIRONMENT%|", Type = "global" },
      };

      self.ContainerBuilder
        .Register(context => new AzureAppConfigurationExporter(
          context.Resolve<IConfigurationService>(),
          azureAppConfigurationPrefixes))
        .SingleInstance()
        .AutoActivate()
        .AsSelf();

      self.ContainerBuilder
        .Register(context => new ValidTestConfigurationResolver())
        .SingleInstance()
        .AutoActivate()
        .AsSelf();
    }

    private class KeyPrefixConfiguration : IKeyPrefixConfiguration
    {
      public string Prefix { get; set; }

      public string Type { get; set; }
    }
  }
}