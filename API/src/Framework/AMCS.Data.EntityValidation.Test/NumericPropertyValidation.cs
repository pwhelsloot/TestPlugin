using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AMCS.Data.EntityValidation.Test
{
  [TestClass]
  public partial class NumericPropertyValidation : ValidationBase
  {
    [TestMethod]
    public void EqualityTests()
    {
      var obj = PropertyObject.WithValue(42);

      Assert.IsTrue(Validate(obj, "SByteProperty == 42"));
      Assert.IsTrue(Validate(obj, "ByteProperty == 42"));
      Assert.IsTrue(Validate(obj, "Int16Property == 42"));
      Assert.IsTrue(Validate(obj, "UInt16Property == 42"));
      Assert.IsTrue(Validate(obj, "Int32Property == 42"));
      Assert.IsTrue(Validate(obj, "UInt32Property == 42"));
      Assert.IsTrue(Validate(obj, "Int64Property == 42"));
      Assert.IsTrue(Validate(obj, "UInt64Property == 42ul"));
      Assert.IsTrue(Validate(obj, "SingleProperty == 42"));
      Assert.IsTrue(Validate(obj, "DoubleProperty == 42"));
      Assert.IsTrue(Validate(obj, "DecimalProperty == 42"));
    }
  }
}
