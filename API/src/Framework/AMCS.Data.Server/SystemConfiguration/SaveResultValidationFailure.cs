using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SystemConfiguration
{
  public class SaveResultValidationFailure : ISaveResult
  {
    public string Message { get; }

    public SaveResultValidationFailure(string message)
    {
      Message = message;
    }
  }
}
