import { Injectable } from '@angular/core';
import { ApplicationInsightsProperties } from '@core-module/models/application-insights-properties.interface';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { ApplicationInsightsConfiguration } from '@coremodels/application-insights-configuration.model';
import { BaseService } from '@coreservices/base.service';
import { LoggingVerbs } from '@coreservices/logging/loggingVerbs.model';
import { TrackingEventType } from '@coreservices/logging/tracking-event-type.enum';
import { environment } from '@environments/environment';
import { AppInsights } from 'applicationinsights-js';
import { Observable } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class InstrumentationService extends BaseService {

    private appConfig: ApplicationInsightsConfiguration;
    private timers?: { [name: string]: number } = {};
    private pageViewTimers?: { [name: string]: number } = {};
    private urlComponentNames?: { [name: string]: string } = {};
    private config: Microsoft.ApplicationInsights.IConfig;

    setUpInsights(insightsConfig$: Observable<[ApplicationInsightsConfiguration, ApplicationInsightsProperties]>) {
        insightsConfig$
            .pipe(takeUntil(this.unsubscribe))
            .subscribe((data: [ApplicationInsightsConfiguration, ApplicationInsightsProperties]) => {
                const config: ApplicationInsightsConfiguration = data[0];
                const properties: ApplicationInsightsProperties = data[1];
                if (config.appInsightsOn && (this.config == null || config.instrumentationKey !== this.config.instrumentationKey)) {
                    this.config = { instrumentationKey: config.instrumentationKey };
                    AppInsights.downloadAndSetup(this.config);
                    if (appInsights.queue && appInsights.queue.length === 0) {
                        appInsights.queue.push(function() {
                            appInsights.context.application.ver = environment.applicationVersion;
                            appInsights.context.application.build = environment.applicationVersion;
                        });
                    }
                }
                this.appConfig = config;
                if (this.appConfig) {
                    this.logEvent('Platform Settings Changed', properties);
                }
            });
    }

    setUrlComponentName(url: string, componentName: string) {
        this.urlComponentNames[url] = componentName;
    }

    startTimer(event: string) {
        this.timers[event] = new Date().getTime();
    }

    stopTimer(event: string): number {
        this.timers[event] = new Date().getTime() - this.timers[event];
        return this.timers[event];
    }

    logPageView(name?: string, url?: string, properties?: any,
        measurements?: any, duration?: number) {
        if (this.appConfig && this.appConfig.appInsightsOn) {
            AppInsights.trackPageView(name, url, properties, measurements, duration);
        }
    }

    logEvent(name: string, properties?: any, measurements?: any) {
        if (this.appConfig && this.appConfig.appInsightsOn) {
            AppInsights.trackEvent(name, properties, measurements);
        }

        if (this.appConfig && this.appConfig.consoleLoggingOn) {
            console.log(name, properties);
        }
    }

    startTrackPage(url?: string, name?: string) {
        try {
            const pageIdentifier: string = name ?? url;
            this.pageViewTimers[pageIdentifier] = new Date().getTime();
            if (this.appConfig && this.appConfig.trackingOptionsPageViewLoadTimesUrlBased) {
                if (this.appConfig.appInsightsOn) {
                    AppInsights.startTrackPage(pageIdentifier);
                }
                if (this.appConfig.consoleLoggingOn) {
                    console.log('Start Track Page ' + pageIdentifier + ' ' + AmcsDate.create().toString());
                }
            }
        } catch (ex) {
            console.warn('Angular application insights Error [startTrackPage]: ', ex);
        }
    }

    stopTrackNamedPage(name: string, url: string) {
        try {
            if (this.appConfig && this.pageViewTimers[name]) {
                const duration = new Date().getTime() - this.pageViewTimers[name];
                if (this.appConfig.appInsightsOn) {
                    AppInsights.stopTrackPage(name, url);
                }
                if (this.appConfig.consoleLoggingOn) {
                    console.log(`Stop Track Page ${name}. Duration: ${duration}`);
                }
                delete this.pageViewTimers[name];
            }
        } catch (ex) {
            console.warn('Angular application insights Error [stopTrackNamedPage]: ', ex);
        }
    }

    stopTrackPage(name?: string, url?: string, properties?: { [name: string]: string }, measurements?: { [name: string]: number }) {
        try {

            if (this.appConfig && this.appConfig.appInsightsOn) {

                if (this.pageViewTimers[url]) {
                    let duration = 0;
                    duration = new Date().getTime() - this.pageViewTimers[url];

                    if (this.appConfig.trackingOptionsPageViewLoadTimesUrlBased) {
                        // yes this should be url url as for start and stop track page we only know the URL when we fire the start event
                        AppInsights.stopTrackPage(url, url, properties, measurements);
                    }

                    // this should be the same duration but the name should be resolved to the component name not the url so we
                    // don't see things like /customer/1 in the URL
                    let resolvedName = name;

                    if (this.urlComponentNames[url]) {
                        resolvedName = this.urlComponentNames[url];
                    }

                    if (this.appConfig.trackingOptionspageViewLoadTimesManualComponentNameBased) {
                        this.trackPageView(LoggingVerbs.EventPageManualTimedView + ' ' + resolvedName,
                            url,
                            { name, url, component: resolvedName },
                            { duration },
                            duration
                        );
                    }

                    delete this.pageViewTimers[url];
                }
            }
            if (this.appConfig && this.appConfig.consoleLoggingOn) {
                if (this.appConfig.trackingOptionsPageViewLoadTimesUrlBased) {
                    console.log('Stop Track Page ' + url);
                }
            }
        } catch (ex) {
            console.warn('Angular application insights Error [stopTrackPage]: ', ex);
        }
    }

    trackPageView(name?: string, url?: string, properties?: { [name: string]: string }, measurements?: { [name: string]: number }, duration?: number) {
        try {
            if (this.appConfig && this.appConfig.appInsightsOn) {
                if (duration) {
                    AppInsights.trackPageView(name, url, properties, measurements, duration);
                } else {
                    AppInsights.trackPageView(name, url, properties, measurements);
                }
            }
            if (this.appConfig && this.appConfig.consoleLoggingOn) {
                console.log('Track Page View ' + name);
                if (measurements) {
                    console.log('Duration in ms for  ' + name + ' ' + measurements['duration'].toString());
                }
            }
        } catch (ex) {
            console.warn('Angular application insights Error [stopTrackPage]: ', ex);
        }
    }

    // https://github.com/Microsoft/ApplicationInsights-JS/blob/master/API-reference.md#setauthenticatedusercontext
    // setAuthenticatedUserContext(authenticatedUserId: string, accountId?: string)
    // Set the authenticated user id and the account id in this session. Use this when you have identified a specific
    // signed-in user. Parameters must not contain spaces or ,;=|
    /**
        * Sets the authenticated user id and the account id.
        * User auth id and account id should be of type string. They should not contain commas, semi-colons, equal signs, spaces, or vertical-bars.
        *
        * By default the method will only set the authUserID and accountId for all events in this page view. To add them to all events within
        * the whole session, you should either call this method on every page view or set `storeInCookie = true`.
        *
        * @param authenticatedUserId {string} - The authenticated user id. A unique and persistent string that represents each authenticated user in the service.
        * @param accountId {string} - An optional string to represent the account associated with the authenticated user.
        * @param storeInCookie {boolean} - AuthenticateUserID will be stored in a cookie and added to all events within this session.
        */
    setAuthenticatedUserContext(authenticatedUserId: string, accountId?: string, storeInCookie: boolean = false) {
        try {
            if (this.appConfig && this.appConfig.appInsightsOn) {
                AppInsights.setAuthenticatedUserContext(authenticatedUserId, accountId, storeInCookie);
            }
        } catch (ex) {
            console.warn('Angular application insights Error [setAuthenticatedUserContext]: ', ex);
        }
    }

    // https://github.com/Microsoft/ApplicationInsights-JS/blob/master/API-reference.md#clearauthenticatedusercontext
    // clearAuthenticatedUserContext ()
    // Clears the authenticated user id and the account id from the user context, and clears the associated cookie.
    clearAuthenticatedUserContext() {
        try {
            if (this.appConfig && this.appConfig.appInsightsOn) {
                AppInsights.clearAuthenticatedUserContext();
            }
        } catch (ex) {
            console.warn('Angular application insights Error [clearAuthenticatedUserContext]: ', ex);
        }
    }

    // https://github.com/Microsoft/ApplicationInsights-JS/blob/master/API-reference.md#trackexception
    // trackException(exception: Error, handledAt?: string, properties?: {[string]:string}, measurements?: {[string]:number}, severityLevel?: AI.SeverityLevel)
    // Log an exception you have caught. (Exceptions caught by the browser are also logged.)
    trackException(exception: Error, handledAt?: string, properties?: { [name: string]: string },
        measurements?: { [name: string]: number }, severityLevel?: AI.SeverityLevel) {
        try {
            if (this.appConfig && this.appConfig.appInsightsOn) {
                AppInsights.trackException(exception, handledAt, properties, measurements, severityLevel);
            }
            if (this.appConfig && this.appConfig.consoleLoggingOn) {
                console.error(exception, handledAt, properties);
            }
        } catch (ex) {
            console.warn('Angular application insights Error [trackException]: ', ex);
        }
    }

    trackComponentLifecycleEvent(eventName: string, eventProperties?: { [name: string]: string }, metricProperty?: { [name: string]: number }) {

        this.trackEvent(eventName, eventProperties, metricProperty, TrackingEventType.ComponentLifecycle);
    }

    trackUIInteractionEvent(eventName: string, eventProperties?: { [name: string]: string }, metricProperty?: { [name: string]: number }) {
        if (this.appConfig && this.appConfig.trackingOptionsUiInteractionEvent) {
            this.trackEvent(eventName, eventProperties, metricProperty, TrackingEventType.UiInteraction);
        }
    }

    // https://github.com/Microsoft/ApplicationInsights-JS/blob/master/API-reference.md#trackevent
    // trackEvent(name: string, properties?: {[string]:string}, measurements?: {[string]:number})
    // Log a user action or other occurrence.
    trackEvent(eventName: string, eventProperties?: { [name: string]: string }, metricProperty?: { [name: string]: number }, trackingEventType?: TrackingEventType) {

        let trackEvent = false;
        if (this.appConfig) {

            switch (+trackingEventType) {
                case TrackingEventType.ComponentLifecycle:
                    trackEvent = (this.appConfig.trackingOptionsComponentLifecycle);
                    break;
                case TrackingEventType.UiInteraction:
                    trackEvent = (this.appConfig.trackingOptionsUiInteractionEvent);
                    break;
                case TrackingEventType.Login:
                    trackEvent = (this.appConfig.trackingOptionsLoginEvents);
                    break;

                case TrackingEventType.CustomerContact:
                    trackEvent = (this.appConfig.trackingOptionsCustomerContactEvents);
                    break;
                case TrackingEventType.CustomerServiceLocation:
                    trackEvent = (this.appConfig.trackingOptionsCustomerServiceLocationEvents);
                    break;
                case TrackingEventType.CustomerCalendar:
                    trackEvent = (this.appConfig.trackingOptionsCustomerCalendarEvents);
                    break;
                case TrackingEventType.CustomerOperations:
                    trackEvent = (this.appConfig.trackingOptionsCustomerOperationEvents);
                    break;
                case TrackingEventType.GlobalSearch:
                    trackEvent = (this.appConfig.trackingOptionsGlobalSearchEvents);
                    break;
                case TrackingEventType.GlobalSearchTerm:
                    trackEvent = (this.appConfig.trackingOptionsGlobalSearchTerms);
                    break;
                case TrackingEventType.ScaleSearchTerm:
                    trackEvent = (this.appConfig.trackingOptionsScaleSearchTerms);
                    break;

                default:
                    trackEvent = true;
            }
        }

        if (trackEvent) {
            try {
                if (this.appConfig.appInsightsOn) {
                    AppInsights.trackEvent('Angular UI ' + eventName, eventProperties, metricProperty);
                }
                if (this.appConfig.consoleLoggingOn) {
                    console.log('TET: ' + trackingEventType + ' ' + eventName, eventProperties);
                }
            } catch (ex) {
                console.warn('Angular application insights Error [trackEvent]: ', ex);
            }
        }
    }

    startTrackTimedEvent(name: string): any {
        try {
            this.startTimer(name);
            if (this.appConfig && this.appConfig.appInsightsOn) {
                AppInsights.startTrackEvent(
                    'Angular UI ' + name
                );
            }
            if (this.appConfig && this.appConfig.consoleLoggingOn) {
                console.log('TIMER START: Platform UI ' + name);
            }
        } catch (ex) {
            console.warn('Angular application insights Error [startTrackEvent]: ', ex);
        }
    }

    stopTrackTimedEvent(name: string, properties?: { [p: string]: string }, measurements?: { [p: string]: number }): any {
        try {
            const duration = this.stopTimer(name);
            if (this.appConfig && this.appConfig.appInsightsOn) {
                AppInsights.stopTrackEvent(
                    'Angular UI ' + name,
                    {
                        eventName: name
                    },
                    {
                        duration,
                    }
                );
            }
            if (this.appConfig && this.appConfig.consoleLoggingOn) {
                console.log('TIMER END: Platform UI ' + name + ' duration ' + duration);
            }
        } catch (ex) {
            console.warn('Angular application insights Error [stopTrackEvent]: ', ex);
        }
    }

    // https://github.com/Microsoft/ApplicationInsights-JS/blob/master/API-reference.md#trackmetric
    // trackMetric(name: string, average: number, sampleCount?: number, min?: number, max?: number, properties?: {[string]:string})
    // Log a positive numeric value that is not associated with a specific event.
    // Typically used to send regular reports of performance indicators.
    trackMetric(name: string, average: number, sampleCount?: number, min?: number, max?: number, properties?: { [name: string]: string }) {
        try {
            AppInsights.trackMetric(name, average, sampleCount, min, max, properties);
        } catch (ex) {
            console.warn('Angular application insights Error [trackTrace]: ', ex);
        }
    }
}
