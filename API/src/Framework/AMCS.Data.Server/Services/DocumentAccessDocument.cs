using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  public class DocumentAccessDocument
  {
    public Stream Stream { get; }

    public string DefaultExtension { get; }

    public DocumentAccessDocument(Stream stream, string defaultExtension = null)
    {
      Stream = stream;
      DefaultExtension = defaultExtension;
    }
  }
}
