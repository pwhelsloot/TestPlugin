namespace AMCS.Data.Server.Plugin.MetadataRegistry.WebHook
{
  using System.Collections.Generic;
  using System.Linq;
  using AMCS.Data.Configuration;
  using AMCS.Data.Entity.Plugin;
  using AMCS.Data.Entity.Webhook;
  using AMCS.Data.Server.Plugin;
  using AMCS.Data.Server.Services;
  using AMCS.PluginData.Data.Configuration;
  using AMCS.PluginData.Data.Metadata;
  using AMCS.PluginData.Data.Metadata.MetadataRegistries;
  using AMCS.PluginData.Data.MetadataRegistry.WebHooks;
  using AMCS.PluginData.Data.WebHook;

  public class WebHookMetadataRegistryService : IWebHookMetadataRegistryService
  {
    private readonly IServiceRootResolver serviceRootResolver;
    private readonly IProjectConfiguration projectConfiguration;
    private readonly IBusinessObjectService businessObjectService;
    private readonly IPluginSystem pluginSystem;

    public WebHookMetadataRegistryService(
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
      var serviceRoot = serviceRootResolver
        .GetProjectServiceRoot()
        .TrimEnd('/');
      
      pluginMetadata.MetadataRegistries.Add(new MetadataRegistry
      {
        Type = MetadataRegistryType.WebHooks,
        Url = $"{serviceRoot}/metadata/web-hooks.xml"
      });
    }
    
    public WebHookRegistry CreateWebHookMetadataRegistry()
    {
      var webHooks = businessObjectService
        .GetAll()
        .Where(webHook => webHook.BusinessObject.AllowWebHooks)
        .Select(webHook => new WebHook
        {
          Name = $"{pluginSystem.FullyQualifiedName}:{webHook.BusinessObject.Name}",
          Trigger = WebHookTriggerUtil.PrintTriggers(),
          Format = $"{WebHookFormat.Simple.ToString().ToLower()},{WebHookFormat.Coalesce.ToString().ToLower()}"
        });

      var webHookRegistry = new WebHookRegistry();
      webHookRegistry.WebHooks.AddRange(webHooks);

      return webHookRegistry;
    }
  }
}