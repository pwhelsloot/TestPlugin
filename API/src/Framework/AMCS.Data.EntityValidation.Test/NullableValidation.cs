using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AMCS.Data.EntityValidation.Test
{
  [TestClass]
  public class NullableValidation : ValidationBase
  {
    [TestMethod]
    public void NullTests()
    {
      var obj = new PropertyObject();

      Assert.IsTrue(Validate(obj, "IntProperty == null"));
      Assert.IsTrue(Validate(obj, "EnumProperty == null"));
      Assert.IsFalse(Validate(obj, "IntProperty == 42"));
    }

    [TestMethod]
    public void ValueTests()
    {
      var obj = new PropertyObject
      {
        IntProperty = 42,
        EnumProperty = PropertyEnum.B
      };

      Assert.IsFalse(Validate(obj, "IntProperty == null"));
      Assert.IsTrue(Validate(obj, "IntProperty == 42"));
      Assert.IsTrue(Validate(obj, "EnumProperty != null"));
      Assert.IsTrue(Validate(obj, "EnumProperty == 2"));
    }

    public enum PropertyEnum
    {
      A = 1,
      B,
      C
    }

    public class PropertyObject
    {
      public int? IntProperty { get; set; }
      public PropertyEnum? EnumProperty { get; set; }
    }
  }
}
