#if NETFRAMEWORK
namespace AMCS.PlatformFramework.Server.Controllers
{
  using System;
  using System.IO;
  using System.Net;
  using System.Web.Mvc;
  using ApiService;
  using Data;
  using Data.Server.Services;
  using log4net;
  using Newtonsoft.Json;

  [Authenticated]
  [Route("plugins/register-tenant")]
  public class TenantRegistrationController : Controller
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(TenantRegistrationController));

    [HttpPost]
    public ActionResult RegisterTenant()
    {
      string body;

      Request.InputStream.Position = 0;

      using (var stream = Request.InputStream)
      using (var reader = new StreamReader(stream))
      {
        body = reader.ReadToEnd();
      }

      Logger.Info($"Received call to register tenant: { body }");

      var tenant = JsonConvert.DeserializeObject<TenantMetadata>(body);

      DataServices.Resolve<ITenantManager>().AddTenant(tenant.TenantId, tenant.CoreServiceRoot);

      return new HttpStatusCodeResult(HttpStatusCode.NoContent);
    }
  }
}
#endif