#if NETFRAMEWORK

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AMCS.ApiService.Controllers.Responses;
using AMCS.Data;
using AMCS.Data.Server.Configuration;
using Swashbuckle.Swagger.Annotations;

namespace AMCS.ApiService.Controllers
{
  [Route("applicationUISettings")]
  public class ApplicationUISettingsController : Controller
  {
    [HttpGet]
    [Authenticated]
    [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ApplicationUISettingsResponse))]
    public ActionResult Get()
    {
      var appSettings = DataServices.Resolve<IPlatformUIConfiguration>();
      var response = new ApplicationUISettingsResponse
      {
        ApplicationConfiguration = new ApplicationConfiguration(appSettings.ApplicationSettings),
        ApplicationInsightsConfiguration = new ApplicationInsightsConfiguration(appSettings.ApplicationInsightsSettings),
        GeneralConfiguration = new GeneralConfiguration(appSettings.General),
        MapProviderConfiguration = new MapProvidersConfiguration(appSettings.MapProviders),
        DeveloperButtonsConfiguration = new DeveloperConfiguration(appSettings.DeveloperSettings),
        FeatureToggleConfiguration = new FeatureToggleConfiguration(appSettings.FeatureToggleSettings)
      };

      return Json(response, JsonRequestBehavior.AllowGet);
    }
  }
}

#endif
