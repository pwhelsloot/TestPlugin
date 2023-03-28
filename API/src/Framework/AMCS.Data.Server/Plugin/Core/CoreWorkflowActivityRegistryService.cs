namespace AMCS.Data.Server.Plugin.Core
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Threading.Tasks;
  using AMCS.Data.Server.Broadcast;
  using AMCS.Data.Server.Services;
  using AMCS.PluginData.Data.Metadata.MetadataRegistries;
  using AMCS.PluginData.Data.MetadataRegistry.Workflows;
  using Entity.Tenants;
  using log4net;
  using PluginData.Utils;
  using Support;

  public class CoreWorkflowActivityRegistryService : ICoreWorkflowActivityRegistryService
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(CoreWorkflowActivityRegistryService));

    private readonly IBroadcastService broadcastService;
    private readonly ITenantManager tenantManager;
    private readonly ICorePluginHttpService corePluginHttpService;

    private readonly ConcurrentDictionary<string, Dictionary<string, List<WorkflowActivity>>> workflowActivities =
      new ConcurrentDictionary<string, Dictionary<string, List<WorkflowActivity>>>(StringComparer.CurrentCultureIgnoreCase);

    public event EventHandler DataRefreshed;

    public CoreWorkflowActivityRegistryService(
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
    
    public WorkflowActivity GetWorkflowActivity(string tenantId, string workflowActivity, bool autoRefresh)
    {
      var tenantWorkflowActivities = GetTenantWorkflowActivities(tenantId, autoRefresh);

      var pluginActivity = tenantWorkflowActivities?
        .SelectMany(plugin => plugin.Value)
        .SingleOrDefault(component => string.Equals(component.Name, workflowActivity, StringComparison.OrdinalIgnoreCase));

      return pluginActivity;
    }
    
    public IList<WorkflowActivity> GetWorkflowActivities(string tenantId, bool autoRefresh)
    {
      var tenantWorkflowActivities = GetTenantWorkflowActivities(tenantId, autoRefresh);
      
      if (tenantWorkflowActivities == null)
        return new List<WorkflowActivity>();

      var activities = tenantWorkflowActivities
        .SelectMany(plugin => plugin.Value)
        .ToList();

      return activities;
    }

    public IList<WorkflowActivity> GetWorkflowActivities(string tenantId, string plugin, bool autoRefresh)
    {
      var tenantWorkflowActivities = GetTenantWorkflowActivities(tenantId, autoRefresh);
      
      if (tenantWorkflowActivities == null)
        return new List<WorkflowActivity>();

      tenantWorkflowActivities.TryGetValue(plugin, out var activities);
      return activities ?? new List<WorkflowActivity>();
    }
    
    private Dictionary<string, List<WorkflowActivity>> GetTenantWorkflowActivities(string tenantId, bool autoRefresh)
    {
      if (string.IsNullOrWhiteSpace(tenantId))
        throw new ArgumentNullException(nameof(tenantId));

      if (workflowActivities.TryGetValue(tenantId, out var tenantWorkflowActivities))
        return tenantWorkflowActivities;

      if (!autoRefresh)
        return null;
      
      TaskUtils.RunSynchronously(() => RefreshData(tenantId));

      workflowActivities.TryGetValue(tenantId, out tenantWorkflowActivities);
      return tenantWorkflowActivities;
    } 
    
    private async Task AddTenantRegistry(ITenant tenant)
    {
      try
      {
        var metadataRegistry =
          await corePluginHttpService.GetMetadataRegistry<WorkflowActivityRegistry>(
            tenant,
            MetadataRegistryType.WorkflowActivities);

        Logger.Info($"Received {metadataRegistry.WorkflowActivities.Count} workflow activities from tenant {tenant.TenantId}.");
        
        var tenantWorkflowActivities = metadataRegistry.WorkflowActivities
          .GroupBy(uiComponent => PluginMetadataUtility.GetPluginName(uiComponent.Name))
          .ToDictionary(workflowActivity => workflowActivity.Key, workflowActivity => workflowActivity.ToList(), StringComparer.OrdinalIgnoreCase);
        
        workflowActivities.AddOrUpdate(tenant.TenantId,
          tenantId => tenantWorkflowActivities,
          (tenantId, components) => tenantWorkflowActivities);
      }
      catch (Exception ex)
      {
        Logger.Error($"There was an error getting workflow activities from tenant {tenant.TenantId}", ex);
      }
    }
    
    private void RaiseRefreshed()
    {
      Logger.Info("Workflow Activity Registry Data Refreshed");
      
      broadcastService.Broadcast(new WorkflowActivityRegistry());
      DataRefreshed?.Invoke(this, EventArgs.Empty);
    }
  }
}