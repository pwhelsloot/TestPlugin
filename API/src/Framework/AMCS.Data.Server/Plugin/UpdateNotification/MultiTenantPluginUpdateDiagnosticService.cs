namespace AMCS.Data.Server.Plugin.UpdateNotification
{
  using System;
  using System.Collections.Generic;
  using Core;
  using WebDiagnostics;

  public class MultiTenantPluginUpdateDiagnosticService : IPluginUpdateDiagnosticService
  {
    private readonly IPluginUpdateNotificationService pluginUpdateNotificationService;

    public MultiTenantPluginUpdateDiagnosticService(
      IPluginUpdateNotificationService pluginUpdateNotificationService,
      IDiagnosticsManager diagnostics = null)
    {
      this.pluginUpdateNotificationService = pluginUpdateNotificationService;
      diagnostics?.Register(GetDiagnostics);
    }

    private IEnumerable<DiagnosticResult> GetDiagnostics()
    {
      const string diagnosticName = "Plugin Update";
      
      DataServices.TryResolve<ICoreWebHookRegistryService>(out var coreWebHookRegistryService);
      DataServices.TryResolve<ICoreWorkflowActivityRegistryService>(out var coreWorkflowActivityRegistryService);
      DataServices.TryResolve<ICoreUiComponentRegistryService>(out var coreUiComponentRegistryService);
      
      foreach (var tenantUpdate in pluginUpdateNotificationService.TenantUpdates)
      {
        var webHooks = coreWebHookRegistryService == null 
          ? 0 
          : coreWebHookRegistryService.GetWebHooks(tenantUpdate.Key, false).Count;
        
        var workflowActivities = coreWorkflowActivityRegistryService == null 
          ? 0 
          : coreWorkflowActivityRegistryService.GetWorkflowActivities(tenantUpdate.Key, false).Count;
        
        var uiComponents = coreUiComponentRegistryService == null 
          ? 0 
          : coreUiComponentRegistryService.GetUiComponents(tenantUpdate.Key, false).Count;
        
        var coreUpdates =
          $"({webHooks} Web Hooks, {workflowActivities} Workflow Activities, {uiComponents} UI Components)";
        
        if (tenantUpdate.Value == null)
        {
          yield return new DiagnosticResult.Success($"{diagnosticName} - Tenant ({tenantUpdate.Key})",
            $"Update for tenant succeeded {coreUpdates}");
        }
        else
        {
          yield return new DiagnosticResult.Success($"{diagnosticName} - Tenant ({tenantUpdate.Key})",
            $"Update for tenant failed - {tenantUpdate.Value.Message} {coreUpdates}");
        }
      }
    }
  }
}