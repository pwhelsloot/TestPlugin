namespace AMCS.Data.Entity.Plugin.MetadataRegistry
{
  public class Output : InputOutputBase
  {
    internal AMCS.PluginData.Data.MetadataRegistry.Shared.Output ToPluginData()
    {
      return new AMCS.PluginData.Data.MetadataRegistry.Shared.Output
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