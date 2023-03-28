using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Entity.JobSystem
{
  [EntityTable("NA", "NA")]
  public class ApiJobHandlerProperty : EntityObject
  {
    [EntityMember]
    public string Name { get; set; }

    [EntityMember]
    public string DisplayName { get; set; }

    [EntityMember]
    public bool IsRequired { get; set; }

    [EntityMember]
    public bool IsList { get; set; }

    [EntityMember]
    public string Type { get; set; }

    public override int? GetId() => null;
  }
}
