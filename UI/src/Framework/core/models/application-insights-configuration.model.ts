import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { alias } from '@coreconfig/api-dto-alias.function';

@amcsJsonObject()
export class ApplicationInsightsConfiguration extends ApiBaseModel {

    @alias('InstrumentationKey')
    @amcsJsonMember('InstrumentationKey')
    instrumentationKey: string;

    @alias('AppInsightsOn')
    @amcsJsonMember('AppInsightsOn')
    appInsightsOn: boolean;

    @alias('ConsoleLoggingOn')
    @amcsJsonMember('ConsoleLoggingOn')
    consoleLoggingOn: boolean;

    @alias('TrackingOptionsPageViewLoadTimesUrlBased')
    @amcsJsonMember('TrackingOptionsPageViewLoadTimesUrlBased')
    trackingOptionsPageViewLoadTimesUrlBased: boolean;

    @alias('TrackingOptionspageViewLoadTimesManualComponentNameBased')
    @amcsJsonMember('TrackingOptionspageViewLoadTimesManualComponentNameBased')
    trackingOptionspageViewLoadTimesManualComponentNameBased: boolean;

    @alias('TrackingOptionscomponentLifecycle')
    @amcsJsonMember('TrackingOptionscomponentLifecycle')
    trackingOptionsComponentLifecycle: boolean;

    @alias('TrackingOptionsuiInteractionEvent')
    @amcsJsonMember('TrackingOptionsuiInteractionEvent')
    trackingOptionsUiInteractionEvent: boolean;

    @alias('TrackingOptionsLoginEvents')
    @amcsJsonMember('TrackingOptionsLoginEvents')
    trackingOptionsLoginEvents: boolean;

    @alias('TrackingOptionsDesktopEvents')
    @amcsJsonMember('TrackingOptionsDesktopEvents')
    trackingOptionsDesktopEvents: boolean;

    @alias('TrackingOptionsGlobalSearchEvents')
    @amcsJsonMember('TrackingOptionsGlobalSearchEvents')
    trackingOptionsGlobalSearchEvents: boolean;

    @alias('TrackingOptionsGlobalSearchTerms')
    @amcsJsonMember('TrackingOptionsGlobalSearchTerms')
    trackingOptionsGlobalSearchTerms: boolean;

    @alias('TrackingOptionsCustomerContactEvents')
    @amcsJsonMember('TrackingOptionsCustomerContactEvents')
    trackingOptionsCustomerContactEvents: boolean;

    @alias('TrackingOptionsCustomerCalendarEvents')
    @amcsJsonMember('TrackingOptionsCustomerCalendarEvents')
    trackingOptionsCustomerCalendarEvents: boolean;

    @alias('TrackingOptionsCustomerOperationEvents')
    @amcsJsonMember('TrackingOptionsCustomerOperationEvents')
    trackingOptionsCustomerOperationEvents: boolean;

    @alias('TrackingOptionsCustomerServiceLocationEvents')
    @amcsJsonMember('TrackingOptionsCustomerServiceLocationEvents')
    trackingOptionsCustomerServiceLocationEvents: boolean;

    @alias('TrackingOptionsScaleSearchTerms')
    @amcsJsonMember('TrackingOptionsScaleSearchTerms')
    trackingOptionsScaleSearchTerms: boolean;
}
