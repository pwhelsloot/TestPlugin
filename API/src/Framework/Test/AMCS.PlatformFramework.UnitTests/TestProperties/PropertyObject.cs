using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AMCS.PlatformFramework.UnitTests.TestProperties.EnumsList;

namespace AMCS.PlatformFramework.UnitTests.TestProperties
{
  public class PropertyObject
  {
    public int? IntProperty { get; set; }
    public PropertyEnum? EnumProperty { get; set; }
    public string StringProperty { get; set; }
  }
}
