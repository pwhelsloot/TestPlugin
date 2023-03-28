using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity.Api.JobSystem
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
