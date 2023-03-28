namespace AMCS.Data.Server.Plugin.MetadataRegistry.WorkflowActivity
{
  using System;
  using System.Collections.Generic;
  using PluginData.Data.MetadataRegistry.Workflows;
  using Workflow;

  public class WorkflowActivityMetadataRegistryService : IWorkflowActivityMetadataRegistryService
  {
    private readonly List<Action<WorkflowActivityRegistryBuilder>> builders = new List<Action<WorkflowActivityRegistryBuilder>>();
    private readonly object syncRoot = new object();

    public WorkflowActivityRegistry CreateWorkflowMetadataRegistry()
    {
      var builder = new WorkflowActivityRegistryBuilder();

      lock (syncRoot)
      {
        foreach (var action in builders)
        {
          action(builder);
        }
      }

      return builder.Build();
    }

    public void Register(Action<WorkflowActivityRegistryBuilder> builder)
    {
      lock (syncRoot)
      {
        builders.Add(builder);
      }
    }
  }
}