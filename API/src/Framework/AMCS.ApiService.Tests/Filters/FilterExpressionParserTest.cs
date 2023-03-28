using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Elemos;
using AMCS.Data.Server.SQL.Querying;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AMCS.ApiService.Tests.Filters
{
  [TestClass]
  public class FilterExpressionParserTest : BaseTest
  {
    [TestMethod]
    public void Parse()
    {
      Filter.AssertFilter(
        "stringProp in ('abc',   'def',   'ghi') and   boolProp eq   true and   intProp gte   7 and intProp   lt 10   and   " +
        "doubleProp from   8.8   to   65.3 and decimalProp   ne   44.98 and   dateProp from '2017-06-25'   to   '2018-03-19'",
        p => p
          .Add(Expression.In("StringProp", new[] { "abc", "def", "ghi" }))
          .Add(Expression.Eq("BoolProp", true))
          .Add(Expression.Ge("IntProp", 7))
          .Add(Expression.Lt("IntProp", 10))
          .Add(Expression.Ge("DoubleProp", 8.8))
          .Add(Expression.Le("DoubleProp", 65.3))
          .Add(Expression.Ne("DecimalProp", 44.98m))
          .Add(Expression.Ge("DateProp", new DateTime(2017, 6, 25)))
          .Add(Expression.Le("DateProp", new DateTime(2018, 3, 19))));
    }

    [TestMethod]
    public void ParseFail()
    {
      Filter.Fails("stringProp in ('abc',   'def',   'ghi') or boolProp eq true");
    }
  }
}
