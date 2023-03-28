#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Serialization;

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

    public override void ExecuteResult(ControllerContext context)
    {
      var response = context.HttpContext.Response;

      response.ContentType = string.IsNullOrEmpty(ContentType) ? "text/xml" : ContentType;
      if (ContentEncoding != null)
        response.ContentEncoding = ContentEncoding;

      if (Data == null)
        return;

      var type = DataType ?? Data.GetType();

      using (var writer = new StreamWriter(response.OutputStream, ContentEncoding ?? Encoding.UTF8))
      {
        new XmlSerializer(type).Serialize(writer, Data);
      }
    }
  }
}

#endif
