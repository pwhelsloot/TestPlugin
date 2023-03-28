namespace AMCS.Data.Server.Plugin.MetadataRegistry.UiComponent
{
  using System.Threading.Tasks;
  using AMCS.PluginData.Data.MetadataRegistry.UI;

  public interface IUiComponentsMetadataRegistryService
  {
    Task<UiComponentRegistry> CreateUiComponentRegistry();
  }
}