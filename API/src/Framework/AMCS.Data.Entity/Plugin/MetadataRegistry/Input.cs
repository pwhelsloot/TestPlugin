namespace AMCS.Data.Entity.Plugin.MetadataRegistry
{
  public class Input : InputOutputBase
  {
    internal AMCS.PluginData.Data.MetadataRegistry.Shared.Input ToPluginData()
    {
      return new AMCS.PluginData.Data.MetadataRegistry.Shared.Input
      {
        Name = Name,
        Type = Type,
        Required = Required,
        Description = Description?.ToPluginData(),
        Documentation = Documentation?.ToPluginData()
      };
    }
  }
}