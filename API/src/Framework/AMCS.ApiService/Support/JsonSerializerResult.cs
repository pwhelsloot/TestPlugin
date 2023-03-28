#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace AMCS.ApiService.Support
{
  internal class JsonSerializerResult : ActionResult
  {
    public object Data { get; set; }

    public JsonSerializer Serializer { get; set; }

    public string ContentType { get; set; }

    public Encoding ContentEncoding { get; set; }

    public JsonSerializerResult(object data)
      : this(data, null)
    {
    }

    public JsonSerializerResult(object data, JsonSerializer serializer)
    {
      Data = data;
      Serializer = serializer;
    }

    public override void ExecuteResult(ControllerContext context)
    {
      var response = context.HttpContext.Response;

      response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;
      if (ContentEncoding != null)
        response.ContentEncoding = ContentEncoding;

      if (Data == null)
        return;

      var serializer = Serializer ?? new JsonSerializer();

      using (var writer = new StreamWriter(response.OutputStream, ContentEncoding ?? Encoding.UTF8))
      using (var json = new JsonTextWriter(writer))
      {
        serializer.Serialize(json, Data);
      }
    }
  }
}

#endif