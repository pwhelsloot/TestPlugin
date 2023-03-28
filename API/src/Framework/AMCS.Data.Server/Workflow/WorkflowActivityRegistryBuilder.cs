namespace AMCS.Data.Server.Workflow
{
  using System;
  using AMCS.PluginData.Data.MetadataRegistry.Workflows;

  public class WorkflowActivityRegistryBuilder : Builder<WorkflowActivityRegistry>
  {
    public WorkflowActivityRegistryBuilder AddRestActivity(
      Func<RestActivityBuilder, RestWorkflowActivity> builder)
    {
      Entity.WorkflowActivities.Add(builder(new RestActivityBuilder()));
      return this;
    }

    public WorkflowActivityRegistryBuilder AddAsyncRestActivity(
      Func<AsyncRestActivityBuilder, AsyncRestWorkflowActivity> builder)
    {
      Entity.WorkflowActivities.Add(builder(new AsyncRestActivityBuilder()));
      return this;
    }
  }
}
