using AMCS.PlatformFramework.UnitTests.TestProperties;
using NUnit.Framework;
using System;
using TechTalk.SpecFlow;
using static AMCS.PlatformFramework.UnitTests.TestProperties.EnumsList;

namespace AMCS.PlatformFramework.UnitTests.Steps
{
  [Binding]
  public class EntityValidationsSteps : ValidationBase
  {
    public PropertyEnum EnumProperty { get; set; }
    private EntityValidationsSteps Obj;
    private bool ValidationResult;

    [Given(@"a set of enums")]
    public void GivenASetOfEnums()
    {
      Obj = new EntityValidationsSteps
      {
        EnumProperty = PropertyEnum.B
      };
    }

    [When(@"enums are validated with an (.*)")]
    public void WhenEnumsAreValidatedWithAnEnumExpression(string enumExpression)
    {
      ValidationResult = Validate(Obj, enumExpression);
    }

    [Then(@"corresponding results is shown")]
    public void ThenCorrespondingResultsIsShown()
    {
      Assert.IsTrue(ValidationResult);

    }
  }
}