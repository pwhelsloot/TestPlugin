using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AMCS.Data.EntityValidation.Test
{
  [TestClass]
  public class LiteralValidations : ValidationBase
  {
    [TestMethod]
    public void NumericComparison()
    {
      Assert.IsTrue(Validate("1 < 2"));
      Assert.IsFalse(Validate("1 > 2"));
      Assert.IsTrue(Validate("1 <= 2"));
      Assert.IsFalse(Validate("1 >= 2"));
      Assert.IsTrue(Validate("1 != 2"));
      Assert.IsTrue(Validate("1 == 1.0"));
      Assert.IsTrue(Validate("1e1 == 10"));
      Assert.IsTrue(Validate("1.0e1 == 10"));
      Assert.IsTrue(Validate("0.1e2 == 10"));
      Assert.IsTrue(Validate(".1e2 == 10"));
      Assert.IsTrue(Validate("-1 < 1"));
      Assert.IsTrue(Validate("--1 == 1"));
      Assert.IsTrue(Validate("++1 == 1"));
    }

    [TestMethod]
    public void BooleanComparison()
    {
      Assert.IsTrue(Validate("true"));
      Assert.IsFalse(Validate("false"));
      Assert.IsTrue(Validate("!false"));
      Assert.IsTrue(Validate("!!true"));
      Assert.IsTrue(Validate("true && true"));
      Assert.IsFalse(Validate("true && false"));
      Assert.IsFalse(Validate("false && true"));
      Assert.IsTrue(Validate("true || true"));
      Assert.IsTrue(Validate("true || false"));
      Assert.IsTrue(Validate("false || true"));
      Assert.IsFalse(Validate("false || false"));
    }

    [TestMethod]
    public void Grouping()
    {
      Assert.IsTrue(Validate("true && (false || true)"));
      Assert.IsFalse(Validate("(true && false) || false"));
    }

    [TestMethod]
    public void StringComparison()
    {
      Assert.IsTrue(Validate("\"a\" == \"a\""));
      Assert.IsFalse(Validate("\"a\" == \"b\""));
      Assert.IsTrue(Validate("\"a\" != \"b\""));
      Assert.IsFalse(Validate("\"a\" > \"a\""));
      Assert.IsTrue(Validate("\"a\" >= \"a\""));
      Assert.IsFalse(Validate("\"a\" > \"b\""));
      Assert.IsTrue(Validate("\"a\" < \"b\""));
      Assert.IsTrue(Validate("\"a\" <= \"b\""));
    }
  }
}
