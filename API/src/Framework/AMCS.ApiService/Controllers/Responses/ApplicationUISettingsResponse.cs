using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.Configuration;

namespace AMCS.ApiService.Controllers.Responses
{
  public class ApplicationUISettingsResponse
  {
    public IPlatformUIApplicationConfiguration ApplicationConfiguration { get; set; }

    public IPlatformUIApplicationInsightsConfiguration ApplicationInsightsConfiguration { get; set; }

    public IPlatformUIMapProvidersConfiguration MapProviderConfiguration { get; set; }

    public IPlatformUIGeneralConfiguration GeneralConfiguration { get; set; }

    public IPlatformUIDeveloperConfiguration DeveloperButtonsConfiguration { get; set; }

    public IPlatformUIFeatureToggleConfiguration FeatureToggleConfiguration { get; set; }
  }
}
