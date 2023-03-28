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
  public class ApiJobHandlerParameter : EntityObject
  {
    [EntityMember]
    public string Type { get; set; }

    [EntityMember]
    public List<ApiJobHandlerProperty> Properties { get; set; }

    public override int? GetId() => null;
  }
}
