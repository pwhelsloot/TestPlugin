using System;
using System.Runtime.Serialization;

namespace AMCS.Data.Entity.JobSystem
{
  [EntityTable("JobLog", "JobLogId", SchemaName = "jobs")]
  public class ApiJobLog : EntityObject
  {
    [EntityMember]
    public int? JobLogId { get; set; }

    [EntityMember]
    public int? JobId { get; set; }

    [EntityMember]
    public DateTimeOffset Created { get; set; }

    [EntityMember]
    public int Status { get; set; }

    public override int? GetId() => JobLogId;
  }
}
