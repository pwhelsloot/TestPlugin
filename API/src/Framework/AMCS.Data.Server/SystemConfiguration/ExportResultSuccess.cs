using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SystemConfiguration
{
  public class ExportResultSuccess : IExportResult
  {
    public string Xml { get; }

    public ExportResultSuccess(string xml)
    {
      Xml = xml;
    }
  }
}
