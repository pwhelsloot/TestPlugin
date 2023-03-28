using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity.Api.JobSystem
{
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
