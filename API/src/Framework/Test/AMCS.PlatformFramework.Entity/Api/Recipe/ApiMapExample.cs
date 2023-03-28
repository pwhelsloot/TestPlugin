using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity.Api.Recipe
{
  [EntityTable("MapExample", "MapExampleId")]
  public class ApiMapExample : EntityObject
  {
    [EntityMember]
    public int? MapExampleId { get; set; }

    [EntityMember]
    public string Description { get; set; }

    [EntityMember]
    public double Latitude { get; set; }

    [EntityMember]
    public double Longitude { get; set; }

    public override int? GetId()
    {
      return MapExampleId;
    }
  }
}
