namespace AMCS.PlatformFramework.IntegrationTests.Plugin.MetadataRegistry
{
  using System.Collections.Generic;
  using System.Net.Http;
  using AMCS.Data.Entity;
  using AMCS.Data.Server.Workflow;
  using AMCS.PluginData.Data.Metadata;
  using AMCS.PluginData.Data.Metadata.MetadataRegistries;
  using Data.Server.Plugin;
  using Data.Server.Plugin.MetadataRegistry.WorkflowActivity;
  using PluginData.Data.Configuration;

  internal class TestWorkflowActivityMetadataService
  {
    public const string Url = "https://platformframework.com/plugin/metadata/metadataRegistry?type=workflowActivities";

    public TestWorkflowActivityMetadataService(
      IPluginMetadataService pluginMetadataService,
      IWorkflowActivityMetadataRegistryService workflowActivityMetadataRegistryService)
    {
      pluginMetadataService.Register(AddMetadataRegistry);
      workflowActivityMetadataRegistryService.Register(CreateWorkflowMetadataRegistry);
    }

    private static void AddMetadataRegistry(string tenantId, PluginMetadata pluginMetadata, IList<PluginDependency> dependencies)
    {
      pluginMetadata.MetadataRegistries.Add(new MetadataRegistry
      {
        Type = MetadataRegistryType.WorkflowActivities,
        Url = Url
      });
    }

    private static void CreateWorkflowMetadataRegistry(WorkflowActivityRegistryBuilder builder)
    {
      builder
        .AddRestActivity(restBuilder => restBuilder
          .WithName(PluginHelper.MergePluginObjectName("CoreRESTAuthentication"))
          .AddDescription(descriptionBuilder => descriptionBuilder
            .WithText("Authenticates against the core app."))
          .AddEndpoint(endpointBuilder => endpointBuilder
            .WithUrl("http://localhost:26519/authTokens")
            .WithHttpMethod(HttpMethod.Post)
            .JsonRequest("{\"username\": \"admin\",\"password\": \"aB1?abc\"}"))
          .AddInput(inputBuilder => inputBuilder
            .IsRequired()
            .WithName("Username")
            .WithType(DataType.String)
            .AddDescription("Your username or email address."))
          .AddInput(inputBuilder => inputBuilder
            .IsRequired()
            .WithName("Password")
            .WithType(DataType.String)
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("Your AMCS domain password.")))
          .AddOutput(inputBuilder => inputBuilder
            .WithName("IsAuthenticated")
            .WithType(DataType.Bool)
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("If authentication was successful.")))
          .Build())
        .AddRestActivity(restBuilder => restBuilder
          .WithName(PluginHelper.MergePluginObjectName("RestName2"))
          .Build())
        .Build();
    }
  }
}