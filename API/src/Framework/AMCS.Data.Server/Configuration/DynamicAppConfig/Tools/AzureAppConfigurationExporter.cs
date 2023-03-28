namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Tools
{
  using System;
  using System.Collections.Generic;
  using Providers.AzureAppConfiguration;

  public class AzureAppConfigurationExporter
  {
    private readonly IConfigurationService configurationService;
    private readonly IEnumerable<IKeyPrefixConfiguration> azureAppConfigurationPrefixes;

    public AzureAppConfigurationExporter(
      IConfigurationService configurationService,
      IEnumerable<IKeyPrefixConfiguration> azureAppConfigurationPrefixes)
    {
      this.configurationService = configurationService;
      this.azureAppConfigurationPrefixes = azureAppConfigurationPrefixes;
    }

    public AzureAppConfigurations ExportDynamicAppConfigurations()
    {
      var dynamicAppConfigurations = new AzureAppConfigurations();

      var globalConfigurations = AzureAppConfigurationGlobalProvider.GetGlobalConfigurations();
      var tenantConfigurations = AzureAppConfigurationTenantProvider.GetTenantConfigurations();

      foreach (var azureAppConfigurationPrefix in azureAppConfigurationPrefixes)
      {
        var prefixType = AzureAppConfigurationPrefixType.Default;

        if (!string.IsNullOrEmpty(azureAppConfigurationPrefix.Type))
          prefixType = (AzureAppConfigurationPrefixType)Enum.Parse(typeof(AzureAppConfigurationPrefixType), azureAppConfigurationPrefix.Type, true);

        switch (prefixType)
        {
          case AzureAppConfigurationPrefixType.Tenant:
            foreach (var tenantConfiguration in tenantConfigurations)
            {
              var type = configurationService.GetAzureAppConfigurationType(tenantConfiguration);
              if (type == typeof(bool))
              {
                dynamicAppConfigurations.FeatureFlags.Add(new
                {
                  Name = $"{azureAppConfigurationPrefix.Prefix.TrimEnd('|')}|{tenantConfiguration}"
                });
              }
              else
              {
                dynamicAppConfigurations.ConfigurationKeys.Add(new
                {
                  Name = $"{azureAppConfigurationPrefix.Prefix.TrimEnd('|')}|{tenantConfiguration}"
                });
              }
            }

            break;
          case AzureAppConfigurationPrefixType.Global:
            foreach (var globalConfiguration in globalConfigurations)
            {
              var type = configurationService.GetAzureAppConfigurationType(globalConfiguration);

              if (type == typeof(bool))
              {
                dynamicAppConfigurations.FeatureFlags.Add(new
                {
                  Name = $"{azureAppConfigurationPrefix.Prefix.TrimEnd('|')}|{globalConfiguration}"
                });
              }
              else
              {
                dynamicAppConfigurations.ConfigurationKeys.Add(new
                {
                  Name = $"{azureAppConfigurationPrefix.Prefix.TrimEnd('|')}|{globalConfiguration}"
                });
              }
            }

            break;
          default:
            throw new ArgumentOutOfRangeException(prefixType.ToString(), "Prefix type must be 'tenant' or 'global'");
        }
      }

      return dynamicAppConfigurations;
    }
  }
}