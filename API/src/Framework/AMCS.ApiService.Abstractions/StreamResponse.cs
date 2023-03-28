using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Abstractions
{
  public class StreamResponse
  {
    public Stream Stream { get;  }

    public string ContentType { get; }

    public string ContentDisposition { get; }

    public StreamResponse(Stream stream, string contentType)
      : this(stream, contentType, null)
    {
    }

    public StreamResponse(Stream stream, string contentType, string contentDisposition)
    {
      Stream = stream;
      ContentType = contentType;
      ContentDisposition = contentDisposition;
    }
  }
}
