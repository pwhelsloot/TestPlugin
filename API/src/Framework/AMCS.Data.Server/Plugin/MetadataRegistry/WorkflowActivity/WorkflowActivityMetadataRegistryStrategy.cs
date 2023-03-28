namespace AMCS.Data.Server.Plugin.MetadataRegistry.WorkflowActivity
{
  using System;
  using System.Threading.Tasks;
  using AMCS.PluginData.Services;

  internal class WorkflowActivityMetadataRegistryStrategy : IMetadataRegistryStrategy
  {
    public Task<string> GetMetadataRegistryAsXmlAsync()
    {
      if (!DataServices.TryResolve<IWorkflowActivityMetadataRegistryService>(out var workflowActivityRegistryService))
        throw new InvalidOperationException();

      var workflowActivityRegistry = workflowActivityRegistryService.CreateWorkflowMetadataRegistry();

      var xml = DataServices
        .Resolve<IPluginSerializationService>()
        .Serialize(workflowActivityRegistry);

      return Task.FromResult(xml);
    }
  }
}