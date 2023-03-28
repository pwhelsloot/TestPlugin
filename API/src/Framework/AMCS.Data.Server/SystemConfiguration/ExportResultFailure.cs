using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SystemConfiguration
{
  public class ExportResultFailure : IExportResult
  {
    public string Message { get; }

    public ExportResultFailure(string message)
    {
      Message = message;
    }
  }
}
