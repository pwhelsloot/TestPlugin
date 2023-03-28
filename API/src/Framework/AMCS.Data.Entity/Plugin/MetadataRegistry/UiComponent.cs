namespace AMCS.Data.Entity.Plugin.MetadataRegistry
{
  using System.Collections.Generic;

  public abstract class UiComponent
  {
    public string Name { get; set; }

    public Description Description { get; set; }
    
    public Documentation Documentation { get; set; }
        
    public List<Input> Inputs { get; set; }

    public List<Output> Outputs { get; set; }
  }
}