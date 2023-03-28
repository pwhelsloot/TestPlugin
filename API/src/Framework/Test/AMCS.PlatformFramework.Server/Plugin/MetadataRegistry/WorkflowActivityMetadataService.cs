namespace AMCS.PlatformFramework.Server.Plugin.MetadataRegistry
{
  using System.Collections.Generic;
  using AMCS.Data.Configuration;
  using AMCS.Data.Entity;
  using AMCS.Data.Server.Plugin;
  using AMCS.Data.Server.Workflow;
  using AMCS.PluginData.Data.Configuration;
  using AMCS.PluginData.Data.Metadata;
  using AMCS.PluginData.Data.Metadata.MetadataRegistries;
  using Data.Server.Plugin.MetadataRegistry.WorkflowActivity;

  public class WorkflowActivityMetadataService
  {
    private readonly IServiceRootResolver serviceRootResolver;

    public WorkflowActivityMetadataService(
      IPluginMetadataService pluginMetadataService,
      IServiceRootResolver serviceRootResolver,
      IWorkflowActivityMetadataRegistryService workflowActivityMetadataRegistryService)
    {
      this.serviceRootResolver = serviceRootResolver;

      pluginMetadataService.Register(AddMetadataRegistry);
      workflowActivityMetadataRegistryService.Register(CreateWorkflowMetadataRegistry);
    }

    private void AddMetadataRegistry(string tenantId, PluginMetadata pluginMetadata, IList<PluginDependency> dependencies)
    {
      pluginMetadata.MetadataRegistries.Add(new MetadataRegistry
      {
        Type = MetadataRegistryType.WorkflowActivities,
        Url = $"{GetServiceRoot()}/metadata/workflow-activities.xml"
      });
    }

    private void CreateWorkflowMetadataRegistry(WorkflowActivityRegistryBuilder builder)
    {
      builder
        .AddRestActivity(restBuilder => restBuilder
          .WithName(PluginHelper.MergePluginObjectName("GetMaterial"))
          .AddDescription(descriptionBuilder => descriptionBuilder
            .WithText("Gets the weight for a material from a floor scale.")
            .Build())
          .AddEndpoint(endpointBuilder => endpointBuilder
            .WithUrl($"{GetServiceRoot()}/services/api/transaction?transactionId={{{{TransactionId}}}}")
            .WithHttpMethod(System.Net.Http.HttpMethod.Get)
            .Build())
          .AddInput(inputBuilder => inputBuilder
            .IsRequired()
            .WithName("TransactionId")
            .WithType(DataType.String)
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The transaction that contains the material.")
              .Build())
            .Build())
          .AddOutput(inputBuilder => inputBuilder
            .WithName("ScaleId")
            .WithType(DataType.Integer)
            .IsRequired()
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The scale where the weight will be taken.")
              .Build())
            .Build())
          .AddOutput(inputBuilder => inputBuilder
            .WithName("MaterialId")
            .WithType(DataType.Integer)
            .IsRequired()
            .AddDescription("The id of the material that is getting weighed.")
            .Build())
          .AddOutput(inputBuilder => inputBuilder
            .WithName("MaterialGradeCode")
            .WithType(DataType.String)
            .IsRequired()
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The grade of quality of the material.")
              .Build())
            .Build())
          .AddOutput(inputBuilder => inputBuilder
            .WithName("Material")
            .WithType(DataType.String)
            .IsRequired()
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The name of the material.")
              .Build())
            .Build())
          .Build())
        .AddRestActivity(restBuilder => restBuilder
          .WithName(PluginHelper.MergePluginObjectName("GetMaterialWeight"))
          .AddDescription(descriptionBuilder => descriptionBuilder
            .WithText("Gets the weight for a material from a floor scale.")
            .Build())
          .AddEndpoint(endpointBuilder => endpointBuilder
            .WithUrl($"{GetServiceRoot()}/services/api/materialWeight?materialId={{{{MaterialId}}}}&scaleId={{{{ScaleId}}}}")
            .WithHttpMethod(System.Net.Http.HttpMethod.Get)
            .Build())
          .AddInput(inputBuilder => inputBuilder
            .IsRequired()
            .WithName("MaterialId")
            .WithType(DataType.Integer)
            .AddDescription("The id of the material that is getting weighed.")
            .Build())
          .AddInput(inputBuilder => inputBuilder
            .IsRequired()
            .WithName("ScaleId")
            .WithType(DataType.Integer)
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The scale where the weight is being taken.")
              .Build())
            .Build())
          .AddOutput(inputBuilder => inputBuilder
            .WithName("MaterialWeight")
            .WithType(DataType.Decimal)
            .IsRequired()
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The weight of the material.")
              .Build())
            .Build())
          .Build())
        .AddRestActivity(restBuilder => restBuilder
          .WithName(PluginHelper.MergePluginObjectName("GetMaterialCost"))
          .AddDescription(descriptionBuilder => descriptionBuilder
            .WithText("Gets the cost of the material for the weight and grade.")
            .Build())
          .AddEndpoint(endpointBuilder => endpointBuilder
            .WithUrl($"{GetServiceRoot()}/services/api/materialCost?materialId={{{{MaterialId}}}}&materialGradeCode={{{{MaterialGradeCode}}}}&materialWeight={{{{MaterialWeight}}}}")
            .WithHttpMethod(System.Net.Http.HttpMethod.Get)
            .Build())
          .AddInput(inputBuilder => inputBuilder
            .IsRequired()
            .WithName("MaterialId")
            .WithType(DataType.Integer)
            .AddDescription("The id of the material that is getting weighed.")
            .Build())
          .AddInput(inputBuilder => inputBuilder
            .IsRequired()
            .WithName("MaterialGradeCode")
            .WithType(DataType.String)
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The grade of quality of the material.")
              .Build())
            .Build())
          .AddInput(inputBuilder => inputBuilder
            .IsRequired()
            .WithName("MaterialWeight")
            .WithType(DataType.Decimal)
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The weight of the material.")
              .Build())
            .Build())
          .AddOutput(inputBuilder => inputBuilder
            .WithName("MaterialCost")
            .WithType(DataType.Decimal)
            .IsRequired()
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The cost of the material per for the grade and weight.")
              .Build())
            .Build())
          .Build())
        .AddRestActivity(restBuilder => restBuilder
          .WithName(PluginHelper.MergePluginObjectName("GenerateUserContext"))
          .AddDescription(descriptionBuilder => descriptionBuilder
            .WithText("Used create a test user context.")
            .Build())
          .AddEndpoint(endpointBuilder => endpointBuilder
            .WithUrl($"{GetServiceRoot()}/services/api/usercontext?email={{{{Email}}}}&userConnectionId={{{{UserConnectionId}}}}")
            .WithHttpMethod(System.Net.Http.HttpMethod.Get)
            .Build())
          .AddInput(inputBuilder => inputBuilder
            .IsRequired()
            .WithName("Email")
            .WithType(DataType.String)
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The email address of the user.")
              .Build())
            .Build())
          .AddInput(inputBuilder => inputBuilder
            .IsRequired()
            .WithName("UserConnectionId")
            .WithType(DataType.String)
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The signalR connection id for the user.")
              .Build())
            .Build())
          .AddOutput(inputBuilder => inputBuilder
            .WithName("__UserContext")
            .WithType(DataType.String)
            .IsRequired()
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The email address of the user.")
              .Build())
            .Build())
          .Build())
        .AddAsyncRestActivity(restBuilder => restBuilder
          .WithName(PluginHelper.MergePluginObjectName("LongRunningProcess"))
          .AddDescription(descriptionBuilder => descriptionBuilder
            .WithText("A simple long running process example which takes an action parameter that will set how this process will end.")
            .Build())
          .AddEndpoint(endpointBuilder => endpointBuilder
            .WithUrl($"{GetServiceRoot()}/services/api/longRunningProcess")
            .WithHttpMethod(System.Net.Http.HttpMethod.Post)
            .Build())
          .AddInput(inputBuilder => inputBuilder
            .IsRequired()
            .WithName("Action")
            .WithType(DataType.String)
            .IsRequired()
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The action that should be taken by this activity.")
              .Build())
            .Build())
          .AddInput(inputBuilder => inputBuilder
            .IsRequired()
            .WithName("NumberOfUpdates")
            .WithType(DataType.Integer)
            .IsRequired()
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The number of times that this activity will post updates via the callback link.")
              .Build())
            .Build())
          .AddOutput(inputBuilder => inputBuilder
            .WithName("ExecutionCount")
            .WithType(DataType.Integer)
            .IsRequired()
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The number of times that this activity updated via the callback link.")
              .Build())
            .Build())
          .AddOutput(inputBuilder => inputBuilder
            .WithName("ExecutionTimeInMilliseconds")
            .WithType(DataType.Decimal)
            .AddDescription(descriptionBuilder => descriptionBuilder
              .WithText("The total execution time of the long running process.")
              .Build())
            .Build())
          .Build())
        .Build();
    }
    
    private string GetServiceRoot() => serviceRootResolver.GetProjectServiceRoot().TrimEnd('/');
  }
}