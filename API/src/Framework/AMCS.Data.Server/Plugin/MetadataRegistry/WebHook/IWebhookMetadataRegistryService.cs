namespace AMCS.Data.Server.Plugin.MetadataRegistry.WebHook
{
  using AMCS.PluginData.Data.MetadataRegistry.WebHooks;

  public interface IWebHookMetadataRegistryService
  {
    WebHookRegistry CreateWebHookMetadataRegistry();
  }
}