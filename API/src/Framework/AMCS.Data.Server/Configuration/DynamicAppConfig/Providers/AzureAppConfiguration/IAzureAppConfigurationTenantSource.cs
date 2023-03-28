namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration
{
  using System.Collections.Generic;

  public interface IAzureAppConfigurationTenantSource : ITenantConfigurationSource
  {
    void Update(Dictionary<string, List<AzureAppConfiguration>> tenantConfigurations);
  }
}