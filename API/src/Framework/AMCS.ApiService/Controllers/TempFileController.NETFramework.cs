#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AMCS.Data;
using AMCS.Data.Server.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMCS.ApiService.Controllers
{
  [Authenticated]
  public class TempFileController : Controller
  {
    [HttpGet]
    [Route("uploads")]
    public async Task<ActionResult> Get(string id)
    {
      var stream = await DataServices.Resolve<ITempFileService>().ReadFileAsync(id);

      if (stream == null)
        return HttpNotFound();

      bool compress = HasAcceptEncoding("gzip");

      if (compress)
      {
        stream = new GZipPullStream.GZipPullStream(stream);

        Response.Headers["Content-Encoding"] = "gzip";
      }

      return File(stream, "application/octet-stream");
    }

    private bool HasAcceptEncoding(string encoding)
    {
      string acceptEncoding = Request.Headers["Accept-Encoding"];
      if (acceptEncoding == null)
        return false;

      foreach (string part in acceptEncoding.Split(','))
      {
        string partEncoding;
        int pos = part.IndexOf(';');
        if (pos == -1)
          partEncoding = part.Trim();
        else
          partEncoding = part.Substring(0, pos).Trim();

        if (String.Equals(partEncoding, encoding, StringComparison.OrdinalIgnoreCase))
          return true;
      }

      return false;
    }

    [HttpPost]
    [Route("uploads")]
    public async Task<ActionResult> Post()
    {
      Request.InputStream.Position = 0;

      string id;

      using (var decompressed = Decompress(Request.InputStream))
      {
        id = await DataServices.Resolve<ITempFileService>().WriteFileAsync(decompressed);
      }

      var result = new JObject
      {
        ["Id"] = id
      };

      return Content(result.ToString(), "application/json");
    }

    private Stream Decompress(Stream stream)
    {
      string contentEncoding = Request.Headers["Content-Encoding"];
      if (contentEncoding == null)
        return stream;
      if (!String.Equals(contentEncoding, "gzip", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Unexpected content encoding");

      return new GZipStream(stream, CompressionMode.Decompress);
    }

    [HttpDelete]
    [Route("uploads")]
    public async Task<ActionResult> Delete(string id)
    {
      await DataServices.Resolve<ITempFileService>().DeleteFileAsync(id);

      return Content("");
    }
  }
}

#endif
