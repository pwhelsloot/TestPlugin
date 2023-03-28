import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { ApiRequestTracking } from '@core-module/models/api/api-request-tracking.interface';
import { ApiResponseTracking } from '@core-module/models/api/api-response-tracking.interface';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { ErpApiService } from '@core-module/services/erp-api.service';
import { AiLoggingRequestType } from '@core-module/services/logging/ai-logging-request-type.constants';
import { AiLoggingConstants } from '@core-module/services/logging/ai-logging.constants';
import { AiLoggingService } from '@core-module/services/logging/ai-logging.service';
import { InstrumentationService } from '@core-module/services/logging/instrumentationService.service';
import { Subject } from 'rxjs';

describe('AiLoggingService', () => {
  let service: AiLoggingService;
  let instrumentationService: InstrumentationService;
  let router: Router;
  const mockEnhancedErpService = {} as EnhancedErpApiService;
  const mockErpService = {} as ErpApiService;
  const observer: jasmine.Spy = jasmine.createSpy('AiLoggingService Observer');
  const enhancedRequestTrackingSubject = new Subject<ApiRequestTracking>();
  const enhancedResponseTrackingSubject = new Subject<ApiResponseTracking>();
  const requestTrackingSubject = new Subject<ApiRequestTracking>();
  const responseTrackingSubject = new Subject<ApiResponseTracking>();

  beforeEach(() => {
    mockEnhancedErpService.requestTracking$ = enhancedRequestTrackingSubject;
    mockEnhancedErpService.responseTracking$ = enhancedResponseTrackingSubject;
    mockErpService.requestTracking$ = requestTrackingSubject;
    mockErpService.responseTracking$ = responseTrackingSubject;
    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [
        AiLoggingService,
        InstrumentationService,
        { provide: EnhancedErpApiService, useValue: mockEnhancedErpService },
        { provide: ErpApiService, useValue: mockErpService },
      ],
    });
    service = TestBed.inject(AiLoggingService);
    instrumentationService = TestBed.inject(InstrumentationService);
    router = TestBed.inject(Router);
  });

  afterEach(() => {
    observer.calls.reset();
  });

  it('service is created', () => {
    expect(service).toBeTruthy();
  });

  it('viewInit should start tracking api requests and start AI timers', () => {
    // Arrange
    const viewName = 'test';

    spyOn(instrumentationService, 'startTrackPage').and.callThrough();
    spyOn(instrumentationService, 'startTrackTimedEvent').and.callThrough();

    // Act
    service.viewInit(viewName);

    // Assert
    expect(instrumentationService.startTrackPage).toHaveBeenCalledOnceWith(router.url, viewName);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(2);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledWith(`${viewName} - ${AiLoggingConstants.FirstPaint}`);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledWith(`${viewName} - ${AiLoggingConstants.Ready}`);

    // This tests trackApiRequestsForCurrentView has been called
    enhancedRequestTrackingSubject.next({ id: 1, url: 'test', type: AiLoggingRequestType.GET });

    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(3);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledWith(
      `${viewName} - ${AiLoggingConstants.Api} ${AiLoggingRequestType.GET} - test`
    );

    requestTrackingSubject.next({ id: 2, url: 'test2', type: AiLoggingRequestType.POST });

    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(4);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledWith(
      `${viewName} - ${AiLoggingConstants.Api} ${AiLoggingRequestType.POST} - test2`
    );
  });

  it('viewReady should call stopTrackTimedEvent', () => {
    // Arrange
    const viewName = 'test';

    spyOn(instrumentationService, 'stopTrackTimedEvent').and.callThrough();

    // Act
    service.viewReady(viewName);

    // Assert
    expect(instrumentationService.stopTrackTimedEvent).toHaveBeenCalledOnceWith(`${viewName} - ${AiLoggingConstants.Ready}`);
  });

  it('viewFirstRender should call stopTrackTimedEvent', () => {
    // Arrange
    const viewName = 'test';

    spyOn(instrumentationService, 'stopTrackTimedEvent').and.callThrough();

    // Act
    service.viewFirstRender(viewName);

    // Assert
    expect(instrumentationService.stopTrackTimedEvent).toHaveBeenCalledOnceWith(`${viewName} - ${AiLoggingConstants.FirstPaint}`);
  });

  it('if viewDestroyed is currentView then should stop tracking api requests', () => {
    // Arrange
    const viewName = 'test';

    spyOn(instrumentationService, 'startTrackPage').and.callThrough();
    spyOn(instrumentationService, 'startTrackTimedEvent').and.callThrough();
    spyOn(instrumentationService, 'stopTrackNamedPage').and.callThrough();

    // Act
    service.viewInit(viewName);

    // Assert
    expect(instrumentationService.startTrackPage).toHaveBeenCalledOnceWith(router.url, viewName);

    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(2);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledWith(`${viewName} - ${AiLoggingConstants.FirstPaint}`);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledWith(`${viewName} - ${AiLoggingConstants.Ready}`);
    expect(instrumentationService.stopTrackNamedPage).toHaveBeenCalledTimes(0);

    service.viewDestroyed(viewName);

    expect(instrumentationService.stopTrackNamedPage).toHaveBeenCalledOnceWith(viewName, router.url);

    // This tests stopTrackingApiRequests has been called as startTrackTimedEvent should not trigger again
    enhancedRequestTrackingSubject.next({ id: 1, url: 'test', type: AiLoggingRequestType.GET });

    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(2);

    requestTrackingSubject.next({ id: 2, url: 'test2', type: AiLoggingRequestType.POST });

    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(2);
  });

  it('if viewDestroyed is not currentView then should not stop tracking api requests', () => {
    // Arrange
    const viewName = 'test';
    const newViewName = 'test2';

    spyOn(instrumentationService, 'startTrackPage').and.callThrough();
    spyOn(instrumentationService, 'startTrackTimedEvent').and.callThrough();
    spyOn(instrumentationService, 'stopTrackNamedPage').and.callThrough();

    // Act

    // currentView = viewName
    service.viewInit(viewName);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(2);

    // currentView = newViewName
    service.viewInit(newViewName);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(4);

    // Asset
    expect(instrumentationService.stopTrackNamedPage).toHaveBeenCalledTimes(0);

    // Destroy the original view (current view is newViewName)
    service.viewDestroyed(viewName);
    expect(instrumentationService.stopTrackNamedPage).toHaveBeenCalledOnceWith(viewName, router.url);

    // This tests api request tracking is still active for newViewName
    enhancedRequestTrackingSubject.next({ id: 1, url: 'test', type: AiLoggingRequestType.GET });

    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(5);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledWith(
      `${newViewName} - ${AiLoggingConstants.Api} ${AiLoggingRequestType.GET} - test`
    );

    requestTrackingSubject.next({ id: 2, url: 'test2', type: AiLoggingRequestType.POST });

    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(6);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledWith(
      `${newViewName} - ${AiLoggingConstants.Api} ${AiLoggingRequestType.POST} - test2`
    );
  });

  it('if viewDestroyed is currentView then previousView should be become currentView', () => {
    // Arrange
    const viewName = 'test';
    const newViewName = 'test2';

    spyOn(instrumentationService, 'startTrackPage').and.callThrough();
    spyOn(instrumentationService, 'startTrackTimedEvent').and.callThrough();
    spyOn(instrumentationService, 'stopTrackNamedPage').and.callThrough();

    // Act

    // currentView = viewName
    service.viewInit(viewName);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(2);

    // currentView = newViewName
    service.viewInit(newViewName);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(4);

    // Asset
    expect(instrumentationService.stopTrackNamedPage).toHaveBeenCalledTimes(0);

    // Destroy the current view (this will make currentView = viewName)
    service.viewDestroyed(newViewName);
    expect(instrumentationService.stopTrackNamedPage).toHaveBeenCalledOnceWith(newViewName, router.url);

    // This tests api request tracking is now active for viewName
    enhancedRequestTrackingSubject.next({ id: 1, url: 'test', type: AiLoggingRequestType.POST });

    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(5);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledWith(
      `${viewName} - ${AiLoggingConstants.Api} ${AiLoggingRequestType.POST} - test`
    );

    requestTrackingSubject.next({ id: 2, url: 'test2', type: AiLoggingRequestType.GET });

    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(6);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledWith(
      `${viewName} - ${AiLoggingConstants.Api} ${AiLoggingRequestType.GET} - test2`
    );
  });

  it('if service is destoryed then should not stop tracking api requests', () => {
    // Arrange
    const viewName = 'test';
    const newViewName = 'test2';

    spyOn(instrumentationService, 'startTrackPage').and.callThrough();
    spyOn(instrumentationService, 'startTrackTimedEvent').and.callThrough();
    spyOn(instrumentationService, 'stopTrackNamedPage').and.callThrough();

    // Act

    // currentView = viewName
    service.viewInit(viewName);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(2);

    // currentView = newViewName
    service.viewInit(newViewName);
    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(4);

    // Asset
    expect(instrumentationService.stopTrackNamedPage).toHaveBeenCalledTimes(0);

    // Destroy service
    service.ngOnDestroy();

    // This tests api request tracking is not active
    enhancedRequestTrackingSubject.next({ id: 1, url: 'test', type: AiLoggingRequestType.POST });

    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(4);

    requestTrackingSubject.next({ id: 2, url: 'test2', type: AiLoggingRequestType.GET });

    expect(instrumentationService.startTrackTimedEvent).toHaveBeenCalledTimes(4);
  });
});
