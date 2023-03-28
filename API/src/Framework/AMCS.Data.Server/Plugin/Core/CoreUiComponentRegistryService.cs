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
  using AMCS.PluginData.Data.MetadataRegistry.UI;
  using log4net;
  using PluginData.Utils;

  public class CoreUiComponentRegistryService : ICoreUiComponentRegistryService
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(CoreUiComponentRegistryService));

    private readonly IBroadcastService broadcastService;
    private readonly ITenantManager tenantManager;
    private readonly ICorePluginHttpService corePluginHttpService;

    private readonly ConcurrentDictionary<string, Dictionary<string, List<UiComponent>>> uiComponents =
      new ConcurrentDictionary<string, Dictionary<string, List<UiComponent>>>(StringComparer.CurrentCultureIgnoreCase);

    public event EventHandler DataRefreshed;

    public CoreUiComponentRegistryService(
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
    
    public UiComponent GetUiComponent(string tenantId, string uiComponent, bool autoRefresh)
    {
      var tenantUiComponents = GetTenantUiComponents(tenantId, autoRefresh);

      var pluginComponent = tenantUiComponents?
        .SelectMany(plugin => plugin.Value)
        .SingleOrDefault(component => string.Equals(component.Name, uiComponent, StringComparison.OrdinalIgnoreCase));

      return pluginComponent;
    }
    
    public IList<UiComponent> GetUiComponents(string tenantId, bool autoRefresh)
    {
      var tenantUiComponents = GetTenantUiComponents(tenantId, autoRefresh);
      
      if (tenantUiComponents == null)
        return new List<UiComponent>();

      var components = tenantUiComponents
        .SelectMany(plugin => plugin.Value)
        .ToList();

      return components;
    }

    public IList<UiComponent> GetUiComponents(string tenantId, string plugin, bool autoRefresh)
    {
      var tenantUiComponents = GetTenantUiComponents(tenantId, autoRefresh);
      
      if (tenantUiComponents == null)
        return new List<UiComponent>();

      tenantUiComponents.TryGetValue(plugin, out var components);
      return components ?? new List<UiComponent>();
    }
    
    private Dictionary<string, List<UiComponent>> GetTenantUiComponents(string tenantId, bool autoRefresh)
    {
      if (string.IsNullOrWhiteSpace(tenantId))
        throw new ArgumentNullException(nameof(tenantId));

      if (uiComponents.TryGetValue(tenantId, out var tenantUiComponents))
        return tenantUiComponents;

      if (!autoRefresh)
        return null;
      
      TaskUtils.RunSynchronously(() => RefreshData(tenantId));

      uiComponents.TryGetValue(tenantId, out tenantUiComponents);
      return tenantUiComponents;
    } 
    
    private async Task AddTenantRegistry(ITenant tenant)
    {
      try
      {
        var metadataRegistry =
          await corePluginHttpService.GetMetadataRegistry<UiComponentRegistry>(tenant,
            MetadataRegistryType.UiComponents);

        Logger.Info($"Received {metadataRegistry.UiComponents.Count} ui components from tenant {tenant.TenantId}.");
        
        var tenantUiComponents = metadataRegistry.UiComponents
          .GroupBy(uiComponent => PluginMetadataUtility.GetPluginName(uiComponent.Name))
          .ToDictionary(uiComponent => uiComponent.Key, uiComponent => uiComponent.ToList(), StringComparer.OrdinalIgnoreCase);
        
        uiComponents.AddOrUpdate(tenant.TenantId,
          tenantId => tenantUiComponents,
          (tenantId, components) => tenantUiComponents);
      }
      catch (Exception ex)
      {
        Logger.Error($"There was an error getting ui components from tenant {tenant.TenantId}.", ex);
      }
    }

    private void RaiseRefreshed()
    {
      Logger.Info("UI Component Registry Data Refreshed");
      
      broadcastService.Broadcast(new UiComponentRegistry());
      DataRefreshed?.Invoke(this, EventArgs.Empty);
    }
  }
}