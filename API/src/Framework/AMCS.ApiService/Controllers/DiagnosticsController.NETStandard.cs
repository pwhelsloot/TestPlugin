#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using AMCS.Data;
using AMCS.Data.Server.Services;
using AMCS.WebDiagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AMCS.ApiService.Controllers
{
  public class DiagnosticsController : Controller
  {
    [HttpGet, Route("diagnostics")]
    public IActionResult Get([FromQuery] string run, [FromQuery] string format = null)
    {
      run = run?.ToLower();
      bool parsedRun = run == "true" || run == "1";

      if (!Enum.TryParse<DiagnosticsFormat>(format, true, out var parsedFormat))
        parsedFormat = DiagnosticsFormat.Html;

      if (!DataServices.TryResolve<IDiagnosticsRenderer>(out var renderer))
        renderer = new DefaultDiagnosticsRenderer();

      var result = renderer.Render(parsedFormat, parsedRun);

      var content = Content(result.Content, result.ContentType, result.Encoding);
      if (!result.IsSuccess)
        content.StatusCode = (int)HttpStatusCode.InternalServerError;

      return content;
    }
  }
}

#endif
