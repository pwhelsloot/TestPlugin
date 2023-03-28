namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration
{
  using System;
  using AMCS.Data.Server.Azure.Helpers;
  using Autofac;
  using Data.Configuration;
  using Entity.Plugin;
  using Services;

  public static class DataConfigurationExtensions
  {
    public static void ConfigureAzureAppConfigurationService(this DataConfiguration self,
      IAzureAppConfigurationGlobalSource azureAppConfigurationGlobalSource,
      IAzureAppConfigurationTenantSource azureAppConfigurationTenantSource,
      IAzureAppConfigurationConfiguration configuration,
      IConnectionString azureAppConfigurationEndpoint,
      IConnectionString azureAppConfigurationServiceBus)
    {
      IAzureAppConfigurationTransportFactory azureAppConfigurationTransportFactory =
        new AzureAppConfigurationRemoteTransportFactory(new AzureAppConfigurationRemoteTransportFactoryConfiguration
        {
          ConnectionString = azureAppConfigurationServiceBus.GetConnectionString(),
          AutoDeleteOnIdle = TimeSpan.FromHours(1),
          InstanceName = AzureHelpers.GenerateInstanceName(),
          TopicName = configuration.TopicName
        });

      self.ContainerBuilder
        .Register(context => new AzureAppConfigurationService(
          azureAppConfigurationGlobalSource,
          azureAppConfigurationTenantSource,
          azureAppConfigurationTransportFactory,
          configuration,
          azureAppConfigurationEndpoint.GetConnectionString(),
          context.Resolve<ITenantManager>(),
          context.Resolve<IPluginSystem>(),
          context.Resolve<ISetupService>()))
        .SingleInstance()
        .AutoActivate()
        .AsSelf()
        .As<IAzureAppConfigurationService>();
    }
  }
}