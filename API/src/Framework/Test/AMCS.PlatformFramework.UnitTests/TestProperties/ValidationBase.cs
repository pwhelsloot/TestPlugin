using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.EntityValidation.Rules;

namespace AMCS.PlatformFramework.UnitTests.TestProperties
{
  public abstract class ValidationBase
  {
    protected bool Validate(string expression)
    {
      return Validate(new object(), expression);
    }

    protected bool Validate<T>(T target, string expression)
    {
      return !Validation.Parse(typeof(T), expression).IsValid(target);
    }
  }
}
