using System;
using System.Runtime.Serialization;

namespace AMCS.Data.Entity.JobSystem
{
  [Obsolete("You need to use ApiExecuteScheduledJob instead of ApiJobPost", true)]
  [EntityTable("NA", "NA")]
  public class ApiJobPost : EntityObject
  {
    [EntityMember]
    public string FriendlyName { get; set; }

    [EntityMember]
    public string Handler { get; set; }

    [EntityMember]
    public string Queue { get; set; }

    [EntityMember]
    public string Parameters { get; set; }

    [EntityMember]
    public int? Deadline { get; set; }

    [EntityMember]
    public int PersistenceMode { get; set; }

    [EntityMember]
    public int DuplicateMode { get; set; }

    public override int? GetId() => null;
  }
}