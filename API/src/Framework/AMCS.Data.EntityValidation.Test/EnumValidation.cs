using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AMCS.Data.EntityValidation.Test
{
  [TestClass]
  public class EnumValidation : ValidationBase
  {
    [TestMethod]
    public void EnumEquality()
    {
      var obj = new PropertyObject
      {
        EnumProperty = PropertyEnum.B
      };

      Assert.IsTrue(Validate(obj, "EnumProperty == 2"));
      Assert.IsTrue(Validate(obj, "EnumProperty < 3"));
    }

    public enum PropertyEnum
    {
      A = 1,
      B,
      C
    }

    public class PropertyObject
    {
      public PropertyEnum EnumProperty { get; set; }
    }
  }
}
