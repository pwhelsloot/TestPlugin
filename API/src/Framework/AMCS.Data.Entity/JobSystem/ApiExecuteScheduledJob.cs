using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.JobSystem
{
  [EntityTable("NA", "NA")]
  public class ApiExecuteScheduledJob : EntityObject
  {
    [EntityMember]
    public Guid ScheduledJobId { get; set; }

  }
}
