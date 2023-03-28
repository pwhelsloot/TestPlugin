#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using AMCS.Data;
using AMCS.Data.Server.Services;
using AMCS.Data.Support;
using log4net;

namespace AMCS.ApiService.Controllers
{
  public class BlobController : Controller
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(BlobController));
    private const string DefaultContentType = "application/octet-stream";

    [Route("blob/{hash}")]
    [HttpGet]
    public ActionResult GetBlob(string hash, string embed = "false")
    {
      try
      {
        var embedResponse = XmlConvert.ToBoolean(embed.ToLower());
        var stream = DataServices.Resolve<IBlobStorageService>().GetBlob(hash);
        var ffstream = new FileFormatStream(stream);
        var contentType = DefaultContentType;
        if (embedResponse && !string.IsNullOrEmpty(ffstream.FileFormat.MIMEType))
          contentType = ffstream.FileFormat.MIMEType;

        return new NonExpiringFileStreamResult(ffstream, contentType);
      }
      catch (Exception ex)
      {
        Log.Info("Error while getting blob", ex);

        // We don't expose the specific error type and return any error
        // as a not found.

        return HttpNotFound();
      }
    }

    private class NonExpiringFileStreamResult : FileStreamResult
    {
      public NonExpiringFileStreamResult(Stream fileStream, string contentType)
        : base(fileStream, contentType)
      {
      }

      protected override void WriteFile(HttpResponseBase response)
      {
        response.AddHeader("Cache-Control", "max-age=31536000, public");

        base.WriteFile(response);
      }
    }
  }
}

#endif
