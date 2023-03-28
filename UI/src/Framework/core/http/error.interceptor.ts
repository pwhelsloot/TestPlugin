import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable, isDevMode } from '@angular/core';
import { Router } from '@angular/router';
import * as AuthActions from '@auth-module/store/auth.actions';
import { CoreAppRoutes } from '@core-module/config/routes/core-app-routes.constants';
import { GlossaryLanguage } from '@core-module/models/glossary/glossary-language.model';
import * as fromApp from '@core-module/store/app.reducers';
import { InstrumentationService } from '@coreservices/logging/instrumentationService.service';
import { LoggingVerbs } from '@coreservices/logging/loggingVerbs.model';
import { PreviousRouteService } from '@coreservices/previous-route.service';
import { Store } from '@ngrx/store';
import { AmcsModalService } from '@shared-module/components/amcs-modal/amcs-modal.service';
import { ErrorModalComponent } from '@shared-module/components/layouts/error-modal/error-modal.component';
import { EMPTY, Observable, throwError as _throw } from 'rxjs';
import { catchError, take } from 'rxjs/operators';

/**
 * Intercepts the HTTP responses, and in case that an error/exception is thrown, handles it
 * and extract the relevant information of it.
 */
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private readonly router: Router,
    private readonly modalService: AmcsModalService,
    private readonly previousRouteService: PreviousRouteService,
    private readonly store: Store<fromApp.AppState>,
    private readonly instrumentationService: InstrumentationService
  ) {}

  /**
   * Intercepts an outgoing HTTP request, executes it and handles any error that could be triggered in execution.
   * @see HttpInterceptor
   * @param req the outgoing HTTP request
   * @param next a HTTP request handler
   */
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const clonedRequest = req.clone();
    return next.handle(req).pipe(
      catchError((errorResponse) => {
        this.instrumentationService.trackException(errorResponse.error, LoggingVerbs.ErrorHttp, {
          url: clonedRequest.url,
          method: clonedRequest.method,
          urlWithParams: clonedRequest.urlWithParams,
          body: clonedRequest.body,
          message: errorResponse.message,
          name: errorResponse.name,
          status: errorResponse.status,
          statusText: errorResponse.statusText,
          errorMessage: errorResponse.error?.message,
          errorStackTrace: errorResponse.error?.stackTrace,
          errorType: errorResponse.error?.type,
          fullError: JSON.stringify(errorResponse.error),
        });

        // If auth status fails in any way we revert to login screen
        if (clonedRequest.method === 'GET' && clonedRequest.url.includes('authStatus') && errorResponse.status !== 200) {
          if (isDevMode()) {
            // This is really just to help developers
            this.modalService
              .createModal({
                template: ErrorModalComponent,
                title: 'Auth Status Call failed',
                extraData: ['The auth status call has failed. Redirecting to login'],
                largeSize: false,
                type: 'alert',
                isError: true,
              })
              .afterClosed()
              .pipe(take(1))
              .subscribe(() => {
                this.store.dispatch(new AuthActions.TrySignout());
              });
          } else {
            this.store.dispatch(new AuthActions.TrySignout());
          }
          // We don't want to throw an error (no modal) here just go to log-in screen
          return EMPTY;
        }
        // Any 401 results in redirect to log-in screen
        if (errorResponse instanceof HttpErrorResponse && errorResponse.status === 401) {
          this.store.dispatch(new AuthActions.TrySignout(window.parent.location.href));
          return EMPTY;
        }
        if (errorResponse instanceof HttpErrorResponse && errorResponse.status === 500) {
          // if create exago Api state call it's expecting an exception
          if (clonedRequest.method === 'POST' && clonedRequest.url.includes('bireports') && !clonedRequest.url.includes('executeReport')) {
            return EMPTY;
          }
        }
        if (errorResponse instanceof HttpErrorResponse && errorResponse.status === 404) {
          // if Exago call it's expecting an exception
          if (clonedRequest.method === 'GET' && clonedRequest.url.includes('exago')) {
            return EMPTY;
          }

          // if glossary 404's then throw exception (this is specifically handled in GlossaryService)
          if (clonedRequest.method === 'GET' && clonedRequest.url.includes(GlossaryLanguage.endpointName)) {
            return _throw(errorResponse);
          }

          // 404 from API means we tried to hit url e.g customer/9999 so we want to remove that from
          // our url list
          this.previousRouteService.removeCurrentUrl();
          this.router.navigate([CoreAppRoutes.notFound]);
          return EMPTY;
        }
        // This will be caught in erp service.
        return _throw(errorResponse);
      })
    );
  }
}
