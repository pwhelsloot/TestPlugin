import { TestBed } from '@angular/core/testing';
import { NavigationBehaviorOptions, NavigationExtras, Router, UrlTree } from '@angular/router';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Observable, of, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { WorkflowNavRoute } from '@workflow/models/workflow-nav-route.model';
import { WorkflowResponse } from '@workflow/models/workflow-response.model';
import { WorkflowRunRequest } from '@workflow/models/workflow-run-request.model';
import { WorkflowStartRequest } from '@workflow/models/workflow-start-request.model';
import { WorkflowControllerService } from './workflow-controller.service';

describe('Service: WorkflowControllerService', () => {
  let workflowControllerService: WorkflowControllerService;
  const destroy: Subject<void> = new Subject();
  const observer: jasmine.Spy = jasmine.createSpy('WorkflowControllerService Observer');
  const testScheme = 'Test Scheme';
  const instanceGuid = 'This is a guid';
  const stepName = 'Test Step';
  const stepName2 = 'Test Step 2';
  const url1 = 'Test Url';
  const url2 = '?Test Url 2';
  const startViewData = 'Start ViewData';
  const runViewData = 'Run ViewData';

  let navigateCount = 0;
  let navigateByUrlCount = 0;

  beforeEach(() => {
    const testService = () => ({
      postMessage: (apiRequest, object, type, returnType): Observable<WorkflowResponse> => {
        const result = new WorkflowResponse();
        if (object instanceof WorkflowStartRequest) {
          result.instanceGuid = instanceGuid;
          result.currentStep = stepName;
          result.viewData = startViewData;
        } else {
          result.instanceGuid = instanceGuid;
          result.currentStep = stepName2;
          result.viewData = runViewData;
        }

        return of(result);
      }
    });

    const routerService = () => ({
      navigateByUrl: (url: string | UrlTree, extras?: NavigationBehaviorOptions): void => { navigateByUrlCount++; },
      navigate: (commands: any[], extras?: NavigationExtras): void => { navigateCount++; }
    });

    const translationsService = () => ({
      getTranslation(translationKey: string) { }
    });

    TestBed.configureTestingModule({
      providers: [WorkflowControllerService,
                  WorkflowControllerService.providers,
                  { provide: SharedTranslationsService, useFactory: translationsService },
                  { provide: EnhancedErpApiService, useFactory: testService },
                  { provide: Router, useFactory: routerService }]

    });

    workflowControllerService = TestBed.inject(WorkflowControllerService);
  });

  afterEach(() => {
    destroy.next();
    observer.calls.reset();
  });

  afterAll(() => {
    destroy.complete();
  });

  it('should start a workflow', () => {
    //arrange
    workflowControllerService.combinedPostMessageResult$.pipe(takeUntil(destroy)).subscribe(observer);

    let startRequest = new WorkflowStartRequest();
    startRequest.schemeName = testScheme;

    let navRoute = new WorkflowNavRoute();
    navRoute.name = stepName;
    navRoute.url = url1;

    let navRoute2 = new WorkflowNavRoute();
    navRoute2.name = stepName2;
    navRoute2.url = url2;

    let navRoutes = [];
    navRoutes.push(navRoute);
    navRoutes.push(navRoute2);

    let originalNavigateCount = navigateCount;

    //act
    workflowControllerService.startWorkflow(startRequest, navRoutes);

    //assert
    const result = new WorkflowResponse();
    result.instanceGuid = instanceGuid;
    result.currentStep = stepName;
    result.viewData = startViewData;
    expect(observer).toHaveBeenCalledOnceWith(result);
    expect(navigateCount).toBe(originalNavigateCount + 1);
  });

  it('should run a workflow', () => {
    //arrange
    workflowControllerService.combinedPostMessageResult$.pipe(takeUntil(destroy)).subscribe(observer);

    let runRequest = new WorkflowRunRequest();
    runRequest.instanceGuid = instanceGuid;

    let navRoute = new WorkflowNavRoute();
    navRoute.name = stepName;
    navRoute.url = url1;

    let navRoute2 = new WorkflowNavRoute();
    navRoute2.name = stepName2;
    navRoute2.url = url2;

    let navRoutes = [];
    navRoutes.push(navRoute);
    navRoutes.push(navRoute2);

    let originalNavigateByUrlCount = navigateByUrlCount;

    //act
    workflowControllerService.runWorkflow(runRequest, navRoutes);

    //assert
    const result = new WorkflowResponse();
    result.instanceGuid = instanceGuid;
    result.currentStep = stepName2;
    result.viewData = runViewData;
    expect(observer).toHaveBeenCalledOnceWith(result);
    expect(navigateByUrlCount).toBe(originalNavigateByUrlCount + 1);
  });

  it('should call correct navigate methods', () => {
    //arrange
    workflowControllerService.combinedPostMessageResult$.pipe(takeUntil(destroy)).subscribe(observer);

    let startRequest = new WorkflowStartRequest();
    startRequest.schemeName = testScheme;

    let runRequest = new WorkflowRunRequest();
    runRequest.instanceGuid = instanceGuid;

    let navRoute = new WorkflowNavRoute();
    navRoute.name = stepName;
    navRoute.url = url1;

    let navRoute2 = new WorkflowNavRoute();
    navRoute2.name = stepName2;
    navRoute2.url = url2;

    let navRoutes = [];
    navRoutes.push(navRoute);
    navRoutes.push(navRoute2);

    let originalNavigateCount = navigateCount;
    let originalNavigateByUrlCount = navigateByUrlCount;

    //act
    workflowControllerService.startWorkflow(startRequest, navRoutes);
    workflowControllerService.runWorkflow(runRequest, navRoutes);
    workflowControllerService.runWorkflow(runRequest, navRoutes);

    //assert
    expect(navigateCount).toBe(originalNavigateCount + 1);
    expect(navigateByUrlCount).toBe(originalNavigateByUrlCount + 2);
  });

  it('should rteurn the correct request', () => {
    //arrange
    workflowControllerService.combinedPostMessageResult$.pipe(takeUntil(destroy)).subscribe(observer);

    let startRequest = new WorkflowStartRequest();
    startRequest.schemeName = testScheme;

    let runRequest = new WorkflowRunRequest();
    runRequest.instanceGuid = instanceGuid;

    let navRoute = new WorkflowNavRoute();
    navRoute.name = stepName;
    navRoute.url = url1;

    let navRoute2 = new WorkflowNavRoute();
    navRoute2.name = stepName2;
    navRoute2.url = url2;

    let navRoutes = [];
    navRoutes.push(navRoute);
    navRoutes.push(navRoute2);

    //act
    workflowControllerService.startWorkflow(startRequest, navRoutes);

    //assert
    const startResult = new WorkflowResponse();
    startResult.instanceGuid = instanceGuid;
    startResult.currentStep = stepName;
    startResult.viewData = startViewData;

    expect(observer).toHaveBeenCalledOnceWith(startResult);

    //act
    workflowControllerService.runWorkflow(runRequest, navRoutes);

    //assert
    const runResult = new WorkflowResponse();
    runResult.instanceGuid = instanceGuid;
    runResult.currentStep = stepName2;
    runResult.viewData = runViewData;

    expect(observer).toHaveBeenCalledWith(runResult);
    expect(observer).toHaveBeenCalledTimes(2);

  });
});
