namespace AMCS.Data.Server.Plugin.UpdateNotification
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net.Http;
  using System.Text;
  using System.Threading.Tasks;
  using AMCS.Data.Configuration;
  using AMCS.Data.Entity.Plugin;
  using AMCS.Data.Entity.Tenants;
  using AMCS.Data.Server.Services;
  using AMCS.PluginData.Data.Update;
  using AMCS.PluginData.Services;
  using AzureServiceBusSupport.RetryUtils;
  using Http;
  using log4net;
  using PlatformCredentials;
  using Util;

  public class PluginUpdateNotificationService : IPluginUpdateNotificationService, IDelayedStartup
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(PluginUpdateNotificationService));
    private static readonly BackoffProfile BackoffProfile = new BackoffProfile(TimeSpan.FromSeconds(2), 1, TimeSpan.FromMinutes(1));

    private readonly ITenantManager tenantManager;
    private readonly IPlatformCredentialsTokenManager platformCredentialsTokenManager;
    private readonly IPluginSerializationService serializationService;
    private readonly IPluginSystem pluginSystem;
    private readonly IHttpClient httpClient;

    public ConcurrentDictionary<string, Exception> TenantUpdates { get; } = new ConcurrentDictionary<string, Exception>();

    public PluginUpdateNotificationService(
      ITenantManager tenantManager,
      IPlatformCredentialsTokenManager platformCredentialsTokenManager,
      IPluginSerializationService serializationService,
      IPluginSystem pluginSystem,
      IHttpClient httpClient)
    {
      this.platformCredentialsTokenManager = platformCredentialsTokenManager;
      this.tenantManager = tenantManager;
      this.serializationService = serializationService;
      this.pluginSystem = pluginSystem;
      this.httpClient = httpClient;
    }

    public void Start()
    {
      var tasks = Update();

      // We don't want this to block startup, so run it on a background thread
      TaskUtil.RunBackground(() => Task.WhenAll(tasks));
    }

    public List<Task<bool>> Update(bool force = false)
    {
      var tenants = tenantManager
        .GetAllTenants()
        .ToList();
      
      return Update(tenants, force);
    }
    
    public List<Task<bool>> Update(ITenant tenant, bool force = false) => Update(new[] {tenant}, force);
    
    public List<Task<bool>> Update(IList<ITenant> tenants, bool force = false)
    {
      if (!tenants.Any())
        return new List<Task<bool>>();

      var tasks = tenants.Select(tenant =>
      {
        return RetryPolicy
          .Handle<Exception>(ex =>
          {
            Log.Error("Error when trying to send plugin update", ex);
            TenantUpdates.AddOrUpdate(tenant.TenantId, key => ex, (key, exception) => ex);
          })
          .Backoff(BackoffProfile)
          .RetryAsync(() => SendUpdate(tenant, force));
      }).ToList();

      return tasks;
    }
    
    private async Task SendUpdate(ITenant tenant, bool force)
    {
      var plugin = new PluginUpdate
      {
        Version = pluginSystem.CurrentVersion,
        Plugin = pluginSystem.FullyQualifiedName,
        Force = force,
      };

      var serializedUpdate = serializationService.Serialize(plugin);
      var content = new StringContent(serializedUpdate, Encoding.UTF8, "application/xml");

      var result = await PutAsync(tenant, content);
      
      if (!result.IsSuccessStatusCode)
      {
        var body = await result.Content.ReadAsStringAsync();
        throw new Exception($"Received Status code {result.StatusCode} while trying to send plugin update: {body}");
      }
      
      TenantUpdates.AddOrUpdate(tenant.TenantId, key => null, (key, exception) => null);
    }

    private async Task<HttpResponseMessage> PutAsync(ITenant tenant, HttpContent content)
    {
      var uri = $"{tenant.CoreAppServiceRoot.TrimEnd('/')}/plugins/update";
      var requestMessage =
        GenerateHttpRequestMessage(HttpMethod.Put, uri,
          PluginHelper.GetPluginAppCredentials(pluginSystem.FullyQualifiedName), tenant.TenantId, content);

      return await httpClient.SendAsync(requestMessage);
    }

    private HttpRequestMessage GenerateHttpRequestMessage(HttpMethod method, string requestUri, string credentials,
      string tenantId, HttpContent content)
    {
      if (string.IsNullOrWhiteSpace(credentials))
        throw new ArgumentNullException(nameof(credentials));

      var token = GeneratePlatformCredentialsToken(credentials, tenantId);
      var message = new HttpRequestMessage(method, requestUri)
      {
        Content = content
      };

      message.Headers.Add("Cookie", $"{PlatformCredentials.CookieName}={token}");

      return message;
    }

    private string GeneratePlatformCredentialsToken(string credentials, string tenantId)
    {
      var platformCredentials =
        new PlatformCredentials(credentials, tenantId);
      var token = platformCredentialsTokenManager.Serialize(platformCredentials);

      return token;
    }
  }
}