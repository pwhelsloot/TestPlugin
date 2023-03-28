#if !NETFRAMEWORK

namespace AMCS.ApiService.Controllers.Plugin
{
  using System;
  using System.Net;
  using AMCS.Data;
  using AMCS.Data.Server;
  using AMCS.Data.Server.WebHook;
  using AMCS.PluginData.Services;
  using Microsoft.AspNetCore.Mvc;
  using AMCS.ApiService.Support;
  using AMCS.PluginData.Data.WebHook;
  using Data.Server.Plugin;

  [Route("plugin/web-hooks")]
  [ApiAuthorize(Policy = ApiPolicy.RequiresCoreIdentity)]
  public class PluginWebHookController : Controller
  {
    [HttpPost]
    public IActionResult CreateWebHook([FromBody] PluginWebHook webHook)
    {
      try
      {
        var userId = HttpContext.GetAuthenticatedUser();
        using var session = BslDataSessionFactory.GetDataSession(userId);
        using var transaction = session.CreateTransaction();

        var webHookService = DataServices.Resolve<IWebHookService>();

        if (!webHookService.ChangeDetectedInWebHook(webHook, PluginHelper.GetCorePluginFullName(), Guid.Parse(userId.TenantId)))
          return StatusCode((int)HttpStatusCode.NotModified);

        var result = webHookService.SaveWebHook(webHook, PluginHelper.GetCorePluginFullName(), userId, session);

        var webHookRegistration = new WebHookRegistration
        {
          Id = result.ToString()
        };

        var response = DataServices.Resolve<IPluginSerializationService>().Serialize(webHookRegistration);

        transaction.Commit();

        return Content(response, "text/xml");
      }
      catch (Exception ex)
      {
        return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpDelete]
    public IActionResult DeleteWebHookAction([FromQuery] string webHookGuid)
    {
      try
      {
        var userId = HttpContext.GetAuthenticatedUser();
        using var session = BslDataSessionFactory.GetDataSession(userId);
        using var transaction = session.CreateTransaction();

        var webHookService = DataServices.Resolve<IWebHookService>();

        webHookService.DeleteWebHook(webHookGuid, userId, session);

        transaction.Commit();

        return NoContent();
      }
      catch (Exception ex)
      {
        return StatusCode((int) HttpStatusCode.InternalServerError, ex.Message);
      }
    }
  }
}

#endif