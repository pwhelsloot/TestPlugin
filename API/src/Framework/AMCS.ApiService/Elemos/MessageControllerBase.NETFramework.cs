#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AMCS.ApiService.Abstractions;
using AMCS.ApiService.Support;

namespace AMCS.ApiService.Elemos
{
  public abstract class MessageControllerBase<TResponse> : Controller
  {
    protected ActionResult GetResponse(TResponse response)
    {
      if (typeof(TResponse) == typeof(ContentResponse))
        return GetContentResponse(response as ContentResponse);
      if (typeof(TResponse) == typeof(StreamResponse))
        return GetStreamResponse(response as StreamResponse);
      return GetGenericResponse(response);
    }

    private ActionResult GetGenericResponse(TResponse response)
    {
      if (HttpUtils.ExpectXmlResponse(Request))
        return GetXmlResponse(response);

      return GetJsonResponse(response);
    }

    private ActionResult GetXmlResponse(TResponse response)
    {
      return new XmlSerializerResult(response, typeof(TResponse));
    }

    private ActionResult GetJsonResponse(TResponse response)
    {
      return new JsonDataContractResult(response, typeof(TResponse));
    }

    private ActionResult GetStreamResponse(StreamResponse stream)
    {
      if (stream == null)
        return new HttpStatusCodeResult(204, "No Content");

      if (stream.ContentDisposition != null)
        Response.Headers.Add("Content-Disposition", stream.ContentDisposition);

      return new FileStreamResult(stream.Stream, stream.ContentType);
    }

    private ActionResult GetContentResponse(ContentResponse content)
    {
      return Content(content.Content, content.ContentType);
    }
  }
}

#endif
