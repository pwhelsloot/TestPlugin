using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity
{
  [EntityTable("UserGroup", "UserGroupId")]
  [Serializable]
  public class UserGroupEntity : EntityObject
  {
    [EntityMember]
    public int? UserGroupId { get; set; }

    [EntityMember]
    public string Name { get; set; }

    [EntityMember]
    public bool IsAdministrator { get; set; }

    public override int? GetId() => UserGroupId;
  }
}
