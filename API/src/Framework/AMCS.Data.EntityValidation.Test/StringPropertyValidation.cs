using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AMCS.Data.EntityValidation.Test
{
  [TestClass]
  public class StringPropertyValidation : ValidationBase
  {
    [TestMethod]
    public void ValueTests()
    {
      var obj = new PropertyObject
      {
        StringProperty = "Hi"
      };

      Assert.IsTrue(Validate(obj, "StringProperty == \"Hi\""));
      Assert.IsFalse(Validate(obj, "StringProperty == \"Bye\""));
      Assert.IsTrue(Validate(obj, "StringProperty != null"));
      Assert.IsFalse(Validate(obj, "StringProperty == null"));
    }

    [TestMethod]
    public void NullTests()
    {
      var obj = new PropertyObject();

      Assert.IsTrue(Validate(obj, "StringProperty == null"));
      Assert.IsFalse(Validate(obj, "StringProperty != null"));
    }

    public class PropertyObject
    {
      public string StringProperty { get; set; }
    }
  }
}
