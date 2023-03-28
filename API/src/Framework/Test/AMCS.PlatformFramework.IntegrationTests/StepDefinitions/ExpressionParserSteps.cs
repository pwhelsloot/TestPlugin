using System;
using System.Collections.Generic;
using AMCS.Data.Server.SQL.Querying;
using AMCS.PlatformFramework.IntegrationTests.TestProperties;
using TechTalk.SpecFlow;

namespace AMCS.PlatformFramework.IntegrationTests.Steps
{
  [Binding]
  public class ExpressionParserSteps : BaseTest
  {
    private string EXPECTED_EXPRESSION = "stringProp in ('abc',   'def',   'ghi') and   boolProp eq   true and   intProp gte   7 and intProp   lt 10   and   " +
        "doubleProp from   8.8   to   65.3 and decimalProp   ne   44.98 and   dateProp from '2017-06-25'   to   '2018-03-19'";

    private IList<IExpression> ExpressionsList = new List<IExpression>();
    private string[] StringProp = new string[] { };
    private bool BoolProp;
    private int StartIntProp;
    private int EndIntProp;
    private double StartDoubleProp;
    private double EndDoubleProp;
    private Decimal DecimalProp;
    private DateTime StartDateProp;
    private DateTime EndDateProp;

    [Given(@"(.*) filter criterias")]
    public void GivenVariousFilterCriterias(string criteriaAvailability)
    {
      if (criteriaAvailability.ToUpperInvariant().Equals("VARIOUS"))
      {
        StringProp = new[] { "abc", "def", "ghi" };
        BoolProp = true;
        StartIntProp = 7;
        EndIntProp = 10;
        StartDoubleProp = 8.8;
        EndDoubleProp = 65.3;
        DecimalProp = 44.98m;
        StartDateProp = new DateTime(2017, 6, 25);
        EndDateProp = new DateTime(2018, 3, 19);
      }
    }

    [When(@"expression built out of them")]
    public void WhenExpressionBuiltOutOfThem()
    {
      ExpressionsList.Add(Expression.In("StringProp", StringProp));
      ExpressionsList.Add(Expression.Eq("BoolProp", BoolProp));
      ExpressionsList.Add(Expression.Ge("IntProp", StartIntProp));
      ExpressionsList.Add(Expression.Lt("IntProp", EndIntProp));
      ExpressionsList.Add(Expression.Ge("DoubleProp", StartDoubleProp));
      ExpressionsList.Add(Expression.Le("DoubleProp", EndDoubleProp));
      ExpressionsList.Add(Expression.Ne("DecimalProp", DecimalProp));
      ExpressionsList.Add(Expression.Ge("DateProp", StartDateProp));
      ExpressionsList.Add(Expression.Le("DateProp", EndDateProp));
    }

    [Then(@"it matches expected filter expression")]
    public void ThenItMatchesExpectedFilterExpression()
    {
      Filter.AssertFilter(EXPECTED_EXPRESSION, p =>
      {
        foreach (var expression in ExpressionsList)
        {
          p.Add(expression);
        }
      });
    }

    [When(@"validated for expression builder")]
    public void WhenValidatedForExpressionBuilder()
    {
      EXPECTED_EXPRESSION = "stringProp in ('abc',   'def',   'ghi') or boolProp eq true";
    }

    [Then(@"it fails the validation")]
    public void ThenItFailsTheValidation()
    {
      Filter.Fails(EXPECTED_EXPRESSION);
    }
  }
}