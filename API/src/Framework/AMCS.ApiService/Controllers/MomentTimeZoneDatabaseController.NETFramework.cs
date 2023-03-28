#if NETFRAMEWORK

using System.Net;
using System.Web;
using System.Web.Mvc;
using AMCS.Data;
using Swashbuckle.Swagger.Annotations;

namespace AMCS.ApiService.Controllers
{
  public class MomentTimeZoneDatabaseController : Controller
  {
    [HttpGet]
    [Route("momentTimeZoneDatabase")]
    [SwaggerResponse(HttpStatusCode.OK)]
    public ActionResult GetDatabase()
    {
      var database = DataServices.Resolve<ITimeZoneConfiguration>().MomentTimeZoneDatabaseCache.MomentTimeZoneDatabase;
      if (database == null)
        return new HttpStatusCodeResult(HttpStatusCode.NotFound);

      // Check and set the ETag.

      var requestedETag = Request.Headers["If-None-Match"];
      if (requestedETag == database.ETag)
        return new HttpStatusCodeResult(HttpStatusCode.NotModified);

      Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
      Response.Cache.SetETag(database.ETag);

      // Compression support.

      bool compress = Request["Accept-Encoding"]?.Contains("gzip") ?? false;
      if (compress)
      {
        Response.Headers.Remove("Content-Encoding");
        Response.AppendHeader("Content-Encoding", "gzip");
      }

      Response.AppendHeader("Vary", "Content-Encoding");

      // Return the response.

      return new FileStreamResult(database.GetStream(compress), "application/json");
    }
  }
}

#endif
