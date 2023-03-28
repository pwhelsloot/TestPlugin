using AMCS.Data.EndToEnd.Tests;
using AMCS.Data.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace AMCS.Data.EndToEnd.Tests
{
  [Binding]
  public class StepHelper : BindingBase
  {
    public StepHelper(ScenarioContext context) 
      : base(context)
    {
    }

    [StepArgumentTransformation(@"(does|doesn't|does not)")]
    public bool DoesToBool(string value)
    {
      switch (value)
      {
        case "does": return true;
        case "does not":
        case "doesn't":
          return false;
        default:
          throw new NotSupportedException(value + " is not supported");
      }
    }

    [StepArgumentTransformation(@"(should|shouldn't|should not)")]
    public bool ShouldToBool(string value)
    {
      switch (value)
      {
        case "should": return true;
        case "should not":
        case "shouldn't":
          return false;
        default:
          throw new NotSupportedException(value + " is not supported");
      }
    }

    [StepArgumentTransformation(@"(enabled|disabled)")]
    public bool EnabledToBool(string value)
    {
      switch (value)
      {
        case "enabled": return true;
        case "disabled": return false;
        default:
          throw new NotSupportedException(value + " is not supported");
      }
    }
  }
}
