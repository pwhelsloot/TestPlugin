using AMCS.Data.Server.SQL;
using NUnit.Framework;
using System.Data.SqlClient;
using TechTalk.SpecFlow;

namespace AMCS.PlatformFramework.UnitTests.Steps
{
  [Binding]
  public class SQLLiteralExpanderSteps
  {
    private string baseSqlString = "select * from A where B = {=B}";
    private const string baseExpectedString = "select * from A where B";
    private string expectedResult;
    private string actualResult;
    private List<SqlParameter> SqlParameters = new List<SqlParameter>();

    //    Validate(
    //        "select * from A where B in (1, 2, 3, 4)",
    //        "select * from A where B in {=B}",
    //        new SqlParameter("@B", new[] { 1, 2, 3, 4 }));
    //    }

    private void Validate(string expected, string sql, params SqlParameter[] parameters)
    {
      var parameterList = parameters.ToList();
      string actual = SQLLiteralExpander.Translate(sql, parameterList);
      Assert.AreEqual(expected, actual);
      Assert.IsEmpty(parameterList);
    }

    [Given(@"data of type (.*) value (.*)")]
    public void GivenDataOfTypeBoolean(string dataType, string value)
    {
      switch (dataType)
      {
        case "Integer":
          expectedResult = $"{baseExpectedString} = {int.Parse(value)}";
          SqlParameters.Add(new SqlParameter("@B", int.Parse(value)));
          break;
        case "Boolean":
          expectedResult = $"{baseExpectedString} = 1";
          SqlParameters.Add(new SqlParameter("@B", bool.Parse(value)));
          break;
        case "List":
          expectedResult = $"{baseExpectedString} in ({value.Trim()})";
          baseSqlString = "select * from A where B in {=B}";
          List<int> dataList = new List<int>();
          foreach (string integerValue in value.Split(','))
            dataList.Add(int.Parse(integerValue));
          SqlParameters.Add(new SqlParameter("@B", dataList));
          break;
        case "Null":
          expectedResult = $"{baseExpectedString} is null";
          baseSqlString = "select * from A where B is {=B}";
          SqlParameters.Add(new SqlParameter("@B", DBNull.Value));
          break;
        case "MultipleBool":

          if (value.Split(',')[0].Equals("true"))
          {
            expectedResult = $"{baseExpectedString} = 1";
            SqlParameters.Add(new SqlParameter("@B", true));
          }
          else if (value.Split(',')[0].Equals("false"))
          {
            expectedResult = $"{baseExpectedString} = 0";
            SqlParameters.Add(new SqlParameter("@B", false));
          }

          if (value.Split(',')[1].Equals("true"))
          {
            expectedResult += " and C = 1";
            SqlParameters.Add(new SqlParameter("@C", true));
          }
          else if (value.Split(',')[1].Equals(" false"))
          {
            expectedResult += " and C = 0";
            baseSqlString += " and C = {=C}";
            SqlParameters.Add(new SqlParameter("@C", false));
          }

          break;
        case "Decimal":
          expectedResult = $"{baseExpectedString} = {Decimal.Parse(value)}";
          SqlParameters.Add(new SqlParameter("@B", Decimal.Parse(value)));
          break;
      }
    }

    [When(@"passed to literal expander")]
    public void WhenPassedToLiteralExpander()
    {
      actualResult = SQLLiteralExpander.Translate(baseSqlString, SqlParameters);
    }

    [Then(@"correct result is shown")]
    public void ThenCorrectResultIsShown()
    {
      Assert.AreEqual(expectedResult, actualResult);
      Assert.IsEmpty(SqlParameters);
    }
  }
}