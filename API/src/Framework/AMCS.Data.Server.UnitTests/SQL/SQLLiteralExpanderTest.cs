namespace AMCS.Data.Server.UnitTests
{
  using System;
  using System.Data.SqlClient;
  using System.Linq;
  using AMCS.Data.Server.SQL;
  using NUnit.Framework;

  [TestFixture]
  public class SQLLiteralExpanderTest
  {
    [Test]
    public void ExpandBool()
    {
      Validate(
        "select * from A where B = 1",
        "select * from A where B = {=B}",
        new SqlParameter("@B", true));
    }

    [Test]
    public void ExpandMultipleBools()
    {
      Validate(
        "select * from A where B = 1 and C = 0",
        "select * from A where B = {=B} and C = {=C}",
        new SqlParameter("@B", true),
        new SqlParameter("@C", false));
    }

    [Test]
    public void ExpandDecimal()
    {
      Validate(
        "select * from A where B = 42.24",
        "select * from A where B = {=B}",
        new SqlParameter("@B", 42.24m));
    }
    [Test]
    public void ExpandNull()
    {
      Validate(
        "select * from A where B is null",
        "select * from A where B is {=B}",
        new SqlParameter("@B", DBNull.Value));
    }

    [Test]
    public void ExpandInteger()
    {
      Validate(
        "select * from A where B = 42",
        "select * from A where B = {=B}",
        new SqlParameter("@B", 42));
    }

    [Test]
    public void ExpandList()
    {
      Validate(
        "select * from A where B in (1, 2, 3, 4)",
        "select * from A where B in {=B}",
        new SqlParameter("@B", new[] { 1, 2, 3, 4 }));
    }

    private void Validate(string expected, string sql, params SqlParameter[] parameters)
    {
      var parameterList = parameters.ToList();
      string actual = SQLLiteralExpander.Translate(sql, parameterList);
      Assert.AreEqual(expected, actual);
      Assert.IsEmpty(parameterList);
    }
  }
}
