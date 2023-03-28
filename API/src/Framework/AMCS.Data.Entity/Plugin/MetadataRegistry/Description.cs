using System.Linq;

namespace AMCS.Data.Entity.Plugin.MetadataRegistry
{
  public class Description : LocalisedResource
  {
    internal AMCS.PluginData.Data.MetadataRegistry.Shared.Description ToPluginData()
    {
      return new AMCS.PluginData.Data.MetadataRegistry.Shared.Description
      {
        Value = Values
          .Select(value => value?.ToPluginData())
          .ToList()
      };
    }
  }
}