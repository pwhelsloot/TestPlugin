using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity.Api.JobSystem
{
  [EntityTable("Job", "JobId", SchemaName = "jobs")]
  public class ApiJob : EntityObject
  {
    [EntityMember]
    public int? JobId { get; set; }

    [EntityMember]
    public string UserId { get; set; }

    [EntityMember]
    public string FriendlyName { get; set; }

    [EntityMember]
    public string Handler { get; set; }

    [EntityMember]
    public string Queue { get; set; }

    [EntityMember]
    public string Parameters { get; set; }

    [EntityMember]
    public DateTimeOffset? Deadline { get; set; }

    [EntityMember]
    public int DuplicateMode { get; set; }

    [EntityMember]
    public DateTimeOffset Created { get; set; }

    [EntityMember]
    public DateTimeOffset Updated { get; set; }

    [EntityMember]
    public int Status { get; set; }

    [EntityMember]
    public string Result { get; set; }

    [EntityMember]
    public string Error { get; set; }

    [EntityMember]
    public string Log { get; set; }

    [EntityMember]
    public string Checkpoint { get; set; }

    [EntityMember]
    public int? Runtime { get; set; }

    [EntityMember]
    public int? ScheduledJobId { get; set; }

    public override int? GetId() => JobId;
  }
}
