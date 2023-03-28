using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Abstractions
{
  public class ContentResponse
  {
    public string Content { get; }

    public string ContentType { get; }

    public ContentResponse(string content, string contentType)
    {
      Content = content;
      ContentType = contentType;
    }
  }
}
