using AMCS.PlatformFramework.UnitTests.TestProperties;
using TechTalk.SpecFlow;
using PropertyObject = AMCS.PlatformFramework.UnitTests.NumericPropertyValidation.PropertyObject;

namespace AMCS.PlatformFramework.UnitTests.Steps
{
  [Binding]
  public class NumericPropertyValidationsSteps : ValidationBase
  {
    private ScenarioContext scenarioContext;
    private const string ENTITY_OBJECT = "ENTITYOBJECT";
    private const string ACTUAL_VALIDATION_RESULT_1 = "ActualValidationResult1";
    public NumericPropertyValidationsSteps(ScenarioContext ScenarioContext)

    {
      scenarioContext = ScenarioContext;
    }

    [Given(@"a numeric value (.*)")]
    public void GivenANumericValueNumericValue(int value)
    {
      scenarioContext[ENTITY_OBJECT] = PropertyObject.WithValue(value);
    }

    [When(@"numeric value is validated with an (.*)")]
    public void WhenNumericValueIsValidatedWithAnSBytePropertyByInvokingOnce(string validationExpression)
    {
      scenarioContext[ACTUAL_VALIDATION_RESULT_1] = Validate(scenarioContext.Get<PropertyObject>(ENTITY_OBJECT), validationExpression);
    }
  }
}