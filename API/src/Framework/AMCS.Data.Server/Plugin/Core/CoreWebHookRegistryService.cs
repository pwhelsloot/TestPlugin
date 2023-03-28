namespace AMCS.Data.Server.Plugin.Core
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Entity.Tenants;
  using AMCS.Data.Support;
  using AMCS.Data.Server.Broadcast;
  using AMCS.Data.Server.Services;
  using AMCS.PluginData.Data.Metadata.MetadataRegistries;
  using AMCS.PluginData.Data.MetadataRegistry.WebHooks;
  using log4net;
  using PluginData.Utils;

  public class CoreWebHookRegistryService : ICoreWebHookRegistryService
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(CoreWebHookRegistryService));

    private readonly IBroadcastService broadcastService;
    private readonly ITenantManager tenantManager;
    private readonly ICorePluginHttpService corePluginHttpService;

    private readonly ConcurrentDictionary<string, Dictionary<string, List<WebHook>>> webHooks =
      new ConcurrentDictionary<string, Dictionary<string, List<WebHook>>>(StringComparer.CurrentCultureIgnoreCase);

    public event EventHandler DataRefreshed;

    public CoreWebHookRegistryService(
      IBroadcastService broadcastService,
      ITenantManager tenantManager, 
      ICorePluginHttpService corePluginHttpService)
    {
      this.broadcastService = broadcastService;
      this.tenantManager = tenantManager;
      this.corePluginHttpService = corePluginHttpService;
    }

    public void Start()
    {
      _ = RefreshData();
    }
    
    public async Task RefreshData()
    {
      var tenants = tenantManager.GetAllTenants();

      foreach (var tenant in tenants)
      {
        await AddTenantRegistry(tenant);
      }

      RaiseRefreshed();
    }
    
    public async Task RefreshData(string tenantId)
    {
      var tenant = tenantManager.GetTenant(tenantId);

      await AddTenantRegistry(tenant);
      RaiseRefreshed();
    }

    public WebHook GetWebHook(string tenantId, string webHook, bool autoRefresh)
    {
      var tenantWebHooks = GetTenantWebHooks(tenantId, autoRefresh);

      var pluginWebHook = tenantWebHooks?
        .SelectMany(plugin => plugin.Value)
        .SingleOrDefault(hook => string.Equals(hook.Name, webHook, StringComparison.OrdinalIgnoreCase));

      return pluginWebHook;
    }
    
    public IList<WebHook> GetWebHooks(string tenantId, bool autoRefresh)
    {
      var tenantWebHooks = GetTenantWebHooks(tenantId, autoRefresh);
      
      if (tenantWebHooks == null)
        return new List<WebHook>();

      var hooks = tenantWebHooks
        .SelectMany(plugin => plugin.Value)
        .ToList();

      return hooks;
    }

    public IList<WebHook> GetWebHooks(string tenantId, string plugin, bool autoRefresh)
    {
      var tenantWebHooks = GetTenantWebHooks(tenantId, autoRefresh);
      
      if (tenantWebHooks == null)
        return new List<WebHook>();

      tenantWebHooks.TryGetValue(plugin, out var hooks);
      return hooks ?? new List<WebHook>();
    }

    private Dictionary<string, List<WebHook>> GetTenantWebHooks(string tenantId, bool autoRefresh)
    {
      if (string.IsNullOrWhiteSpace(tenantId))
        throw new ArgumentNullException(nameof(tenantId));

      if (webHooks.TryGetValue(tenantId, out var tenantWebHooks))
        return tenantWebHooks;

      if (!autoRefresh)
        return null;
      
      TaskUtils.RunSynchronously(() => RefreshData(tenantId));

      webHooks.TryGetValue(tenantId, out tenantWebHooks);
      return tenantWebHooks;
    } 
    
    private async Task AddTenantRegistry(ITenant tenant)
    {
      try
      {
        var metadataRegistry =
          await corePluginHttpService.GetMetadataRegistry<WebHookRegistry>(tenant,
            MetadataRegistryType.WebHooks);

        Logger.Info($"Received {metadataRegistry.WebHooks.Count} web hooks from tenant {tenant.TenantId}.");

        var tenantWebHooks = metadataRegistry.WebHooks
          .GroupBy(webHook => PluginMetadataUtility.GetPluginName(webHook.Name))
          .ToDictionary(webHook => webHook.Key, webHook => webHook.ToList(), StringComparer.OrdinalIgnoreCase);

        webHooks.AddOrUpdate(tenant.TenantId,
          tenantId => tenantWebHooks,
          (tenantId, hooks) => tenantWebHooks);
      }
      catch (Exception ex)
      {
        Logger.Error($"There was an error getting web hooks from tenant {tenant.TenantId}.", ex);
      }
    }
    
    private void RaiseRefreshed()
    {
      Logger.Info("Web Hook Registry Data Refreshed");
      
      broadcastService.Broadcast(new WebHookRegistry());
      DataRefreshed?.Invoke(this, EventArgs.Empty);
    }
  }
}