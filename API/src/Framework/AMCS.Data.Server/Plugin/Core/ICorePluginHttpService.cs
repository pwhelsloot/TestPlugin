namespace AMCS.Data.Server.Plugin.Core
{
  using System;
  using System.Threading.Tasks;
  using AMCS.Data.Entity.Tenants;
  using AMCS.PluginData.Data.Metadata.MetadataRegistries;
  using AMCS.PluginData.Data.MetadataRegistry;

  public interface ICorePluginHttpService
  {
    Task<T> GetMetadataRegistry<T>(ITenant tenant, MetadataRegistryType metadataRegistryType)
      where T : IMetadataRegistryItem;

    Task CreateWorkflowInstance(
      ITenant tenant,
      string workflowProviderName,
      Guid workflowDefinitionId,
      string userContext,
      string startParameters);
  }
}