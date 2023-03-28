using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  public class RenderedDiagnostics
  {
    public string Content { get; }
    public string ContentType { get; }
    public bool IsSuccess { get; }
    public Encoding Encoding { get; }

    public RenderedDiagnostics(string content, string contentType, bool isSuccess, Encoding encoding)
    {
      Content = content;
      ContentType = contentType;
      IsSuccess = isSuccess;
      Encoding = encoding;
    }
  }
}
