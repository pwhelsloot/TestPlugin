using AMCS.PlatformFramework.UnitTests;
using AMCS.PlatformFramework.UnitTests.TestProperties;
using System;
using TechTalk.SpecFlow;

namespace AMCS.Data.EntityValidation.Test
{
  [Binding]
  public class LiteralValidationsSteps : ValidationBase
  {
    public ScenarioContext scenarioContext;
    private const string ACTUAL_VALIDATION_RESULT_1 = "ActualValidationResult1";
    private string Value1;
    private string Value2;
    private const string OR_Operator = "OR";
    private const string AND_Operator = "AND";
    private string Comparator;

    public LiteralValidationsSteps(ScenarioContext ScenarioContext)
    {
      scenarioContext = ScenarioContext;
    }
    [Given(@"two (.*) value1 (.*) value2 (.*)")]
    public void GivenTwoNumericsValues(string inputType, string value1 = null, string value2 = null)
    {
      Value1 = value1;
      Value2 = value2;
    }

    [When(@"they are compared with comparator (.*)")]
    public void WhenValueValueIsComparedWithComparator(string comparator)
    {
      if (comparator.ToUpperInvariant().Equals(OR_Operator))
        Comparator = "||";
      else if (comparator.ToUpperInvariant().Equals(AND_Operator))
        Comparator = "&&";
      else
        Comparator = comparator;      
      scenarioContext[ACTUAL_VALIDATION_RESULT_1] = Validate((Value1 + Comparator + Value2).ToString());
    }
  }
}