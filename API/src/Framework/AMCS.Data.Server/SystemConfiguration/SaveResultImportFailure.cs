using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SystemConfiguration
{
  public class SaveResultImportFailure : ISaveResult
  {
    public string Xml { get; }

    public SaveResultImportFailure(string xml)
    {
      Xml = xml;
    }
  }
}
