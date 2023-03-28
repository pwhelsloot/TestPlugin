#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AMCS.ApiService.Support
{
  internal class JsonDataContractResult : ActionResult
  {
    private static readonly UTF8Encoding UTF8NoBom = new UTF8Encoding(false);

    public object Data { get; set; }

    public Type DataType { get; set; }

    public string ContentType { get; set; }

    public Encoding ContentEncoding { get; set; }

    public JsonDataContractResult(object data)
      : this(data, null)
    {
    }

    public JsonDataContractResult(object data, Type dataType)
    {
      Data = data;
      DataType = dataType;
    }

    public override void ExecuteResult(ControllerContext context)
    {
      var response = context.HttpContext.Response;

      response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;
      if (ContentEncoding != null)
        response.ContentEncoding = ContentEncoding;

      if (Data == null)
        return;

      var type = DataType ?? Data.GetType();

      new JsonMediaTypeFormatter().WriteToStream(type, Data, response.OutputStream, ContentEncoding ?? UTF8NoBom);
    }
  }
}

#endif
