#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace AMCS.ApiService.Support
{
  internal class XmlSerializerResult : ActionResult
  {
    public object Data { get; set; }

    public Type DataType { get; set; }

    public string ContentType { get; set; }

    public Encoding ContentEncoding { get; set; }

    public XmlSerializerResult(object data)
      : this(data, null)
    {
    }

    public XmlSerializerResult(object data, Type dataType)
    {
      Data = data;
      DataType = dataType;
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
      var response = context.HttpContext.Response;

      response.ContentType = string.IsNullOrEmpty(ContentType) ? "text/xml" : ContentType;
      if (ContentEncoding != null)
        response.ContentType += "; charset=" + ContentEncoding.WebName;

      if (Data == null)
        return;

      var type = DataType ?? Data.GetType();

      using (var stream = new MemoryStream())
      {
        using (var writer = new StreamWriter(stream, ContentEncoding ?? Encoding.UTF8, leaveOpen: true))
        {
          new XmlSerializer(type).Serialize(writer, Data);
        }

        stream.Position = 0;

        await stream.CopyToAsync(response.Body);
      }
    }
  }
}

#endif
