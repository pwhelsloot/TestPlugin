#if !NETFRAMEWORK

using System;
using AMCS.ApiService.Controllers.Responses;
using AMCS.Data;
using AMCS.Data.Server.Configuration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AMCS.ApiService.Controllers
{
  [Route("applicationUISettings")]
  public class ApplicationUISettingsController : Controller
  {
    [HttpGet]
    [ApiAuthorize]
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

      return Json(response, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
    }
  }
}

#endif
