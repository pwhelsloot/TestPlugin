import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { BaseService } from '@core-module/services/base.service';
import { ErrorNotificationService } from '@core-module/services/error-notification.service';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { merge, Observable } from 'rxjs';
import { shareReplay, takeUntil } from 'rxjs/operators';
import { WorkflowNavRoute } from '@workflow/models/workflow-nav-route.model';
import { WorkflowResponse } from '@workflow/models/workflow-response.model';
import { WorkflowRunRequest } from '@workflow/models/workflow-run-request.model';
import { WorkflowStartRequest } from '@workflow/models/workflow-start-request.model';
import { WorkflowRunService } from './workflow-run.service';
import { WorkflowStartService } from './workflow-start.service';

@Injectable()
export class WorkflowControllerService extends BaseService {

  static providers = [WorkflowControllerService, WorkflowStartService, WorkflowRunService];

  combinedPostMessageResult$: Observable<WorkflowResponse>;
  navRoutes: WorkflowNavRoute[];

  constructor(private startService: WorkflowStartService,
              private runService: WorkflowRunService,
              private router: Router,
              private errorNotificationService: ErrorNotificationService,
              private translationsService: SharedTranslationsService) {
    super();
    this.combinedPostMessageResult$ = merge(startService.postMessageResult$,
                                            runService.postMessageResult$)
                                            .pipe(shareReplay(1));
    this.setupWorkflowResponseStream();
  }

  startWorkflow(startRequest: WorkflowStartRequest, navRoutes: WorkflowNavRoute[]) {
    this.navRoutes = navRoutes;
    this.startService.postMessage(startRequest);
  }

  runWorkflow(runRequest: WorkflowRunRequest, navRoutes: WorkflowNavRoute[]) {
    this.navRoutes = navRoutes;
    this.runService.postMessage(runRequest);
  }

  private setupWorkflowResponseStream() {
    this.combinedPostMessageResult$.pipe(
        takeUntil(this.unsubscribe),
      )
      .subscribe(response => {
        this.navigate(response.currentStep);
      });
  }

  private navigate(step: string) {
    const route = this.navRoutes.find(r => r.name === step);

    if (isTruthy(route)) {
      if (this.checkContainsQueryString(route.url)) {
        this.router.navigateByUrl(route.url);
      } else {
        this.router.navigate([route.url]);
      }
    } else {
      this.errorNotificationService.notifyError(this.translationsService.getTranslation('workflow.missingRoute'));
    }
  }

  private checkContainsQueryString(input: string) {
    return input.indexOf('?') !== -1;
  }

}
