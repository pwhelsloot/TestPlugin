using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.IntegrationTests.EntityMapping
{
  [EntityTable("NA", "NA")]
  public class WithNestedTargetEntity : EntityObject
  {
    [EntityMember]
    public int Value1 { get; set; }

    [EntityMember]
    public WithNestedTargetEntity NestedEntity { get; set; }

    [EntityMember]
    public List<WithNestedTargetEntity> NestedEntityList { get; set; }
  }
}
