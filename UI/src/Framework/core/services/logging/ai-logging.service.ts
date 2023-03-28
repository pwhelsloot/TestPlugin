import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ApiRequestTracking } from '@core-module/models/api/api-request-tracking.interface';
import { ApiResponseTracking } from '@core-module/models/api/api-response-tracking.interface';
import { BaseService } from '@core-module/services/base.service';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { ErpApiService } from '@core-module/services/erp-api.service';
import { AiLoggingConstants } from '@core-module/services/logging/ai-logging.constants';
import { AiViewApiRequest } from '@core-module/services/logging/ai-view-api-request.interface';
import { InstrumentationService } from '@core-module/services/logging/instrumentationService.service';
import { takeUntil } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AiLoggingService extends BaseService {
  // This is assigned by the AiLoggingWrapper and allows us to use it in the ai decorators
  static aiLoggingServiceReference: AiLoggingService;

  constructor(
    private readonly router: Router,
    private readonly enhancedErpApiService: EnhancedErpApiService,
    private readonly erpApiService: ErpApiService,
    private readonly instrumentationService: InstrumentationService
  ) {
    super();
  }

  private previousViews = new Array<string>();
  private currentView: string;
  private readonly viewUrlsMap = new Map<string, string>();
  private readonly enhancedApiRequests = new Map<number, AiViewApiRequest>();
  private readonly apiRequests = new Map<number, AiViewApiRequest>();

  /**
   * Starts page tracking, first paint and ready timers.
   * Initialised view becomes current View and previous current view pushed on previousViews array.
   * Starts tracking API calls for the current view.
   * @param viewName The view which is initialised
   */
  viewInit(viewName: string): void {
    this.viewUrlsMap.set(viewName, this.router.url);
    this.stopTrackingApiRequests();
    if (this.currentView) {
      this.previousViews.push(this.currentView);
    }
    this.currentView = viewName;
    this.trackApiRequestsForCurrentView();
    this.instrumentationService.startTrackPage(this.router.url, viewName);
    this.instrumentationService.startTrackTimedEvent(`${viewName} - ${AiLoggingConstants.FirstPaint}`);
    this.instrumentationService.startTrackTimedEvent(`${viewName} - ${AiLoggingConstants.Ready}`);
  }

  /**
   * Stops first paint timer and logs to AI.
   * @param viewName The view which is painted
   */
  viewFirstRender(viewName: string): void {
    this.instrumentationService.stopTrackTimedEvent(`${viewName} - ${AiLoggingConstants.FirstPaint}`);
  }

  /**
   * Stops view ready timer and logs to AI.
   * @param viewName The view which is ready
   */
  viewReady(viewName: string): void {
    this.instrumentationService.stopTrackTimedEvent(`${viewName} - ${AiLoggingConstants.Ready}`);
  }

  /**
   * Stops page tracking timer and logs to AI. If view is current view then stop listening for api requests.
   * If previous view exists then make it the new current view and start listening for api requests.
   * @param viewName The view being destroyed
   */
  viewDestroyed(viewName: string): void {
    this.instrumentationService.stopTrackNamedPage(viewName, this.viewUrlsMap.get(viewName));
    this.viewUrlsMap.delete(viewName);
    if (this.currentView === viewName) {
      this.stopTrackingApiRequests();
      this.currentView = this.previousViews.pop();
      if (this.currentView) {
        this.trackApiRequestsForCurrentView();
      }
    } else {
      this.previousViews = this.previousViews.filter((x) => x !== viewName);
    }
  }

  /**
   * Tracks all API calls for the current view.
   */
  private trackApiRequestsForCurrentView(): void {
    this.trackApiRequests();
    this.trackApiResponses();
  }

  /**
   * Tracks all API requests from the Enhanced + Legacy ErpApiServices.
   */
  private trackApiRequests() {
    this.enhancedErpApiService.requestTracking$.pipe(takeUntil(this.unsubscribe)).subscribe((trackingData: ApiRequestTracking) => {
      this.instrumentationService.startTrackTimedEvent(
        `${this.currentView} - ${AiLoggingConstants.Api} ${trackingData.type} - ${trackingData.url}`
      );
      this.enhancedApiRequests.set(trackingData.id, { viewName: this.currentView, ...trackingData });
    });
    this.erpApiService.requestTracking$.pipe(takeUntil(this.unsubscribe)).subscribe((trackingData: ApiRequestTracking) => {
      this.instrumentationService.startTrackTimedEvent(
        `${this.currentView} - ${AiLoggingConstants.Api} ${trackingData.type} - ${trackingData.url}`
      );
      this.apiRequests.set(trackingData.id, { viewName: this.currentView, ...trackingData });
    });
  }

  /**
   * Tracks all API responses from the Enhanced + Legacy ErpApiServices.
   */
  private trackApiResponses() {
    this.enhancedErpApiService.responseTracking$.pipe(takeUntil(this.unsubscribe)).subscribe((trackingData: ApiResponseTracking) => {
      const viewApiRequest: AiViewApiRequest = this.enhancedApiRequests.get(trackingData.id);
      if (viewApiRequest) {
        this.instrumentationService.stopTrackTimedEvent(
          `${viewApiRequest.viewName} - ${AiLoggingConstants.Api} ${viewApiRequest.type} - ${viewApiRequest.url}`
        );
        this.enhancedApiRequests.delete(trackingData.id);
      }
    });
    this.erpApiService.responseTracking$.pipe(takeUntil(this.unsubscribe)).subscribe((trackingData: ApiResponseTracking) => {
      const viewApiRequest: AiViewApiRequest = this.apiRequests.get(trackingData.id);
      if (viewApiRequest) {
        this.instrumentationService.stopTrackTimedEvent(
          `${viewApiRequest.viewName} - ${AiLoggingConstants.Api} ${viewApiRequest.type} - ${viewApiRequest.url}`
        );
        this.apiRequests.delete(trackingData.id);
      }
    });
  }

  private stopTrackingApiRequests(): void {
    this.unsubscribe.next();
  }
}
