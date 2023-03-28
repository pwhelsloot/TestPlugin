using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.Configuration;

namespace AMCS.ApiService.Controllers.Responses
{
  public class ApplicationInsightsConfiguration : IPlatformUIApplicationInsightsConfiguration
  {
    private readonly IPlatformUIApplicationInsightsConfiguration wrapped;

    public ApplicationInsightsConfiguration(IPlatformUIApplicationInsightsConfiguration wrapped)
    {
      this.wrapped = wrapped;
    }

    public string InstrumentationKey { get; set; }

    public bool AppInsightsOn => wrapped.AppInsightsOn;

    public bool ConsoleLoggingOn => wrapped.ConsoleLoggingOn;

    public bool TrackingOptionsPageViewLoadTimesUrlBased => wrapped.TrackingOptionsPageViewLoadTimesUrlBased;

    public bool TrackingOptionsPageViewLoadTimesManualComponentNameBased => wrapped.TrackingOptionsPageViewLoadTimesManualComponentNameBased;

    public bool TrackingOptionsComponentLifecycle => wrapped.TrackingOptionsComponentLifecycle;

    public bool TrackingOptionsUIInteractionEvent => wrapped.TrackingOptionsUIInteractionEvent;

    public bool TrackingOptionsLoginEvents => wrapped.TrackingOptionsLoginEvents;

    public bool TrackingOptionsDesktopEvents => wrapped.TrackingOptionsDesktopEvents;

    public bool TrackingOptionsGlobalSearchEvents => wrapped.TrackingOptionsGlobalSearchEvents;

    public bool TrackingOptionsGlobalSearchTerms => wrapped.TrackingOptionsGlobalSearchTerms;

    public bool TrackingOptionsCustomerContactEvents => wrapped.TrackingOptionsCustomerContactEvents;

    public bool TrackingOptionsCustomerCalendarEvents => wrapped.TrackingOptionsCustomerCalendarEvents;

    public bool TrackingOptionsCustomerOperationEvents => wrapped.TrackingOptionsCustomerOperationEvents;

    public bool TrackingOptionsCustomerServiceLocationEvents => wrapped.TrackingOptionsCustomerServiceLocationEvents;
  }
}
