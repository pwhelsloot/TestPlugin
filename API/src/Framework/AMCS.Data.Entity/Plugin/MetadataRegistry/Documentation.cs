using System.Linq;

namespace AMCS.Data.Entity.Plugin.MetadataRegistry
{
  public class Documentation : LocalisedResource
  {
    internal AMCS.PluginData.Data.MetadataRegistry.Shared.Documentation ToPluginData()
    {
      return new AMCS.PluginData.Data.MetadataRegistry.Shared.Documentation
      {
        Value = Values
          .Select(value => value?.ToPluginData())
          .ToList()
      };
    }
  }
}