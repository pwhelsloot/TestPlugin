using System.Linq;

namespace AMCS.Data.Entity.Plugin.MetadataRegistry
{
  public class ShowScreenUiComponent : UiComponent
  {
    internal AMCS.PluginData.Data.MetadataRegistry.UI.UiComponent ToPluginData()
    {
      return new AMCS.PluginData.Data.MetadataRegistry.UI.ShowScreenUiComponent
      {
        Name = Name,
        Description = Description?.ToPluginData(),
        Documentation = Documentation?.ToPluginData(),
        Inputs = Inputs?
          .Select(input => input.ToPluginData())
          .ToList(),
        Outputs = Outputs?
          .Select(output => output.ToPluginData())
          .ToList()
      };
    }
  }
}