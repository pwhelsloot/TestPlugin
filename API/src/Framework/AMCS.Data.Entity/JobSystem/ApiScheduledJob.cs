using System;
using System.Runtime.Serialization;

namespace AMCS.Data.Entity.JobSystem
{
  [Serializable]
  [EntityTable("ScheduledJob", "ScheduledJobId", SchemaName = "jobs", TrackUpdates = true)]
  public class ApiScheduledJob : EntityObject
  {
    [EntityMember]
    public int? ScheduledJobId { get; set; }

    [EntityMember]
    public string Trigger { get; set; }

    [EntityMember]
    public string FriendlyName { get; set; }

    [EntityMember]
    public string Handler { get; set; }

    [EntityMember]
    public string Parameters { get; set; }

    [EntityMember]
    public string Queue { get; set; }

    [EntityMember]
    public int? Deadline { get; set; }

    [EntityMember]
    public int? PersistenceMode { get; set; }

    [EntityMember]
    public int DuplicateMode { get; set; }

    [EntityMember]
    public string JobState { get; set; }

    public override int? GetId() => ScheduledJobId;
  }
}
