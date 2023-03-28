#if !NETFRAMEWORK
namespace AMCS.PlatformFramework.Server.Controllers
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using ApiService;
  using Data;
  using Data.Server.Services;
  using log4net;
  using Microsoft.AspNetCore.Mvc;
  using Newtonsoft.Json;

  [ApiAuthorize]
  [Route("plugins/register-tenant")]
  public class TenantRegistrationController : Controller
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(TenantRegistrationController));

    [HttpPost]
    public async Task<IActionResult> RegisterTenant()
    {
      string body;

      await using (var stream = Request.Body)
      using (var reader = new StreamReader(stream))
      {
        body = await reader.ReadToEndAsync();
      }

      Logger.Info($"Received call to register tenant: { body }");

      var tenant = JsonConvert.DeserializeObject<TenantMetadata>(body);

      DataServices.Resolve<ITenantManager>().AddTenant(tenant.TenantId, tenant.CoreServiceRoot);

      return NoContent();
    }
  }
}
#endif