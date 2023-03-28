using AMCS.PlatformFramework.UnitTests.TestProperties;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace AMCS.PlatformFramework.UnitTests.Steps
{
  [Binding]
  public class InvocationValidationsSteps : ValidationBase
  {
    public ScenarioContext scenarioContext;
    private const string ENTITY_OBJECT = "ENTITYOBJECT";
    private const string ACTUAL_VALIDATION_RESULT_1= "ActualValidationResult1";
    private const string ACTUAL_VALIDATION_RESULT_2 = "ActualValidationResult2";
    private string WHITESPACE = "<WHITESPACE>";
    private string INVOCATION_ONCE = "ONCE";
    private string INVOCATION_TWICE = "TWICE";
    private string noOfTimesInvoked= "ONCE";    

    public InvocationValidationsSteps(ScenarioContext ScenarioContext)
    {
      scenarioContext = ScenarioContext;
    }

    [Given(@"a string (.*)")]
    public void GivenAString(string input)
    {
      if (input.ToUpperInvariant().Equals(WHITESPACE))
        input = " ";
      if (string.IsNullOrEmpty(input))
        input = null;
      scenarioContext[ENTITY_OBJECT] = new PropertyObject
      {
        StringProperty = input
      };
    }

    [When(@"value is validated with an (.*) by invoking (.*)")]
    public void WhenStringIsValidatedWithAnExpression(string validationExpression, string invocationTimes)
    {
      noOfTimesInvoked = invocationTimes;
      if (invocationTimes.ToUpperInvariant().Equals(INVOCATION_ONCE) || invocationTimes.ToUpperInvariant().Equals(INVOCATION_TWICE))
        scenarioContext[ACTUAL_VALIDATION_RESULT_1] = Validate(scenarioContext.Get<PropertyObject>(ENTITY_OBJECT), validationExpression);
      if (invocationTimes.ToUpperInvariant().Equals(INVOCATION_TWICE))
        scenarioContext[ACTUAL_VALIDATION_RESULT_2] = Validate(scenarioContext.Get<PropertyObject>(ENTITY_OBJECT), validationExpression);
    }

    [Then(@"actual result matches expected result (.*)")]
    public void ThenActualResultMatchesExpectedResultTrue(bool expectedResult)
    {
      if (noOfTimesInvoked.ToUpperInvariant().Equals(INVOCATION_ONCE) || noOfTimesInvoked.ToUpperInvariant().Equals(INVOCATION_TWICE))
        Assert.AreEqual(expectedResult, scenarioContext.Get<bool>(ACTUAL_VALIDATION_RESULT_1));
      if (noOfTimesInvoked.ToUpperInvariant().Equals(INVOCATION_TWICE))
        Assert.AreEqual(expectedResult, scenarioContext.Get<bool>(ACTUAL_VALIDATION_RESULT_2));
    }    
  }
}