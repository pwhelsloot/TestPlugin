namespace AMCS.Data.Server.Configuration.DynamicAppConfig
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.CompilerServices;
  using Data;
  using Data.Configuration;
  using Providers;
  using Providers.AzureAppConfiguration;

  public class ConfigurationService : IConfigurationService
  {
    private static readonly object syncRoot = new object();
    private static ConfigurationService setupInstance;
    private readonly IConfigurationSourceManager configurationSources;
    private readonly List<Configuration> configurations = new List<Configuration>();
    private readonly HashSet<string> registeredConfigurationIds = new HashSet<string>();

    public ConfigurationService(IConfigurationSourceManager configurationSources, ISetupService setupService, Type[] configurationTypes)
    {
      this.configurationSources = configurationSources;

      setupService.RegisterSetup(() =>
      {
        lock (syncRoot)
        {
          setupInstance = this;
        }

        foreach (var type in configurationTypes)
        {
          RunClassConstructors(type);
        }

        lock (syncRoot)
        {
          setupInstance = null;
        }
      }, -1000);
    }

    public List<ConfigValue> GetPublicConfigValues(ISessionToken userId)
    {
      return configurations
        .Where(config => config.ConfigurationVisibility == ConfigurationVisibility.Public)
        .Select(config => new ConfigValue
        {
          ConfigId = config.ConfigId,
          Value = config.GetValueUntyped(userId)
        })
        .ToList();
    }

    public static TenantConfiguration<T> CreateTenant<T>(string configId,
      IValueCombiner<T> valueCombiner,
      ConfigurationVisibility configurationVisibility,
      params IConfigurationProvider[] providers)
    {
      lock (syncRoot)
      {
        if (setupInstance == null)
        {
          throw new InvalidOperationException(
            "ConfigurationService setup needs to be called before accessing dynamic configuration");
        }

        if (setupInstance.configurations.Exists((configuration) => configuration.ConfigId == configId))
        {
          throw new InvalidOperationException($"{configId} has already been added");
        }

        var config = new TenantConfiguration<T>(configId, valueCombiner, configurationVisibility,
          providers, setupInstance.configurationSources);
        setupInstance.configurations.Add(config);
        return config;
      }
    }

    public static Configuration<T> Create<T>(
      string configId,
      IValueCombiner<T> valueCombiner,
      ConfigurationVisibility configurationVisibility,
      params IConfigurationProvider[] providers)
    {
      lock (syncRoot)
      {
        if (setupInstance == null)
        {
          throw new InvalidOperationException(
            "ConfigurationService setup needs to be called before accessing dynamic configuration");
        }

        bool isAdded = setupInstance.registeredConfigurationIds.Add(configId);

        if (!isAdded)
        {
          throw new InvalidOperationException($"{configId} has already been added");
        }

        var config = new Configuration<T>(configId, valueCombiner, configurationVisibility,
          providers, setupInstance.configurationSources);
        setupInstance.configurations.Add(config);
        return config;
      }
    }

    /// <summary>
    /// Run class constructor for a certain type and its NestedTypes recursively
    /// </summary>
    /// <param name="type"></param>
    private void RunClassConstructors(Type type)
    {
      RuntimeHelpers.RunClassConstructor(type.TypeHandle);
      foreach (var nestedType in type.GetNestedTypes())
      {
        RunClassConstructors(nestedType);
      }
    }

    public Type GetAzureAppConfigurationType(string configId)
    {
      Configuration configuration = null;

      foreach (var config in configurations)
      {
        foreach (var provider in config.Providers)
        {
          if (provider is AzureAppConfigurationGlobalProvider globalProvider && globalProvider.Configuration == configId)
          {
            configuration = config;
            break;
          }

          if (provider is AzureAppConfigurationTenantProvider tenantProvider && tenantProvider.Configuration == configId)
          {
            configuration = config;
            break;
          }
        }

        if (configuration != null)
          break;
      }

      if (configuration != null)
      {
        var configTypes = configuration.GetType().GetGenericTypeArguments(typeof(Configuration<>));
        if (configTypes != null)
          return configTypes[0];

        var tenantConfigTypes = configuration.GetType().GetGenericTypeArguments(typeof(TenantConfiguration<>));
        if (tenantConfigTypes != null)
          return tenantConfigTypes[0];
      }

      throw new InvalidOperationException($"Could not find Type for provided configuration: { configId }");
    }
  }

  public class ConfigValue
  {
    public string ConfigId { get; set; }
    public object Value { get; set; }
  }
}
