#if NETFRAMEWORK
namespace AMCS.ApiService.Controllers
{
  using System.Web.Mvc;
  using AMCS.ApiService.CommsServer;
  using AMCS.Data;

  [Route("commsServerSyncStatus")]
  public class CommsServerSyncStatusController : Controller
  {
    [HttpGet]
    public ActionResult Get()
    {
      if (DataServices.TryResolve<ICommsServerSyncStatusService>(out var service))
      {
        return Json(service.GetSyncStatus());
      }
      else
      {
        return HttpNotFound();
      }
    }
  }
}
#endif