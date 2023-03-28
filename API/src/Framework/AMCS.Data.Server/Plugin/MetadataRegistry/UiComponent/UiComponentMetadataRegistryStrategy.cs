namespace AMCS.Data.Server.Plugin.MetadataRegistry.UiComponent
{
  using System;
  using System.Threading.Tasks;
  using AMCS.PluginData.Services;

  internal class UiComponentMetadataRegistryStrategy : IMetadataRegistryStrategy
  {
    public async Task<string> GetMetadataRegistryAsXmlAsync()
    {
      if (!DataServices.TryResolve<IUiComponentsMetadataRegistryService>(out var uiComponentsRegistryService))
        throw new InvalidOperationException();

      var uiComponentRegistry = await uiComponentsRegistryService.CreateUiComponentRegistry();

      var xml = DataServices
        .Resolve<IPluginSerializationService>()
        .Serialize(uiComponentRegistry);

      return xml;
    }
  }
}