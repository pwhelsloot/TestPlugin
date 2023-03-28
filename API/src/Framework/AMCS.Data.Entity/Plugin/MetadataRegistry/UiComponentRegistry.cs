using System.Collections.Generic;
using System.Linq;

namespace AMCS.Data.Entity.Plugin.MetadataRegistry
{
  public class UiComponentRegistry
  {
    public List<ShowScreenUiComponent> ShowScreenUiComponents { get; set; } = new List<ShowScreenUiComponent>();

    public AMCS.PluginData.Data.MetadataRegistry.UI.UiComponentRegistry ToPluginData()
    {
      return new AMCS.PluginData.Data.MetadataRegistry.UI.UiComponentRegistry
      {
        UiComponents = ShowScreenUiComponents?
          .Select(uiComponent => uiComponent.ToPluginData())
          .ToList()
      };
    }
  }
}