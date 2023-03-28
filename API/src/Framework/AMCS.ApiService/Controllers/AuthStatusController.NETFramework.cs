#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AMCS.ApiService.Controllers.Responses;
using AMCS.Data;
using Swashbuckle.Swagger.Annotations;

namespace AMCS.ApiService.Controllers
{
  [Route("authStatus")]
  public class AuthStatusController : Controller
  {
    [HttpGet]
    [Authenticated]
    [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LoginResponse))]
    public ActionResult Get()
    {
      return DataServices.Resolve<IAuthenticationService>().Initialise(HttpContext);
    }
  }
}

#endif
