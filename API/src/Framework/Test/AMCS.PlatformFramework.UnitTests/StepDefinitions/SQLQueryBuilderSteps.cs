using System.Text.RegularExpressions;
using AMCS.Data.Server.SQL.Querying;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace AMCS.PlatformFramework.UnitTests.Steps
{
  [Binding]
  public class SQLQueryBuilderSteps
  {
    private SQLQueryBuilder Query;
    private const string SIMPLE_QUERY = "SIMPLEQUERY";
    private const string SIMPLE_SELECT_ADDED_WHERE_QUERY = "SIMPLESELECTADDEDWHEREQUERY";
    private const string SIMPLE_SELECT_CLEARED_WHERE_QUERY = "SIMPLESELECTCLEAREDWHEREQUERY";
    private const string SIMPLE_REPLACED_SELECT_ADDED_WHERE_QUERY = "SIMPLEREPLACEDSELECTADDEDWHEREQUERY";
    private const string SIMPLE_SELECT_REPLACED_WHERE_QUERY = "SIMPLESELECTREPLACEDWHEREQUERY";
    private const string SELECT_MULTIPLE_WHERE_QUERY = "SELECTMULTIPLEWHEREQUERY";
    private const string ADDITIONAL_WHERE_CLAUSE = "field2 < 0";
    private const string CLEARED_WHERE_CLAUSE = "field1 < 0";
    private const string NEW_SELECT_CLAUSE = "field1";
    private IList<string> MULTIPLE_WHERE_CLAUSE = new List<string>() { $"{ADDITIONAL_WHERE_CLAUSE}", "field3 < 0", "field4 < 0", "field5 < 0", "field6 < 0", "field7 < 0" };
    private string selectQueryString;
    private string whereQueryString;
    private string expectedResult = null;
    private string SimpleSelectClearedWhereQuery = @"select field1, field2
from table";
    private string SimpleQuery = @"
select field1, field2
from table
where field1 > 0
";
    private string SimpleSelectAddedWhereQuery = @"
select field1, field2
from table
where field1 > 0 and field2 < 0
";
    private string SimpleReplacedSelectAddedWhereQuery = @"
select field1
from table
where field1 > 0 and field2 < 0
";

    private string SimpleSelectReplacedWhereQuery = @"
select field1, field2
from table
where field2 < 0
";
    private string SelectMultipleWhereQuery = @"
select field1, field2
from table
where field1 > 0 and field2 < 0 and field3 < 0 and field4 < 0 and field5 < 0 and field6 < 0 and field7 < 0
";

    [Given(@"a queryString (.*)")]
    public void GivenASimpleQuery(string queryType)
    {
      switch (queryType.ToUpperInvariant())
      {
        case SIMPLE_SELECT_ADDED_WHERE_QUERY:
        case SIMPLE_SELECT_REPLACED_WHERE_QUERY:
          whereQueryString = ADDITIONAL_WHERE_CLAUSE;
          break;
        case SIMPLE_REPLACED_SELECT_ADDED_WHERE_QUERY:
          selectQueryString = NEW_SELECT_CLAUSE;
          whereQueryString = ADDITIONAL_WHERE_CLAUSE;
          break;
        case SIMPLE_SELECT_CLEARED_WHERE_QUERY:
          whereQueryString = CLEARED_WHERE_CLAUSE;
          break;
      }
    }

    [When(@"queryString (.*) is built using query builder")]
    public void WhenQueryIsBuiltUsingQueryBuilder(string queryType)
    {
      Query = Create();
      switch (queryType.ToUpperInvariant())
      {
        case SIMPLE_QUERY:
          Query = Create();
          break;
        case SIMPLE_SELECT_ADDED_WHERE_QUERY:
          Query.AddWhere(whereQueryString);
          break;
        case SIMPLE_SELECT_CLEARED_WHERE_QUERY:
          Query.ClearWhere();
          break;
        case SIMPLE_SELECT_REPLACED_WHERE_QUERY:
          Query.ResetWhere(whereQueryString);
          break;
        case SIMPLE_REPLACED_SELECT_ADDED_WHERE_QUERY:
          Query.ClearSelect();
          Query.AddWhere(whereQueryString);
          Query.AddSelect(selectQueryString);
          break;
        case SELECT_MULTIPLE_WHERE_QUERY:
          foreach (string queryString in MULTIPLE_WHERE_CLAUSE)
            Query.AddWhere(queryString);
          break;
      }
    }

    [Then(@"actual result equals expected result (.*)")]
    public void ThenActualResultEqualsExpectedResult(string expectedQueryType)
    {
      switch (expectedQueryType.ToUpperInvariant())
      {
        case SIMPLE_QUERY:
          expectedResult = SimpleQuery;
          break;
        case SIMPLE_SELECT_ADDED_WHERE_QUERY:
          expectedResult = SimpleSelectAddedWhereQuery;
          break;
        case SIMPLE_SELECT_CLEARED_WHERE_QUERY:
          expectedResult = SimpleSelectClearedWhereQuery;
          break;
        case SIMPLE_SELECT_REPLACED_WHERE_QUERY:
          expectedResult = SimpleSelectReplacedWhereQuery;
          break;
        case SIMPLE_REPLACED_SELECT_ADDED_WHERE_QUERY:
          expectedResult = SimpleReplacedSelectAddedWhereQuery;
          break;
        case SELECT_MULTIPLE_WHERE_QUERY:
          expectedResult = SelectMultipleWhereQuery;
          break;
      }
      AssertEqual(expectedResult, Query);
    }
    private void AssertEqual(string expected, SQLQueryBuilder query)
    {
      expected = expected.Trim();
      string actual = query.ToString().Trim();

      string expectedSimple = Regex.Replace(expected, "\\s+", " ").Trim();
      string actualSimple = Regex.Replace(actual, "\\s+", " ").Trim();

      if (!string.Equals(expectedSimple, actualSimple, StringComparison.OrdinalIgnoreCase))
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