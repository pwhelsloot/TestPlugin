#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.Data.Server.Services;
using Swashbuckle.Swagger.Annotations;

namespace AMCS.ApiService.Documents
{
  [Route("documentaccess")]
  public class DocumentAccessController : Controller
  {
    [HttpGet]
    [SwaggerResponse(HttpStatusCode.OK)]
    public ActionResult Get(string type, string key, string filename)
    {
      var userId = DataServices.Resolve<IUserService>().SystemUserSessionKey;

      Stream stream;

      using (var dataSession = BslDataSessionFactory.GetDataSession(userId))
      {
        var document = DataServices.Resolve<IDocumentAccessManager>().GetDocument(userId, type, key, dataSession);

        if (
          filename != null &&
          string.IsNullOrEmpty(Path.GetExtension(filename)) &&
          document.DefaultExtension != null)
        {
          filename += document.DefaultExtension;
        }

        stream = document.Stream;
      }

      if (stream == null)
        return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

      return new FileStreamResult(stream, "application/octet-stream")
      {
        FileDownloadName = filename
      };
    }
  }
}

#endif
