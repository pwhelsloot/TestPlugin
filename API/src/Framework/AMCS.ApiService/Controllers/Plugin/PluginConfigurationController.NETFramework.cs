#if NETFRAMEWORK

namespace AMCS.ApiService.Controllers.Plugin
{
  using System.Text.Json.Serialization;
  using System.Web.Mvc;
  using AMCS.ApiService.Controllers.Responses;
  using AMCS.Data;
  using AMCS.Data.Configuration;
  using AMCS.Data.Entity.Plugin;
  using Newtonsoft.Json;

  [Route(".well-known/amcs-plugin-configuration")]
  public class PluginConfigurationController : Controller
  {
    [HttpGet]
    public ActionResult GetConfiguration()
    {
      if(!DataServices.TryResolve<IPluginSystem>(out _))
        return HttpNotFound();

      var serviceRoot = DataServices.Resolve<IServiceRootResolver>().GetServiceRoot("PluginApiRoot").TrimEnd('/');
      var mexEndpoint = $"{serviceRoot}/plugin/mex";
      var webHooksEndpoint = $"{serviceRoot}/plugin/web-hooks";
      var response = JsonConvert.SerializeObject(new PluginConfigurationResponse(mexEndpoint, webHooksEndpoint));

      return Content(response,"application/json");
    }
  }
}

#endif