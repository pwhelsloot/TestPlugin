namespace AMCS.Data.Entity.Plugin.MetadataRegistry
{
  public class LocalisationValue
  {
    public string Language { get; set; }

    public string Text { get; set; }
    
    internal AMCS.PluginData.Data.MetadataRegistry.Shared.LocalisationValue ToPluginData()
    {
      return new AMCS.PluginData.Data.MetadataRegistry.Shared.LocalisationValue
      {
        Language = Language,
        Text = Text
      };
    }
  }
}