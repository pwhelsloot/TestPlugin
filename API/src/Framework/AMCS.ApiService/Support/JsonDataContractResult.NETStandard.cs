#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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

    public override async Task ExecuteResultAsync(ActionContext context)
    {
      var response = context.HttpContext.Response;

      response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;
      if (ContentEncoding != null)
        response.ContentType += "; charset=" + ContentEncoding.WebName;

      if (Data == null)
        return;

      var type = DataType ?? Data.GetType();

      using (var stream = new MemoryStream())
      {
        new JsonMediaTypeFormatter().WriteToStream(type, Data, stream, ContentEncoding ?? UTF8NoBom);

        stream.Position = 0;

        await stream.CopyToAsync(response.Body);
      }
    }
  }
}

#endif
