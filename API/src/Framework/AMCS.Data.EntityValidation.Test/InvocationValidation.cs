using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AMCS.Data.EntityValidation.Test
{
  [TestClass]
  public class InvocationValidation : ValidationBase
  {
    [TestMethod]
    public void StringNullOrEmpty()
    {
      var obj = new PropertyObject();

      Assert.IsTrue(Validate(obj, "String.IsNullOrEmpty(StringProperty)"));
      Assert.IsTrue(Validate(obj, "string.IsNullOrEmpty(StringProperty)"));

      obj.StringProperty = "Hi";

      Assert.IsFalse(Validate(obj, "String.IsNullOrEmpty(StringProperty)"));
      Assert.IsFalse(Validate(obj, "string.IsNullOrEmpty(StringProperty)"));
    }

    [TestMethod]
    public void StringNullOrWhiteSpace()
    {
      var obj = new PropertyObject();

      Assert.IsTrue(Validate(obj, "String.IsNullOrWhiteSpace(StringProperty)"));
      Assert.IsTrue(Validate(obj, "string.IsNullOrWhiteSpace(StringProperty)"));

      obj.StringProperty = " ";

      Assert.IsTrue(Validate(obj, "String.IsNullOrWhiteSpace(StringProperty)"));
      Assert.IsTrue(Validate(obj, "string.IsNullOrWhiteSpace(StringProperty)"));

      obj.StringProperty = "Hi";

      Assert.IsFalse(Validate(obj, "String.IsNullOrWhiteSpace(StringProperty)"));
      Assert.IsFalse(Validate(obj, "string.IsNullOrWhiteSpace(StringProperty)"));
    }

    [TestMethod]
    public void RegexIsMatch()
    {
      var obj = new PropertyObject
      {
        StringProperty = "123"
      };

      Assert.IsTrue(Validate(obj, "Regex.IsMatch(StringProperty, \"^[0-9]+$\")"));
      Assert.IsTrue(Validate(obj, "System.Text.RegularExpressions.Regex.IsMatch(StringProperty, \"^[0-9]+$\")"));

      obj.StringProperty = "Hi";

      Assert.IsFalse(Validate(obj, "Regex.IsMatch(StringProperty, \"^[0-9]+$\")"));
      Assert.IsFalse(Validate(obj, "System.Text.RegularExpressions.Regex.IsMatch(StringProperty, \"^[0-9]+$\")"));
    }

    public class PropertyObject
    {
      public string StringProperty { get; set; }
    }
  }
}
