using System;
using TechTalk.SpecFlow;
using static AMCS.PlatformFramework.UnitTests.TestProperties.EnumsList;
using AMCS.PlatformFramework.UnitTests.TestProperties;

namespace AMCS.PlatformFramework.UnitTests.Steps
{
  [Binding]
  public class NullableValidationsSteps : ValidationBase

  {
    private ScenarioContext scenarioContext;
    private const string ENTITY_OBJECT = "ENTITYOBJECT";
    private const string ACTUAL_VALIDATION_RESULT_1 = "ActualValidationResult1";
    private PropertyEnum? enumProperty;
    public NullableValidationsSteps(ScenarioContext ScenarioContext)

    {
      scenarioContext = ScenarioContext;
    }

    [Given(@"an integer value (.*) enum value (.*)")]
    public void GivenAnIntegerValueEnumValue(int? integerValue, string enumValue)
    {
      switch (enumValue.ToUpperInvariant())
      {
        case "A":
          enumProperty = PropertyEnum.A;
          break;
        case "B":
          enumProperty = PropertyEnum.B;
          break;
        case "C":
          enumProperty = PropertyEnum.C;
          break;
        default:
          enumProperty = null;
          break;

      }
      scenarioContext[ENTITY_OBJECT] = new PropertyObject
      {
        IntProperty = integerValue,
        EnumProperty = enumProperty
      };
    }

    [When(@"values are checked with validation expression (.*)")]
    public void WhenValuesAreCheckedWithValidationExpression(string validationExpresssion)
    {
      scenarioContext[ACTUAL_VALIDATION_RESULT_1] = Validate(scenarioContext.Get<PropertyObject>(ENTITY_OBJECT), validationExpresssion);
    }
  }
}
