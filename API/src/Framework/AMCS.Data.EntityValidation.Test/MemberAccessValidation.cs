using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AMCS.Data.EntityValidation.Test
{
  [TestClass]
  public class MemberAccessValidation : ValidationBase
  {
    [TestMethod]
    public void ValidateStringLength()
    {
      var obj = new PropertyObject
      {
        StringProperty = "Hi"
      };

      Assert.IsTrue(Validate(obj, "StringProperty != null && StringProperty.Length < 10"));
    }

    [TestMethod]
    public void ValidateStringLengthOnNull()
    {
      var obj = new PropertyObject();

      Assert.IsFalse(Validate(obj, "StringProperty != null && StringProperty.Length < 10"));
    }

    public class PropertyObject
    {
      public string StringProperty { get; set; }
    }
  }
}
