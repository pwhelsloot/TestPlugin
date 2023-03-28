namespace AMCS.Data.Server.Plugin.MetadataRegistry.WorkflowActivity
{
  using System;
  using AMCS.PluginData.Data.MetadataRegistry.Workflows;
  using Workflow;

  public interface IWorkflowActivityMetadataRegistryService
  {
    WorkflowActivityRegistry CreateWorkflowMetadataRegistry();
    void Register(Action<WorkflowActivityRegistryBuilder> builder);
  }
}