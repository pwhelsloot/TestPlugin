namespace AMCS.Data.Entity.Plugin.MetadataRegistry
{
  public abstract class InputOutputBase
  {
    public string Name { get; set; }
    
    public string Type { get; set; }
    
    public bool Required { get; set; }

    public Description Description { get; set; }
    
    public Documentation Documentation { get; set; }
  }
}