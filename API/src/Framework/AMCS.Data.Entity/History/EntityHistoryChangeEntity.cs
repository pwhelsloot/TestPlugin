using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.History
{
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class EntityHistoryChange : EntityObject
  {
    [DataMember(Name = "PropertyKey")]
    public string PropertyKey { get; set; }

    [DataMember(Name = "OldValue")]
    public object OldValue { get; set; }

    [DataMember(Name = "NewValue")]
    public object NewValue { get; set; }

    public override string GetTableName()
    {
      return "NA";
    }

    public override string GetKeyName()
    {
      return "NA";
    }

    public override int? GetId()
    {
      return null;
    }
  }
}
