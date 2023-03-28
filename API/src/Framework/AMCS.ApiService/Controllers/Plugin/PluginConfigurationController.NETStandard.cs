#if !NETFRAMEWORK

namespace AMCS.ApiService.Controllers.Plugin
{
  using Responses;
  using Data;
  using AMCS.Data.Configuration;
  using AMCS.Data.Entity.Plugin;
  using Microsoft.AspNetCore.Mvc;
  
  [Route(".well-known/amcs-plugin-configuration")]
  public class PluginConfigurationController : Controller
  {
    [HttpGet]
    public ActionResult GetConfiguration()
    {
      if(!DataServices.TryResolve<IPluginSystem>(out _))
        return NotFound();

      var serviceRoot = DataServices.Resolve<IServiceRootResolver>().GetServiceRoot("PluginApiRoot").TrimEnd('/');
      var mexEndpoint = $"{serviceRoot}/plugin/mex";
      var webHooksEndpoint = $"{serviceRoot}/plugin/web-hooks";
      var response = new PluginConfigurationResponse(mexEndpoint, webHooksEndpoint);

      return Json(response);
    }
  }
}

#endif