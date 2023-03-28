#if NETFRAMEWORK

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AMCS.ApiService.Support;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;

namespace AMCS.ApiService.Elemos.Batch
{
  [Authenticated]
  [Route("batchRequests")]
  public class BatchRequestController : Controller
  {
    [HttpPost]
    [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable))]
    public ActionResult Perform()
    {
      var user = HttpContext.GetAuthenticatedUser();
      var requests = BatchRequestParser.FromStream(Request.InputStream);

      using (var writer = new StringWriter())
      {
        using (var json = new JsonTextWriter(writer))
        {
          json.WriteStartArray();

          foreach (var request in requests)
          {
            try
            {
              request.Perform(json, user, new ApiContext(HttpContext));
            }
            catch (Exception ex)
            {
              json.WriteStartObject();

              json.WritePropertyName("$error");
              ErrorResponseWriter.WriteError(json, ex, HttpContext.IsCustomErrorEnabled);

              json.WriteEndObject();
            }
          }

          json.WriteEndArray();
        }

        return Content(writer.ToString(), "application/json");
      }
    }
  }
}

#endif
