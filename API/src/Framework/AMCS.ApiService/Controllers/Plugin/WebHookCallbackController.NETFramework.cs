#if NETFRAMEWORK

using System;
using System.IO;
using System.Web.Mvc;
using System.Net;
using AMCS.Data;
using AMCS.Data.Server.WebHook;

namespace AMCS.ApiService.Controllers.Plugin
{
  [Authenticated]
  [Route("services/api/webhook/callback")]
  public class WebHookCallbackController : Controller
  {
    [HttpPost]
    public ActionResult Callback(
      string registrationKey, 
      string id,
      string action,
      string txid)
    {
      string body;

      Request.InputStream.Position = 0;

      using (var stream = Request.InputStream)
      using (var reader = new StreamReader(stream))
      {
        body = reader.ReadToEnd();
      }

      DataServices.Resolve<IWebHookService>().ExecuteCallback(new WebHookCallback
      {
        RegistrationKey = registrationKey,
        Id = id,
        Trigger = action,
        TransactionId = txid,
        Body = body
      });

      return new HttpStatusCodeResult(HttpStatusCode.NoContent);
    }
  }
}

#endif