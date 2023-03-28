namespace AMCS.Data.Server.Plugin.MetadataRegistry.BusinessObject
{
  using System.Collections.Generic;
  using System.Linq;
  using AMCS.Data.Configuration;
  using AMCS.Data.Entity.Plugin;
  using AMCS.Data.Server.Plugin;
  using AMCS.Data.Server.Services;
  using AMCS.PluginData.Data.Configuration;
  using AMCS.PluginData.Data.Metadata;
  using AMCS.PluginData.Data.Metadata.MetadataRegistries;
  using AMCS.PluginData.Data.MetadataRegistry.BusinessObjects;

  public class BusinessObjectMetadataRegistryService : IBusinessObjectMetadataRegistryService
  {
    private readonly IServiceRootResolver serviceRootResolver;
    private readonly IProjectConfiguration projectConfiguration;
    private readonly IBusinessObjectService businessObjectService;
    private readonly IPluginSystem pluginSystem;

    public BusinessObjectMetadataRegistryService(
      IPluginMetadataService pluginMetadataService,
      IServiceRootResolver serviceRootResolver,
      IProjectConfiguration projectConfiguration,
      IBusinessObjectService businessObjectService,
      IPluginSystem pluginSystem)
    {
      this.serviceRootResolver = serviceRootResolver;
      this.projectConfiguration = projectConfiguration;
      this.businessObjectService = businessObjectService;
      this.pluginSystem = pluginSystem;

      pluginMetadataService.Register(AddMetadataRegistry);
    }

    private void AddMetadataRegistry(string tenantId, PluginMetadata pluginMetadata, IList<PluginDependency> dependencies)
    {
      var serviceRoot = serviceRootResolver.GetServiceRoot(projectConfiguration.ServiceRootName);

      pluginMetadata.MetadataRegistries.Add(new MetadataRegistry
      {
        Type = MetadataRegistryType.BusinessObjects,
        Url = $"{serviceRoot.TrimEnd('/')}/metadata/business-objects.xml"
      });
    }

    public BusinessObjectRegistry CreateBusinessObjectMetadataRegistry()
    {
      var businessObjects = businessObjectService
        .GetAll()
        .Select(businessObject => new BusinessObject
        {
          Name = $"{pluginSystem.FullyQualifiedName}:{businessObject.BusinessObject.Name}",
          AllowUserDefinedFields = businessObject.BusinessObject.AllowUserDefinedFields
        });

      var businessObjectRegistry = new BusinessObjectRegistry();

      businessObjectRegistry.BusinessObjects.AddRange(businessObjects);

      return businessObjectRegistry;
    }
  }
}