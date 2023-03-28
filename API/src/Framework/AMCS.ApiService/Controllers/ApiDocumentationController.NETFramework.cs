#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AMCS.ApiService.Documentation.Abstractions.Swagger;
using AMCS.Data;

namespace AMCS.ApiService.Controllers
{
  public class ApiDocumentationController : Controller
  {
    [Route("api/documentation/error/{errorCode}")]
    public ActionResult ErrorCode(int errorCode)
    {
      string html = DataServices.Resolve<IApiExplorerConfiguration>().RenderMarkdownDocumentation($"System/ErrorCode/{errorCode}");
      if (html == null)
        return new HttpStatusCodeResult(HttpStatusCode.NotFound);

      return Content(html, "text/html; charset=utf-8");
    }
  }
}

#endif
