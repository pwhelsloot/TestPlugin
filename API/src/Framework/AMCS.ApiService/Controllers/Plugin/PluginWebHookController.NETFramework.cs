#if NETFRAMEWORK

namespace AMCS.ApiService.Controllers.Plugin
{
  using System;
  using System.Net;
  using System.Web.Mvc;
  using AMCS.Data;
  using AMCS.Data.Server;
  using AMCS.Data.Server.WebHook;
  using AMCS.PluginData.Data.WebHook;
  using AMCS.PluginData.Services;
  using AMCS.ApiService.Support;
  using Data.Server.Plugin;

  [Route("plugin/web-hooks")]
  [Authenticated(RequiredIdentity = WellKnownIdentities.CoreApp)]
  public class PluginWebHookController : Controller
  {
    [HttpPost]
    [PluginActionFilter]
    public ActionResult InstallWebHookAction()
    {
      try
      {
        var userId = HttpContext.GetAuthenticatedUser();
        using (var session = BslDataSessionFactory.GetDataSession(userId))
        using (var transaction = session.CreateTransaction())
        {
          var webHook = (PluginWebHook)ViewData.Model;

          var webHookService = DataServices.Resolve<IWebHookService>();

          if (!webHookService.ChangeDetectedInWebHook(webHook, PluginHelper.GetCorePluginFullName(), Guid.Parse(userId.TenantId)))
            return new HttpStatusCodeResult(HttpStatusCode.NotModified);

          var result = webHookService.SaveWebHook(webHook, PluginHelper.GetCorePluginFullName(), userId, session);

          var webHookRegistration = new WebHookRegistration
          {
            Id = result.ToString()
          };

          var response = DataServices.Resolve<IPluginSerializationService>().Serialize(webHookRegistration);

          transaction.Commit();

          return Content(response, "text/xml");
        }
      }
      catch (Exception ex)
      {
        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpDelete]
    public ActionResult DeleteWebHookAction(string webHookGuid)
    {
      try
      {
        var userId = HttpContext.GetAuthenticatedUser();
        using (var session = BslDataSessionFactory.GetDataSession(userId))
        using (var transaction = session.CreateTransaction())
        {
          var webHookService = DataServices.Resolve<IWebHookService>();
          webHookService.DeleteWebHook(webHookGuid, userId, session);

          transaction.Commit();
        }

        return new HttpStatusCodeResult(HttpStatusCode.NoContent);
      }
      catch (Exception ex)
      {
        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
      }
    }
  }
}

#endif