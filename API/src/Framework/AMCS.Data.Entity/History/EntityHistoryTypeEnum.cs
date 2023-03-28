using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.History
{
  [DataContract(Namespace = "http://www.solutionworks.co.uk/elemos")]
  public enum EntityHistoryTypeEnum
  {
    [EnumMember]
    Insert = 1,

    [EnumMember]
    Update = 2,

    [EnumMember]
    Delete = 3,
  }
}
