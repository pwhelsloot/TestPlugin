namespace AMCS.Data.Server.Plugin.Core
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Data.Configuration;
  using PluginData.Data.MetadataRegistry.WebHooks;

  public interface ICoreWebHookRegistryService : IDelayedStartup
  {
    Task RefreshData();
    Task RefreshData(string tenantId);
    WebHook GetWebHook(string tenantId, string webHook, bool autoRefresh);
    IList<WebHook> GetWebHooks(string tenantId, bool autoRefresh);
    IList<WebHook> GetWebHooks(string tenantId, string plugin, bool autoRefresh);
  }
}