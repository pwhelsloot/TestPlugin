using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.Configuration
{
  [Serializable]
  [EntityTable("DBSystemConfiguration", "DbSystemConfigurationId", IdentityInsertMode = IdentityInsertMode.Off)]
  public class DbSystemConfigurationEntity : EntityObject
  {
    [EntityMember]
    public int? DbSystemConfigurationId { get; set; }

    [EntityMember]
    public string Key { get; set; }

    [EntityMember]
    public string Value { get; set; }

    public override int? GetId()
    {
      return DbSystemConfigurationId;
    }
  }
}
