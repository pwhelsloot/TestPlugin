namespace AMCS.Data.Server.Plugin.MetadataRegistry.UiComponent
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Reflection;
  using System.Threading.Tasks;
  using AMCS.PluginData.Data.Configuration;
  using Configuration;
  using Data.Configuration;
  using Newtonsoft.Json;
  using PluginData.Data.Metadata;
  using PluginData.Data.Metadata.MetadataRegistries;
  using PluginData.Data.MetadataRegistry.UI;

  public class UiComponentsMetadataRegistryService : IUiComponentsMetadataRegistryService
  {
    private readonly IProjectConfiguration projectConfiguration;
    private readonly IServiceRootResolver serviceRootResolver;
    private readonly IServerConfiguration serverConfiguration;
    private readonly Assembly uiComponentAssembly;
    
    public UiComponentsMetadataRegistryService(
      IPluginMetadataService pluginMetadataService,
      IProjectConfiguration projectConfiguration,
      IServiceRootResolver serviceRootResolver,
      IServerConfiguration serverConfiguration, 
      Assembly uiComponentAssembly)
    {
      this.projectConfiguration = projectConfiguration;
      this.serviceRootResolver = serviceRootResolver;
      this.serverConfiguration = serverConfiguration;
      this.uiComponentAssembly = uiComponentAssembly;

      pluginMetadataService.Register(AddMetadataRegistry);
    }

    private void AddMetadataRegistry(string tenantId, PluginMetadata pluginMetadata, IList<PluginDependency> dependencies)
    {
      if (string.IsNullOrWhiteSpace(serverConfiguration.PluginSystem.MetadataRegistries.UiComponentsMetadata))
        return;

      var serviceRoot = serviceRootResolver.GetServiceRoot(projectConfiguration.ServiceRootName);
      pluginMetadata.MetadataRegistries.Add(new MetadataRegistry
        {
          Type = MetadataRegistryType.UiComponents,
          Url = $"{serviceRoot.TrimEnd('/')}/metadata/ui-components.xml"
        });
    }

    public async Task<UiComponentRegistry> CreateUiComponentRegistry()
    {
      var uiComponentRegistry = await GetUiComponentRegistry();
      return uiComponentRegistry.ToPluginData();
    }

    private async Task<AMCS.Data.Entity.Plugin.MetadataRegistry.UiComponentRegistry> GetUiComponentRegistry()
    {
      using (var stream = uiComponentAssembly
               .GetManifestResourceStream(serverConfiguration.PluginSystem.MetadataRegistries.UiComponentsMetadata))
      {
        if (stream == null)
          throw new InvalidOperationException(
            $"Could not find the ui components json file located at: {serverConfiguration.PluginSystem.MetadataRegistries.UiComponentsMetadata}");

        using (var streamReader = new StreamReader(stream))
        {
          var uiComponentsJson = await streamReader.ReadToEndAsync();
          
          var uiComponentRegistry = JsonConvert
            .DeserializeObject<Entity.Plugin.MetadataRegistry.UiComponentRegistry>(uiComponentsJson);

          return uiComponentRegistry;
        }
      }
    }
  }
}