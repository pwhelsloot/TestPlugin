namespace AMCS.TestPlugin.Server.Plugin.MetadataRegistry
{
  using System;
  using System.Collections.Generic;
  using AMCS.Data;
  using AMCS.Data.Configuration;
  using AMCS.Data.Entity;
  using AMCS.Data.Entity.Plugin;
  using AMCS.Data.Server.Plugin;
  using AMCS.Data.Server.Plugin.MetadataRegistry.WorkflowActivity;
  using AMCS.Data.Server.Workflow;
  using AMCS.PluginData.Data.Configuration;
  using AMCS.PluginData.Data.Metadata;
  using AMCS.PluginData.Data.Metadata.MetadataRegistries;
  using AMCS.PluginData.Data.MetadataRegistry.Workflows;

  public class WorkflowActivityMetadataService
  {
    private readonly string serviceRoot;

    public WorkflowActivityMetadataService(
      IPluginMetadataService pluginMetadataService,
      IProjectConfiguration projectConfiguration,
      IServiceRootResolver serviceRootResolver,
      IWorkflowActivityMetadataRegistryService workflowActivityMetadataRegistryService)
    {
      this.serviceRoot = serviceRootResolver
        .GetServiceRoot(projectConfiguration.ServiceRootName)
        .TrimEnd('/');

      pluginMetadataService.Register(AddMetadataRegistry);
      workflowActivityMetadataRegistryService.Register(CreateWorkflowMetadataRegistry);
    }

    private void AddMetadataRegistry(string tenantId, PluginMetadata pluginMetadata, IList<PluginDependency> dependencies)
    {
      pluginMetadata.MetadataRegistries ??= new List<MetadataRegistry>();

      pluginMetadata.MetadataRegistries.Add(new MetadataRegistry
      {
        Type = MetadataRegistryType.WorkflowActivities,
        Url = $"{this.serviceRoot.TrimEnd('/')}/metadata/workflow-activities.xml"
      });
    }

    private IPluginSystem pluginSystem;
    public IPluginSystem PluginSystem
    {
      get
      {
        if (pluginSystem != null)
          return pluginSystem;

        if (!DataServices.TryResolve(out pluginSystem))
          throw new InvalidOperationException("This application has not been registered as a plugin");

        return pluginSystem;
      }
    }

    private string FullyQualifyName(string name) => $"{PluginSystem.FullyQualifiedName}:{name}";

    public void CreateWorkflowMetadataRegistry(WorkflowActivityRegistryBuilder builder)
    {
      builder
        .AddRestActivity(GetTestMessage)
        .Build();
    }

    private RestWorkflowActivity GetTestMessage(RestActivityBuilder builder)
    {
      builder
          .WithName(FullyQualifyName("GetTestMessage"))
          .AddDescription(descriptionBuilder => descriptionBuilder
            .WithText("Testing a workflow activity in a Test Plugin"))
          .AddEndpoint(endpointBuilder => endpointBuilder
            .WithUrl($"{serviceRoot}/gettestmessage/?deviceId={{{{DeviceId}}}}")
            .WithHttpMethod(System.Net.Http.HttpMethod.Get))
          .AddInput(inputBuilder => inputBuilder
            .IsRequired()
            .WithName("DeviceId")
            .WithType(DataType.String)
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The unique identifier of the device.")))
          .AddOutput(inputBuilder => inputBuilder.WithName("testResponse").WithType(DataType.String)
            .AddDescription(descriptionBuilder => descriptionBuilder.WithText("Returned test message")));

      return builder.Build();
    }
  }
}
