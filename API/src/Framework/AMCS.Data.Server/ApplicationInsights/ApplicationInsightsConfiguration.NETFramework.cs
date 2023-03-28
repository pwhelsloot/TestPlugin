#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Configuration;
using AMCS.Data.Server.Services;
using Microsoft.ApplicationInsights.Extensibility;

namespace AMCS.Data.Server.ApplicationInsights
{
  internal class ApplicationInsightsConfiguration : IDelayedStartup
  {
    private readonly string applicationInsightsInstrumentationKey;

    public ApplicationInsightsConfiguration(IAppSetupService setupService, string applicationInsightsInstrumentationKey)
    {
      this.applicationInsightsInstrumentationKey = applicationInsightsInstrumentationKey;
    }

    public void Start()
    {
      if (string.IsNullOrWhiteSpace(TelemetryConfiguration.Active.InstrumentationKey) && !string.IsNullOrWhiteSpace(applicationInsightsInstrumentationKey))
        TelemetryConfiguration.Active.InstrumentationKey = applicationInsightsInstrumentationKey;
      if (string.IsNullOrWhiteSpace(TelemetryConfiguration.Active.InstrumentationKey))
        TelemetryConfiguration.Active.DisableTelemetry = true;
    }
  }
}
#endif
