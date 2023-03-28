#if !NETFRAMEWORK

using System;
using System.Linq;
using System.Net;
using AMCS.Data;
using Microsoft.AspNetCore.Mvc;

namespace AMCS.ApiService.Controllers
{
  public class MomentTimeZoneDatabaseController : Controller
  {
    [HttpGet]
    [Route("momentTimeZoneDatabase")]
    public ActionResult GetDatabase()
    {
      var database = DataServices.Resolve<ITimeZoneConfiguration>().MomentTimeZoneDatabaseCache.MomentTimeZoneDatabase;
      if (database == null)
        return new NotFoundResult();

      // Check and set the ETag.

      var requestedETag = Request.Headers["If-None-Match"];
      if (requestedETag == database.ETag)
        return new StatusCodeResult((int)HttpStatusCode.NotModified);

      Response.Headers["Cache-Control"] = "private";
      Response.Headers["ETag"] = database.ETag;

      // Compression support.

      var acceptEncoding = Request.Headers["Accept-Encoding"];
      bool compress = acceptEncoding.Contains("gzip");
      if (compress)
      {
        Response.Headers.Remove("Content-Encoding");
        Response.Headers.Add("Content-Encoding", "gzip");
      }

      Response.Headers.Add("Vary", "Content-Encoding");

      // Return the response.

      return new FileStreamResult(database.GetStream(compress), "application/json");
    }
  }
}

#endif
