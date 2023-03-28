namespace AMCS.Data.Server.Configuration.DynamicAppConfig.Providers.AzureAppConfiguration
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Threading.Tasks;
  using Azure.Helpers;
  using AzureServiceBusSupport;
  using Broadcast;
  using Data.Configuration;
  using Entity.Plugin;
  using log4net;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.Configuration.AzureAppConfiguration;
  using Microsoft.Extensions.Configuration.AzureAppConfiguration.Extensions;
  using Services;

  public class AzureAppConfigurationService : IAzureAppConfigurationService
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(BroadcastService));

    private readonly object syncRoot = new object();
    private readonly IAzureAppConfigurationGlobalSource azureAppConfigurationGlobalSource;
    private readonly IAzureAppConfigurationTenantSource azureAppConfigurationTenantSource;
    private IAzureAppConfigurationTransportFactory appConfigurationTransportFactory;
    private readonly string azureAppConfigurationEndpoint;
    private IAzureAppConfigurationReceiverTransport azureAppConfigurationReceiver;
    private readonly KeyPrefixProvider keyPrefixProvider;

    private IConfiguration configuration;
    private IConfigurationRefresher refresher;
    private ManualResetEventSlim refreshEvent = new ManualResetEventSlim();
    private CancellationTokenSource refreshCancellation = new CancellationTokenSource();
    private bool disposed;

    public AzureAppConfigurationService(IAzureAppConfigurationGlobalSource azureAppConfigurationGlobalSource,
      IAzureAppConfigurationTenantSource azureAppConfigurationTenantSource,
      IAzureAppConfigurationTransportFactory appConfigurationTransportFactory,
      IAzureAppConfigurationConfiguration azureAppConfigurationConfiguration,
      string azureAppConfigurationEndpoint,
      ITenantManager tenantManager,
      IPluginSystem pluginSystem,
      ISetupService setupService)
    {
      this.azureAppConfigurationGlobalSource = azureAppConfigurationGlobalSource;
      this.azureAppConfigurationTenantSource = azureAppConfigurationTenantSource;
      this.appConfigurationTransportFactory = appConfigurationTransportFactory;
      this.azureAppConfigurationEndpoint = azureAppConfigurationEndpoint;
      this.keyPrefixProvider = new KeyPrefixProvider(azureAppConfigurationConfiguration, tenantManager, pluginSystem);

      tenantManager.TenantsChanged += (e) =>
      {
        DoSetup();
        TaskUtils.RunBackground(RefreshConfiguration);
      };

      setupService.RegisterSetup(() =>
      {
        DoSetup();
        OpenReceiver();
        TaskUtils.RunBackground(RefreshConfiguration);
      }, -500);
    }

    private void DoSetup()
    {
      lock (syncRoot)
      {
        keyPrefixProvider.GeneratePrefixes();
        var keyPrefixes = keyPrefixProvider.GetPrefixes();

        configuration = new ConfigurationBuilder().AddAzureAppConfiguration(options =>
        {
          options
            .Connect(azureAppConfigurationEndpoint)
            // feature flags are always prefixed with "featureManagement:" so we need to remove it
            .TrimKeyPrefix("featureManagement:")
            .UseFeatureFlags(featureFlagOptions =>
            {
              foreach (var keyPrefix in keyPrefixes)
              {
                featureFlagOptions.Select($"{keyPrefix.Prefix}*");
              }
            })
            .ConfigureRefresh(refresh =>
            {
              refresh.SetCacheExpiration(TimeSpan.FromDays(7)); // set to a week and ignore

              // register against a sentinel key for each key prefix that we want to subscribe to
              // we listen to this key only and if it changes we refresh all other keys
              // this acts like a 'master' switch
              // alternatively register every key that should be listened to
              foreach (var keyPrefix in keyPrefixes)
              {
                refresh.Register($"{keyPrefix.Prefix}sentinel", true);
              }
            });

          foreach (var keyPrefix in keyPrefixes)
          {
            options.Select($"{keyPrefix.Prefix}*");
          }

          refresher = options.GetRefresher();
        }).Build();
      }
    }

    private void OpenReceiver()
    {
      azureAppConfigurationReceiver = appConfigurationTransportFactory.CreateReceiverTransport();
      azureAppConfigurationReceiver.MessageReceived += ProcessMessagesReceived;
      azureAppConfigurationReceiver.Open();
    }

    private void ProcessMessagesReceived(MessageReceivedEventArgs e)
    {
      e.EventGridEvent.TryCreatePushNotification(out var pushNotification);

      // we have to provide a delay value after which the configuration will be set to dirty and
      // a refresh will take effect. If we don't provide a value, the default of 30 seconds is used so we provide 0 seconds
      // we are aware that DateTime.UtcNow is not guaranteed to increment monotonically and that this may cause missed key updates
      refresher.ProcessPushNotification(pushNotification, TimeSpan.FromSeconds(0));

      TaskUtils.RunBackground(RefreshConfiguration);
    }

    private async Task RefreshConfiguration()
    {
      try
      {
        refreshEvent.Reset();

        // we need to explicitly wait for the configuration to be marked as dirty before attempting to refresh
        // we use 1 second delay above when marking as dirty, so we double that here
        // await Task.Delay(TimeSpan.FromSeconds(2));

        await refresher.RefreshAsync(refreshCancellation.Token);

        if (!refreshCancellation.Token.IsCancellationRequested)
        {
          var globalConfigurations = new List<AzureAppConfiguration>();
          var tenantConfigurations = new Dictionary<string, List<AzureAppConfiguration>>();

          foreach (var keyValuePair in configuration.AsEnumerable())
          {
            if (!string.IsNullOrEmpty(keyValuePair.Key) && keyValuePair.Value != null)
            {
              foreach (var keyPrefix in keyPrefixProvider.GetPrefixes())
              {
                string key = keyValuePair.Key;

                if (key.StartsWith(keyPrefix.Prefix, StringComparison.OrdinalIgnoreCase))
                {
                  // should only ever be one prefix
                  key = key.Substring(keyPrefix.Prefix.Length);
                  switch (keyPrefix.PrefixType)
                  {
                    case AzureAppConfigurationPrefixType.Tenant:
                      // extract tenantId from the prefix so that only the specific tenant is updated
                      var tenantId = keyPrefixProvider.GetTenantId(keyValuePair.Key);

                      if (tenantConfigurations.TryGetValue(tenantId, out List<AzureAppConfiguration> tenantConfiguration))
                      {
                        tenantConfiguration.Add(
                          new AzureAppConfiguration
                          {
                            Key = key,
                            Value = keyValuePair.Value
                          });
                      }
                      else
                      {
                        tenantConfigurations.Add(
                          tenantId,
                          new List<AzureAppConfiguration>
                            {
                              new AzureAppConfiguration {
                                Key = key,
                                Value = keyValuePair.Value
                              }
                            });
                      }
                      break;

                    case AzureAppConfigurationPrefixType.Global:
                      globalConfigurations.Add(
                        new AzureAppConfiguration
                        {
                          Key = key,
                          Value = keyValuePair.Value
                        });
                      break;
                    case AzureAppConfigurationPrefixType.Default:
                    default:
                      throw new ArgumentOutOfRangeException(keyPrefix.PrefixType.ToString(), "Prefix type must be 'tenant' or 'global'");
                  }

                  break;
                }
              }
            }
          }

          azureAppConfigurationGlobalSource?.Update(globalConfigurations);
          azureAppConfigurationTenantSource?.Update(tenantConfigurations);
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex);
      }
      finally
      {
        refreshEvent.Set();
      }
    }

    public void Dispose()
    {
      if (!disposed)
      {
        if (refreshCancellation != null)
        {
          refreshCancellation.Cancel();
          refreshEvent.Wait();
          refreshCancellation.Dispose();
          refreshCancellation = null;
          refreshEvent.Dispose();
          refreshEvent = null;
        }

        if (azureAppConfigurationReceiver != null)
        {
          azureAppConfigurationReceiver.Dispose();
          azureAppConfigurationReceiver = null;
        }

        if (appConfigurationTransportFactory != null)
        {
          appConfigurationTransportFactory.Dispose();
          appConfigurationTransportFactory = null;
        }

        disposed = true;
      }
    }

    private class KeyPrefixProvider
    {
      private readonly object syncRoot = new object();
      private readonly IAzureAppConfigurationConfiguration azureAppConfiguration;
      private readonly ITenantManager tenantManager;
      private readonly IPluginSystem pluginSystem;

      private readonly List<AzureAppConfigurationPrefix> keyPrefixes =
        new List<AzureAppConfigurationPrefix>();

      public KeyPrefixProvider(
        IAzureAppConfigurationConfiguration azureAppConfiguration,
        ITenantManager tenantManager,
        IPluginSystem pluginSystem)
      {
        this.azureAppConfiguration = azureAppConfiguration;
        this.tenantManager = tenantManager;
        this.pluginSystem = pluginSystem;
      }

      public void GeneratePrefixes()
      {
        lock (syncRoot)
        {
          var tenants = tenantManager.GetAllTenants();
          var pluginId = pluginSystem.FullyQualifiedName;

          foreach (var keyPrefix in azureAppConfiguration.GetKeyPrefixes())
          {
            var prefixType = AzureAppConfigurationPrefixType.Default;

            if (!string.IsNullOrEmpty(keyPrefix.Type))
              prefixType = (AzureAppConfigurationPrefixType)Enum.Parse(typeof(AzureAppConfigurationPrefixType), keyPrefix.Type, true);

            foreach (var tenant in tenants)
            {
              var prefix = keyPrefix.Prefix
                .Replace("%TENANT_ID%", tenant.TenantId)
                .Replace("%PLUGIN_ID%", pluginId);

              Regex.Replace(prefix, Regex.Escape("%ENVIRONMENT%"), GetEnvironment);

              prefix = $"{prefix.TrimEnd('|')}|"; // ensure prefixes end with a '|'

              keyPrefixes.Add(new AzureAppConfigurationPrefix(prefix, prefixType));
            }
          }
        }
      }

      private static string GetEnvironment(Match match)
      {
        var environment = AzureHelpers.GetFullSiteName();

        if (!string.IsNullOrEmpty(environment))
          return environment;

        throw new InvalidOperationException($"Failed to replace { match } - could not get environment name from environment variables");
      }

      public List<AzureAppConfigurationPrefix> GetPrefixes()
      {
        lock (syncRoot)
          return keyPrefixes;
      }

      public string GetTenantId(string key)
      {
        // it should be the first part of the prefix split by pipes
        return key.Split('|')[0];
      }
    }
  }
}