#if !NETFRAMEWORK

using AMCS.ApiService.Documentation.Abstractions.Swagger;
using AMCS.Data;
using Microsoft.AspNetCore.Mvc;

namespace AMCS.ApiService.Controllers
{
  public class ApiDocumentationController : Controller
  {
    [HttpGet]
    [Route("api/documentation/error/{errorCode}")]
    public ActionResult ErrorCode(int errorCode)
    {
      string html = DataServices.Resolve<IApiExplorerConfiguration>().RenderMarkdownDocumentation($"System/ErrorCode/{errorCode}");
      if (html == null)
        return new NotFoundResult();

      return Content(html, "text/html; charset=utf-8");
    }
  }
}

#endif
