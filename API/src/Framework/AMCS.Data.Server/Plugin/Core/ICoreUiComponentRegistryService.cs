namespace AMCS.Data.Server.Plugin.Core
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using AMCS.Data.Configuration;
  using AMCS.PluginData.Data.MetadataRegistry.UI;
  
  public interface ICoreUiComponentRegistryService : IDelayedStartup
  {
    Task RefreshData();
    Task RefreshData(string tenantId);
    UiComponent GetUiComponent(string tenantId, string uiComponent, bool autoRefresh);
    IList<UiComponent> GetUiComponents(string tenantId, bool autoRefresh);
    IList<UiComponent> GetUiComponents(string tenantId, string plugin, bool autoRefresh);
  }
}