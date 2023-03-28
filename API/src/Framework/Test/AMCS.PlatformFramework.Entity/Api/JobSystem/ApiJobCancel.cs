using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity.Api.JobSystem
{
  [EntityTable("NA", "NA")]
  public class ApiJobCancel : EntityObject
  {
    [EntityMember]
    public Guid JobId { get; set; }

    public override int? GetId() => null;
  }
}
