using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.Entity
{
  [EntityTable("CustomerSite", "CustomerSiteId")]
  [Serializable]
  public class CustomerSiteEntity : EntityObject
  {
    [EntityMember]
    public int? CustomerSiteId { get; set; }

    [EntityMember]
    public string Name { get; set; }

    [EntityMember]
    public int LocationId { get; set; }

    public override int? GetId() => CustomerSiteId;
  }
}
