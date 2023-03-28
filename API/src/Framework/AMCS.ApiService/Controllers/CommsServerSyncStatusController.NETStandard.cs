#if !NETFRAMEWORK
namespace AMCS.ApiService.Controllers
{
  using AMCS.ApiService.CommsServer;
  using AMCS.Data;
  using Microsoft.AspNetCore.Mvc;

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
        return NotFound();
      }
    }
  }
}
#endif