using System;
using System.Runtime.Serialization;

namespace AMCS.Data.Entity.JobSystem
{
  [EntityTable("NA", "NA")]
  public class ApiJobCancel : EntityObject
  {
    [EntityMember]
    public Guid JobId { get; set; }

    public override int? GetId() => null;
  }
}
