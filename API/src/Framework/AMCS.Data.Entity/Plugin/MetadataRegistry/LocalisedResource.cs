using System.Collections.Generic;

namespace AMCS.Data.Entity.Plugin.MetadataRegistry
{
  public abstract class LocalisedResource
  {
    public List<LocalisationValue> Values { get; set; } = new List<LocalisationValue>();
  }
}