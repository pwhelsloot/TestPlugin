namespace AMCS.Data.Server.UnitTests
{
  using System;
  using System.Text.RegularExpressions;
  using AMCS.Data.Server.SQL.Querying;
  using NUnit.Framework;

  [TestFixture]
  public class SQLQueryBuilderTests
  {
    [Test]
    public void SimpleQuery()
    {
      var query = Create();

      AssertEqual(
        @"
select field1, field2
from table
where field1 > 0
",
        query);
    }

    [Test]
    public void ExtraWhere()
    {
      var query = Create();

      query.AddWhere("field2 < 0");

      AssertEqual(
        @"
select field1, field2
from table
where field1 > 0 and field2 < 0
",
        query);
    }

    [Test]
    public void ClearedWhere()
    {
      var query = Create();

      query.ClearWhere();

      AssertEqual(
        @"
select field1, field2
from table
",
        query);
    }

    [Test]
    public void OverruledWhere()
    {
      var query = Create();

      query.ResetWhere("field2 < 0");

      AssertEqual(
        @"
select field1, field2
from table
where field2 < 0
",
        query);
    }

    [Test]
    public void ClearThenAdd()
    {
      var query = Create();

      query.ClearSelect();
      query.AddWhere("field2 < 0");
      query.AddSelect("field1");

      AssertEqual(
        @"
select field1
from table
where field1 > 0 and field2 < 0
",
        query);
    }

    [Test]
    public void EnsureGrow()
    {
      var query = Create();

      query.AddWhere("field2 < 0");
      query.AddWhere("field3 < 0");
      query.AddWhere("field4 < 0");
      query.AddWhere("field5 < 0");
      query.AddWhere("field6 < 0");
      query.AddWhere("field7 < 0");

      AssertEqual(
        @"
select field1, field2
from table
where field1 > 0 and field2 < 0 and field3 < 0 and field4 < 0 and field5 < 0 and field6 < 0 and field7 < 0
",
        query);
    }

    private void AssertEqual(string expected, SQLQueryBuilder query)
    {
      expected = expected.Trim();
      string actual = query.ToString().Trim();

      string expectedSimple = Regex.Replace(expected, "\\s+", " ").Trim();
      string actualSimple = Regex.Replace(actual, "\\s+", " ").Trim();

      if(!string.Equals(expectedSimple, actualSimple, StringComparison.OrdinalIgnoreCase))
        Assert.IsTrue(string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase));
    }

    private SQLQueryBuilder Create()
    {
      var query = new SQLQueryBuilder();

      query.SetSelect("field1, field2");
      query.SetFrom("table");
      query.ResetWhere("field1 > 0");

      return query;
    }
  }
}
