namespace AMCS.PlatformFramework.IntegrationTests.Plugin.MetadataRegistry
{
  using System.IO;
  using System.Threading.Tasks;
  using AMCS.Data.Server.Plugin.MetadataRegistry.UiComponent;
  using AMCS.PluginData.Data.Metadata;
  using AMCS.PluginData.Data.Metadata.MetadataRegistries;
  using AMCS.PluginData.Data.MetadataRegistry.UI;
  using Newtonsoft.Json;

  public class TestUiComponentsMetadataRegistryService : IUiComponentsMetadataRegistryService
  {
    public const string Url = "https://platformframework.com/plugin/metadata/metadataRegistry?type=workflowActivities";

    internal void AddMetadataRegistry(PluginMetadata pluginMetadata)
    {
      pluginMetadata.MetadataRegistries.Add(new MetadataRegistry
      {
        Type = MetadataRegistryType.UiComponents,
        Url = Url
      });
    }

    public Task<UiComponentRegistry> CreateUiComponentRegistry()
    {
      var uiComponentJson = File.ReadAllText(@"Plugin\MetadataRegistry\uiComponents.json");

      var uiComponentRegistry =
        JsonConvert.DeserializeObject<AMCS.Data.Entity.Plugin.MetadataRegistry.UiComponentRegistry>(uiComponentJson);

      return Task.FromResult(uiComponentRegistry.ToPluginData());
    }
  }
}