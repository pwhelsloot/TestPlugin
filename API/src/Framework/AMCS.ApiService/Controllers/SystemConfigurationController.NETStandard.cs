#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AMCS.ApiService.Support;
using AMCS.Data;
using AMCS.Data.Server.SystemConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMCS.ApiService.Controllers
{
  [ApiAuthorize]
  public class SystemConfigurationController : Controller
  {
    [Route("systemConfiguration")]
    [HttpGet]
    public ActionResult LoadConfiguration()
    {
      if (!DataServices.TryResolve<ISystemConfigurationService>(out var systemConfigurationService))
        return NotFound();

      var result = systemConfigurationService.LoadConfiguration(HttpContext.GetAuthenticatedUser());

      if (result is ExportResultFailure failure)
      {
        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return new ContentResult();
      }
      else if (result is ExportResultSuccess success)
      {
        return new FileContentResult(Encoding.UTF8.GetBytes(success.Xml), "text/xml")
        {
          FileDownloadName = "SystemConfiguration.xml"
        };
      }
      else
      {
        throw new ArgumentOutOfRangeException("Unknown IExportResult instance returned");
      }
    }

    [Route("systemConfiguration")]
    [HttpPost]
    public ActionResult SaveConfiguration()
    {
      Request.Body.Position = 0;

      string xml;

      using (var reader = new StreamReader(Request.Body))
      {
        xml = reader.ReadToEnd();
      }

      if (!DataServices.TryResolve<ISystemConfigurationService>(out var systemConfigurationService))
        return NotFound();

      var result = systemConfigurationService.SaveConfiguration(HttpContext.GetAuthenticatedUser(), xml);

      if (result is SaveResultImportFailure saveResultFailure)
      {
        Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return new FileContentResult(Encoding.UTF8.GetBytes(saveResultFailure.Xml), "text/xml")
        {
          FileDownloadName = "SystemConfigurationErrors.xml"
        };
      }
      else if (result is SaveResultValidationFailure saveResultValidationFailure)
      {
        Response.StatusCode = (int)HttpStatusCode.BadRequest;
      }

      return new ContentResult();
    }

    [Route("systemConfiguration/schema")]
    [HttpGet]
    public ActionResult GetSchema()
    {
      if (!DataServices.TryResolve<ISystemConfigurationService>(out var systemConfigurationService))
        return NotFound();

      string schema = systemConfigurationService.XsdSchema;

      return new FileContentResult(Encoding.UTF8.GetBytes(schema), "text/xml")
      {
        FileDownloadName = "SystemConfiguration.xsd"
      };
    }
  }
}

#endif
