using System;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity
{
  [EntityTable("Location", "LocationId")]
  [Serializable]
  public class LocationEntity : EntityObject
  {
    [EntityMember]
    public int? LocationId { get; set; }

    [EntityMember]
    public string Address { get; set; }

    [EntityMember]
    public string TimeZoneId { get; set; }

    public override int? GetId() => LocationId;
  }
}
