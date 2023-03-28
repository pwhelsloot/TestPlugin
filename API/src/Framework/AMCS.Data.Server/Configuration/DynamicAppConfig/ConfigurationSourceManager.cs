namespace AMCS.Data.Server.Configuration.DynamicAppConfig
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using log4net;
  using Providers;

  public class ConfigurationSourceManager : IConfigurationSourceManager
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ConfigurationSourceManager));
    private readonly Dictionary<Type, IBaseConfigurationSource> configurationSources;

    public ConfigurationSourceManager(IList<IBaseConfigurationSource> configurationSources)
    {
      // Group by Source order because it is not guaranteed they are added in the correct order while setting up the ConfigurationSourceManager.
      if (configurationSources.GroupBy((source) => source.Order).Any((source) => source.Count() > 1))
        throw new ArgumentException($"Cannot set sources, duplicate source orders have been found.");

      this.configurationSources = configurationSources
        .ToDictionary(source => source.ConfigurationProviderType, source => source);
    }

    public int? GetOrder(IConfigurationProvider provider)
    {
      return GetConfigurationSource(provider)?.Order;
    }

    public void Register<T>(IConfigurationProvider provider, IConfigurationValueUpdater updater)
    {
	    var source = GetConfigurationSource(provider);
      switch (source)
      {
        case null:
          return;
        case IConfigurationSource globalSource:
          globalSource.Register<T>(provider, updater);
          break;
        default:
          throw new InvalidOperationException($"Attempted to use '{source.GetType()}' is a global source.");
      }
    }

    public void RegisterTenant<T>(IConfigurationProvider provider, ITenantConfigurationValueUpdater updater)
    {
      var source = GetConfigurationSource(provider);
      switch (source)
      {
        case null:
          return;
        case IConfigurationSource globalSource:
          globalSource.Register<T>(provider, updater);
          break;
        case ITenantConfigurationSource tenantSource:
          tenantSource.Register<T>(provider, updater);
          break;
        default:
          // Currently we only support a "normal" Configurationsource and TenantConfigurationSource, we are throwing this message incase a new configurationsource was made but no support for this was made yet, its to let the developer know more work needs to be done.
          throw new InvalidOperationException(
            $"Attempted to register '{source.GetType()}', this is not a global or tenant-based source.");
      }
    }

    private IBaseConfigurationSource GetConfigurationSource(IConfigurationProvider provider)
    {
      if (this.configurationSources.TryGetValue(provider.GetType(), out var source))
        return source;

      Logger.Error(new DynamicAppConfigurationProviderException($"{provider.GetType().Name} has not been registered as a valid source"));

      return null;
    }
  }
}
