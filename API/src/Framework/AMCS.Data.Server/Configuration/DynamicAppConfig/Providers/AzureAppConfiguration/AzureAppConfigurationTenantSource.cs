namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class AzureAppConfigurationTenantSource : IAzureAppConfigurationTenantSource
  {
    private readonly object syncRoot = new object();
    private readonly Dictionary<string, List<ITenantConfigurationValueUpdater>> updatersByConfigurationKey
      = new Dictionary<string, List<ITenantConfigurationValueUpdater>>();
    private readonly Dictionary<string, List<string>> configurationsByTenantId = new Dictionary<string, List<string>>();

    public Type ConfigurationProviderType => typeof(AzureAppConfigurationTenantProvider);
    public int Order => 1;

    public void Register<T>(IConfigurationProvider provider, ITenantConfigurationValueUpdater updater)
    {
      string configuration = ((AzureAppConfigurationTenantProvider)provider).Configuration;

      lock (syncRoot)
      {
        if (updatersByConfigurationKey.TryGetValue(configuration, out var updaters))
        {
          updaters.Add(updater);
        }
        else
        {
          updatersByConfigurationKey.Add(configuration, new List<ITenantConfigurationValueUpdater> { updater });
        }
      }
    }

    public void Update(Dictionary<string, List<AzureAppConfiguration>> tenantConfigurations)
    {
      lock (syncRoot)
      {
        var tenants = tenantConfigurations.Select(t => t.Key);
        var seenTenants = new HashSet<string>();

        foreach (var tenant in tenants)
        {
          seenTenants.Add(tenant);

          var newConfigurations = tenantConfigurations[tenant].ToList();
          var newConfigurationKeys = newConfigurations.Select(n => n.Key).ToList();

          if (!configurationsByTenantId.TryGetValue(tenant, out List<string> oldConfigurationKeys))
            oldConfigurationKeys = new List<string>();

          // update any new or existing configurations
          foreach (var newConfiguration in newConfigurations)
          {
            if (!updatersByConfigurationKey.TryGetValue(newConfiguration.Key, out var updaters))
              continue;

            foreach (var updater in updaters)
            {
              updater.SetValue(newConfiguration.Value);
            }
          }

          // clear any deleted configurations
          foreach (var oldConfigurationKey in oldConfigurationKeys.Except(newConfigurationKeys))
          {
            if (!updatersByConfigurationKey.TryGetValue(oldConfigurationKey, out var updaters))
              continue;

            foreach (var updater in updaters)
            {
              updater.ClearValue();
            }
          }

          // Update temp storage
          if (newConfigurations.Any())
            configurationsByTenantId[tenant] = newConfigurationKeys;
          else
            configurationsByTenantId.Remove(tenant);
        }

        // clear any configurations for deleted tenants so that they revert to default or empty values
        foreach (var configurationByTenantId in configurationsByTenantId
                   .Where(c => !seenTenants.Contains(c.Key)))
        {
          foreach (var configuration in configurationByTenantId.Value)
          {
            if (!updatersByConfigurationKey.TryGetValue(configuration, out var updaters))
              continue;

            foreach (var updater in updaters)
            {
              updater.ClearValue(configurationByTenantId.Key);
            }
          }

          configurationsByTenantId.Remove(configurationByTenantId.Key);
        }
      }
    }
  }
}