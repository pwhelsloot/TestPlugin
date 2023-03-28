using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server
{
  public class BslValidationError
  {
    public string PropertyName { get; }

    public string Message { get; }

    public BslValidationError(string propertyName, string message)
    {
      PropertyName = propertyName;
      Message = message;
    }
  }
}
