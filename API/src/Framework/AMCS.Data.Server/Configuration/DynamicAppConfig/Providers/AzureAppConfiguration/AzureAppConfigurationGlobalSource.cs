namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class AzureAppConfigurationGlobalSource : IAzureAppConfigurationGlobalSource
  {
    private readonly object sync = new object();

    private readonly Dictionary<string, List<IConfigurationValueUpdater>> updatersByConfigurationKey
      = new Dictionary<string, List<IConfigurationValueUpdater>>();
    private List<string> configurationKeys;

    public Type ConfigurationProviderType => typeof(AzureAppConfigurationGlobalProvider);
    public int Order => 2;

    public void Register<T>(IConfigurationProvider provider, IConfigurationValueUpdater updater)
    {
      string configuration = ((AzureAppConfigurationGlobalProvider)provider).Configuration;

      lock (sync)
      {
        if (updatersByConfigurationKey.TryGetValue(configuration, out var updaters))
        {
          updaters.Add(updater);
        }
        else
        {
          updatersByConfigurationKey.Add(configuration, new List<IConfigurationValueUpdater> { updater });
        }
      }
    }

    public void Update(List<AzureAppConfiguration> globalConfigurations)
    {
      lock (sync)
      {
        var newConfigurationKeys = globalConfigurations.Select(g => g.Key).ToList();
        var oldConfigurationKeys = configurationKeys ?? new List<string>();

        // update any new or existing configurations
        foreach (var globalConfiguration in globalConfigurations)
        {
          if (!updatersByConfigurationKey.TryGetValue(globalConfiguration.Key, out var updaters))
            continue;

          foreach (var updater in updaters)
          {
            updater.SetValue(globalConfiguration.Value);
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

        configurationKeys = newConfigurationKeys;
      }
    }
  }
}