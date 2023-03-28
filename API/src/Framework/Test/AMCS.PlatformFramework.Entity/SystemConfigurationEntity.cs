using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity
{
  [EntityTable("SystemConfiguration", "SystemConfigurationId")]
  [Serializable]
  public class SystemConfigurationEntity : EntityObject
  {
    [EntityMember]
    public int? SystemConfigurationId { get; set; }

    [EntityMember]
    public string Name { get; set; }

    [EntityMember]
    public string Value { get; set; }

    public override int? GetId() => SystemConfigurationId;
  }
}
