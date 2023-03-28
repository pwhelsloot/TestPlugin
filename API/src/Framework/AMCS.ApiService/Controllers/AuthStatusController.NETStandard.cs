#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Controllers.Responses;
using AMCS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMCS.ApiService.Controllers
{
  [Route("authStatus")]
  public class AuthStatusController : Controller
  {
    [HttpGet]
    [ApiAuthorize]
    public ActionResult Get()
    {
      return DataServices.Resolve<IAuthenticationService>().Initialise(HttpContext);
    }
  }
}

#endif
