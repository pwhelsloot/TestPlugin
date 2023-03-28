using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.Validation
{
  internal class ValidationTest
  {
    public string Test { get; }
    public string Error { get; }

    public ValidationTest(string test, string error)
    {
      Test = test;
      Error = error;
    }
  }
}