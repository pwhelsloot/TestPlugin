namespace AMCS.Data.Server.Plugin.Core
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using AMCS.Data.Configuration;
  using AMCS.PluginData.Data.MetadataRegistry.Workflows;
  
  public interface ICoreWorkflowActivityRegistryService : IDelayedStartup
  {
    Task RefreshData();
    Task RefreshData(string tenantId);
    WorkflowActivity GetWorkflowActivity(string tenantId, string workflowActivity, bool autoRefresh);
    IList<WorkflowActivity> GetWorkflowActivities(string tenantId, bool autoRefresh);
    IList<WorkflowActivity> GetWorkflowActivities(string tenantId, string plugin, bool autoRefresh);
  }
}