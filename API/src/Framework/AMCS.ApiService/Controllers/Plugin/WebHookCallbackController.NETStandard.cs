#if !NETFRAMEWORK

using System;
using System.IO;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Server.WebHook;
using Microsoft.AspNetCore.Mvc;

namespace AMCS.ApiService.Controllers.Plugin
{
  [ApiAuthorize]
  [Route("services/api/webhook")]
  public class WebHookCallbackController : Controller
  {
    [HttpPost]
    [Route("callback")]
    public async Task<IActionResult> Callback(
      [FromQuery] string registrationKey, 
      [FromQuery] string id, 
      [FromQuery] string action,
      [FromQuery] string txid)
    {
      string body;

      await using (var stream = Request.Body)
      using (var reader = new StreamReader(stream))
      {
        body = await reader.ReadToEndAsync();
      }

      DataServices.Resolve<IWebHookService>().ExecuteCallback(new WebHookCallback
      {
        RegistrationKey = registrationKey,
        Id = id,
        Trigger = action,
        TransactionId = txid,
        Body = body
      });

      return NoContent();
    }
  }
}

#endif