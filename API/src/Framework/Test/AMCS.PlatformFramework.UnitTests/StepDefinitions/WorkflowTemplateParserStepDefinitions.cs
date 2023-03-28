using AMCS.Data.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime.Text;
using NUnit.Framework;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System;
using TechTalk.SpecFlow;

namespace AMCS.PlatformFramework.UnitTests.StepDefinitions
{
  [Binding]
  public class WorkflowTemplateParserStepDefinitions
  {
    private string ParsedResult = string.Empty;
    private string Template = string.Empty;
    private ScenarioContext scenarioContext;
    private const string VALUE = "Value";
    private const string JSON_INPUT = "JsonInput";
    private const string SerializedJson = "SerializedJson";
    private Exception exception = new Exception();

    public WorkflowTemplateParserStepDefinitions(ScenarioContext scenarioContext)
    {
      this.scenarioContext = scenarioContext;
    }

    [Given(@"template (.*) value (.*)")]
    public void GivenTemplateHttpLocalhostIdSomeVariableDefaultValue(string template, object value)
    {
      Template = @template;
      if (value.Equals("null"))
      {
        value = null;
      }
      scenarioContext[VALUE] = value;
    }

    [Given(@"a json")]
    public void GivenAJson()
    {
      scenarioContext[JSON_INPUT] = new
      {
        Variable1 = "{{variable1}}",
        Variable2 = "{{variable2|default:false}}",
        Variable3 = "{{variable3|quote}}",
        Variable4 = "{{variable4|default:sup sup|quote}}",
        Variable5 = "{{variable5}}",
        Variable6 = "{{variable6}}"
      };

      scenarioContext[SerializedJson] = JsonConvert.SerializeObject(scenarioContext.Get<object>(JSON_INPUT));
    }

    [When(@"WorkflowTemplateParser is built with json")]
    public void WhenWorkflowTemplateParserIsBuiltWithJson()
    {
      ParsedResult = WorkflowTemplateParser.Create(scenarioContext.Get<string>(SerializedJson))
                      .AddParameter("variable1", 10)
                      .AddParameter("variable2", null)
                      .AddParameter("variable3", "howdy folks")
                      .AddParameter("variable4", null)
                      .AddParameter("variable6", null)
                      .Build();
    }

    [When(@"WorkflowTemplateParser is built with invalid template")]
    public void WhenWorkflowTemplateParserIsBuiltWithInvalidTemplate()
    {
      throw new PendingStepException();
    }

    [When(@"WorkflowTemplateParser is built with template and value")]
    public void WhenWorkflowTemplateParserIsBuiltWithTemplateAndValue()
    {
      try
      {
        ParsedResult = WorkflowTemplateParser.Create(Template)
                        .AddParameter("someVariable", scenarioContext.Get<object>(VALUE))
                        .Build();
      }
      catch (Exception ex)
      {
        exception = ex;
      }
    }

    [Then(@"InvalidOperationException is thrown")]
    public void ThenInvalidOperationExceptionIsThrown()
    {
      Assert.AreEqual(typeof(InvalidOperationException), exception.GetType());
    }

    [Then(@"result matches expected result")]
    public void ThenResultMatchesExpectedResult()
    {
      var result = JsonConvert.DeserializeAnonymousType(ParsedResult, new
      {
        Variable1 = 0,
        Variable2 = true,
        Variable3 = string.Empty,
        Variable4 = string.Empty,
        Variable5 = string.Empty,
        Variable6 = string.Empty
      });
      Assert.AreEqual(10, result.Variable1);
      Assert.AreEqual(false, result.Variable2);
      Assert.AreEqual("howdy folks", result.Variable3);
      Assert.AreEqual("sup sup", result.Variable4);
      Assert.AreEqual("{{variable5}}", result.Variable5);
      Assert.AreEqual(null, result.Variable6);
    }

    [Then(@"result matches (.*) expected result")]
    public void ThenResultMatchesHttpLocalhostIdExpectedResult(object expectedResult)
    {
      Assert.AreEqual(expectedResult, ParsedResult);
    }
  }
}