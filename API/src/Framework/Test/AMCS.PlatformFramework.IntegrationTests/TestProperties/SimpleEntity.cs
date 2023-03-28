using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.PlatformFramework.IntegrationTests.EntityMapping
{
  [EntityTable("NA", "NA")]
  public class SimpleEntity : EntityObject
  {
    [EntityMember]
    public string Value1 { get; set; }

    [EntityMember]
    public int Value2 { get; set; }

    public double Value3 { get; set; }
  }
}
