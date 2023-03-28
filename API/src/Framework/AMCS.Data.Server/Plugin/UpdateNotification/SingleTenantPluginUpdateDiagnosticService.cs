namespace AMCS.Data.Server.Plugin.UpdateNotification
{
  using System.Collections.Generic;
  using System.Linq;
  using Services;
  using WebDiagnostics;

  public class SingleTenantPluginUpdateDiagnosticService : IPluginUpdateDiagnosticService
  {
    private readonly IPluginUpdateNotificationService pluginUpdateNotificationService;
    private readonly ITenantManager tenantManager;

    public SingleTenantPluginUpdateDiagnosticService(
      IPluginUpdateNotificationService pluginUpdateNotificationService,
      ITenantManager tenantManager,
      IDiagnosticsManager diagnostics = null)
    {
      this.pluginUpdateNotificationService = pluginUpdateNotificationService;
      this.tenantManager = tenantManager;
      diagnostics?.Register(GetDiagnostics);
    }

    private IEnumerable<DiagnosticResult> GetDiagnostics()
    {
      const string diagnosticName = "Plugin Update";

      if (pluginUpdateNotificationService.TenantUpdates.Count == 0)
      {
        var tenants = tenantManager.GetAllTenants();

        if (tenants == null || tenants.Count == 0)
        {
          yield return new DiagnosticResult.Failure(diagnosticName,
            "No tenant has been registered for this single tenant plugin.");
        }
        else if(tenants.Count > 1)
        {
          yield return new DiagnosticResult.Failure(diagnosticName,
            $"This app was registered as a single tenant plugin, however {tenants.Count} tenants have been registered");
        }
        else
        {
          yield return new DiagnosticResult.Failure(diagnosticName,
            $"This plugin failed to update tenant {tenants.Single().TenantId} at startup");
        }
      }
      else
      {
        foreach (var tenantUpdate in pluginUpdateNotificationService.TenantUpdates)
        {
          if (tenantUpdate.Value == null)
          {
            yield return new DiagnosticResult.Success($"{diagnosticName} - Tenant ({tenantUpdate.Key})",
              $"Update for tenant {tenantUpdate.Key} succeeded");
          }
          else
          {
            yield return new DiagnosticResult.Failure($"{diagnosticName} - Tenant ({tenantUpdate.Key})",
              $"Update for tenant {tenantUpdate.Key} failed", tenantUpdate.Value);
          }
        }
      }
    }
  }
}