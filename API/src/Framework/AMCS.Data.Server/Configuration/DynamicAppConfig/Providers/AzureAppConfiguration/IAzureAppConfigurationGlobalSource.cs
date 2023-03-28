namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration
{
  using System.Collections.Generic;

  public interface IAzureAppConfigurationGlobalSource : IConfigurationSource
  {
    void Update(List<AzureAppConfiguration> globalConfigurations);
  }
}