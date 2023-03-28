#if !NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.Services;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.Extensions.DependencyInjection;

namespace AMCS.Data.Server.ApplicationInsights
{
  internal class ApplicationInsightsConfiguration
  {
    public ApplicationInsightsConfiguration(IAppSetupService setupService, string applicationInsightsInstrumentationKey)
    {
      if (!string.IsNullOrEmpty(applicationInsightsInstrumentationKey))
      {
        setupService.RegisterConfigureServices(
          services =>
          {
            services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
            {
              module.EnableSqlCommandTextInstrumentation = true;
            });

            services.AddApplicationInsightsTelemetry(options =>
            {
              options.InstrumentationKey = applicationInsightsInstrumentationKey;
              options.EnableActiveTelemetryConfigurationSetup = true;
              options.RequestCollectionOptions.TrackExceptions = true;
              options.RequestCollectionOptions.InjectResponseHeaders = false;

#if DEBUG
            options.DeveloperMode = true;
#endif
          });
          },
          -2000);
      }
    }
  }
}
#endif
