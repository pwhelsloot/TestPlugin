using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  [Serializable]
  [ApiExplorer(Methods = ApiExplorerMethods.AllQuery)]
  [EntityTable("NA", "NA")]
  public class BslTriggerEnabledEntity : EntityObject
  {
    [EntityMember]
    public string EntityName { get; set; }

    public override int? GetId()
    {
      return null;
    }
  }
}
