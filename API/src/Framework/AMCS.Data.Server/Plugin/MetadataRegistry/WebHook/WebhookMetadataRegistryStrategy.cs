namespace AMCS.Data.Server.Plugin.MetadataRegistry.WebHook
{
  using System;
  using System.Threading.Tasks;
  using AMCS.PluginData.Services;

  internal class WebHookMetadataRegistryStrategy : IMetadataRegistryStrategy
  {
    public Task<string> GetMetadataRegistryAsXmlAsync()
    {
      if (!DataServices.TryResolve<IWebHookMetadataRegistryService>(out var webHookMetadataRegistryService))
        throw new InvalidOperationException("Web hooks have not been configured for this system");

      var webHookRegistry = webHookMetadataRegistryService.CreateWebHookMetadataRegistry();

      var xml = DataServices
        .Resolve<IPluginSerializationService>()
        .Serialize(webHookRegistry);

      return Task.FromResult(xml);
    }
  }
}