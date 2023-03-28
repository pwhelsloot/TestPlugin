import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiResourceResponse } from '@core-module/config/api-resource-response.interface';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { ApiRequestTracking } from '@core-module/models/api/api-request-tracking.interface';
import { ApiResponseTracking } from '@core-module/models/api/api-response-tracking.interface';
import { CoreApiRequest } from '@core-module/models/api/core-api-request';
import { AiLoggingRequestType } from '@core-module/services/logging/ai-logging-request-type.constants';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { BatchRequestItem } from '@coremodels/api/batch-request-item.model';
import { CountCollectionModel } from '@coremodels/api/count-collection.model';
import { ErrorNotificationService } from '@coreservices/error-notification.service';
import { environment } from '@environments/environment';
import { EMPTY, Observable, of, Subject } from 'rxjs';
import { catchError, delay, finalize, map, switchMap, take, tap } from 'rxjs/operators';
import { ErpSaveService } from './erp-save.service';
import { PostErrorParserService } from './errors/post-error-parser.service';

/**
 * Obsolete, use EnhancedErpApiService instead
 *
 * @export
 * @class ErpApiService
 */
@Injectable({ providedIn: 'root' })
export class ErpApiService {

  requestTracking$: Observable<ApiRequestTracking>;
  responseTracking$: Observable<ApiResponseTracking>;

  constructor(
    private erpSaveService: ErpSaveService,
    private httpClient: HttpClient,
    private errorNotificationService: ErrorNotificationService
    ) {
      this.requestTracking$ = this.requestTrackingSubject.asObservable().pipe(delay(0));
      this.responseTracking$ = this.responseTrackingSubject.asObservable();
    }
    private requestTrackingSubject = new Subject<ApiRequestTracking>();
    private responseTrackingSubject = new Subject<ApiResponseTracking>();
    private requestId = 0;

  getRequestHandleError<T>(apiRequest: ApiRequest, mapFunction: (result: T) => any, displayError: boolean, handleErrorFunction?: ((error: any) => any)) {
    const requestId: number = this.getNextRequestId();
    this.trackRequest(requestId, AiLoggingRequestType.GET, apiRequest.getUrlResourcePath());
    return this.doGetRequest<T>(apiRequest)
      .pipe(
        map(mapFunction),
        tap(() => {
          this.responseTrackingSubject.next({ id: requestId });
        }),
        catchError((err: any) => {
          this.responseTrackingSubject.next({ id: requestId, isError: true });
          if (err && displayError) {
            this.errorNotificationService.notifyError(this.parseErrorMessage(err));
            if (handleErrorFunction) {
              return of(handleErrorFunction(err));
            }
            return EMPTY;
          }
          if (handleErrorFunction) {
            return of(handleErrorFunction(err));
          }
          return EMPTY;
        })
      );
  }

  getRequest<T, G>(apiRequest: ApiRequest, mapFunction: (result: T) => any, returnType: (new () => G) | G[] | CountCollectionModel<G>) {
    const requestId: number = this.getNextRequestId();
    this.trackRequest(requestId, AiLoggingRequestType.GET, apiRequest.getUrlResourcePath());
    return this.doGetRequest<T>(apiRequest)
      .pipe(
        map(mapFunction),
        tap(() => {
          this.responseTrackingSubject.next({ id: requestId });
        }),
        catchError((err: any) => {
          this.responseTrackingSubject.next({ id: requestId, isError: true });
          if (err) {
            this.errorNotificationService.notifyError(this.parseErrorMessage(err));
          }
          // Return an empty instance of the model (be it an array, singular, or countcollecitonmodel)
          return of(
            (returnType instanceof Array) ? returnType
              : (returnType instanceof CountCollectionModel) ? returnType
                : new returnType());
        })
      );
  }

  batchRequest<T>(apiRequests: ApiRequest[], mapFunction: (response: T) => any) {

    const requests = {};
    for (const apiRequest of apiRequests) {

      const batchRequestItem: BatchRequestItem = new BatchRequestItem();
      batchRequestItem.name = apiRequest.urlResourcePath[0].toString();
      batchRequestItem.path = apiRequest.getUrlResourcePath() + '?filters=' + apiRequest.getFilters();
      requests[batchRequestItem.name] = batchRequestItem.path;
    }
    const requestBody = JSON.stringify(requests);

    const baseUrl = this.buildBaseUrl(apiRequests[0]);
    const url = baseUrl + + 'batchRequests';

    return this.httpClient.post<T>(url, requestBody, { withCredentials: true })
      .pipe(
        map(mapFunction),
        catchError((err: any) => {
          this.errorNotificationService.notifyError(this.parseErrorMessage(err));
          return EMPTY;
        })
      );
  }

  postRequest<T>(apiRequest: ApiRequest, mapFunction: (response: T) => any) {
    const requestId: number = this.getNextRequestId();
    this.trackRequest(requestId, AiLoggingRequestType.POST, apiRequest.getUrlResourcePath());
    return this.erpSaveService.addRequest(apiRequest).pipe(
      take(1),
      switchMap((addSuccess: boolean) => {
        if (addSuccess) {
          let requestBody = '{' + apiRequest.postData + '}';

          if (apiRequest.postData === undefined) {
            requestBody = '{}';
          }
          const baseUrl = this.buildBaseUrl(apiRequest);
          const url = baseUrl + apiRequest.getUrlResourcePath();

          return this.httpClient.post<T>(url, requestBody, { withCredentials: true })
            .pipe(
              map(mapFunction),
              tap(() => {
              this.responseTrackingSubject.next({ id: requestId });
              }),
              catchError((err: any) => {
                this.responseTrackingSubject.next({ id: requestId, isError: true });
                this.errorNotificationService.notifyError(this.parseErrorMessage(err));
                return EMPTY;
              }),
              finalize(() => {
                this.erpSaveService.removeRequest(apiRequest);
              })
            );
        } else {
          return EMPTY;
        }
      }));
  }

  postHtmlRequest(apiRequest: ApiRequest, mapFunction: (response: string) => any) {
    const requestId: number = this.getNextRequestId();
    this.trackRequest(requestId, AiLoggingRequestType.POST, apiRequest.getUrlResourcePath());
    return this.erpSaveService.addRequest(apiRequest).pipe(
      take(1),
      switchMap((addSuccess: boolean) => {
        if (addSuccess) {
          let requestBody = '{' + apiRequest.postData + '}';

          if (apiRequest.postData === undefined) {
            requestBody = '{}';
          }
          const baseUrl = this.buildBaseUrl(apiRequest);
          const url = baseUrl + apiRequest.getUrlResourcePath();
          return this.httpClient.post(url, requestBody, { responseType: 'text', withCredentials: true })
            .pipe(
              map(mapFunction),
              tap(() => {
              this.responseTrackingSubject.next({ id: requestId });
              }),
              finalize(() => {
                this.erpSaveService.removeRequest(apiRequest);
              })
            );
        } else {
          return EMPTY;
        }
      }));
  }

  postBlobRequest(apiRequest: ApiRequest, mapFunction: (response: Blob) => any, errorMapFunction?: ((error: any) => any)) {
    const requestId: number = this.getNextRequestId();
    this.trackRequest(requestId, AiLoggingRequestType.POST, apiRequest.getUrlResourcePath());
    return this.erpSaveService.addRequest(apiRequest).pipe(
      take(1),
      switchMap((addSuccess: boolean) => {
        if (addSuccess) {
          let requestBody = '{' + apiRequest.postData + '}';

          if (apiRequest.postData === undefined) {
            requestBody = '{}';
          }
          const baseUrl = this.buildBaseUrl(apiRequest);
          const url = baseUrl + apiRequest.getUrlResourcePath();
          return this.httpClient.post(url, requestBody, { responseType: 'blob', withCredentials: true })
            .pipe(
              map(mapFunction),
              tap(() => {
              this.responseTrackingSubject.next({ id: requestId });
              }),
              catchError((err: any) => {
                this.responseTrackingSubject.next({ id: requestId, isError: true });
                this.errorNotificationService.notifyError(this.parseErrorMessage(err));
                if (isTruthy(errorMapFunction)) {
                  return of(errorMapFunction(err));
                }
                return EMPTY;
              }),
              finalize(() => {
                this.erpSaveService.removeRequest(apiRequest);
              })
            );
        } else {
          return EMPTY;
        }
      }));
  }

  postEntityRequest<T>(apiRequest: ApiRequest, postMapFunction: () => T) {
    const requestId: number = this.getNextRequestId();
    this.trackRequest(requestId, AiLoggingRequestType.POST, apiRequest.getUrlResourcePath());
    return this.erpSaveService.addRequest(apiRequest).pipe(
      take(1),
      switchMap((addSuccess: boolean) => {
        if (addSuccess) {
          const baseUrl = this.buildBaseUrl(apiRequest);
          const url = baseUrl + apiRequest.getUrlResourcePath();
          return this.httpClient.post(url, postMapFunction(), { withCredentials: true })
            .pipe(
              map((response: ApiResourceResponse) => {
                if (response.errors != null) {
                  response.errors = PostErrorParserService.parseError(response.errors);
                }
                return response;
              }),
              tap(() => {
                this.responseTrackingSubject.next({ id: requestId });
              }),
              catchError((err: any) => {
                this.responseTrackingSubject.next({ id: requestId, isError: true });
                this.errorNotificationService.notifyError(this.parseErrorMessage(err));
                return EMPTY;
              }),
              finalize(() => {
                this.erpSaveService.removeRequest(apiRequest);
              })
            );
        } else {
          return EMPTY;
        }
      }));
  }

  postSimpleRequest(apiRequest: ApiRequest, entity: any, continueWithError = false) {
    const requestId: number = this.getNextRequestId();
    this.trackRequest(requestId, AiLoggingRequestType.POST, apiRequest.getUrlResourcePath());
    return this.erpSaveService.addRequest(apiRequest).pipe(
      take(1),
      switchMap((addSuccess: boolean) => {
        if (addSuccess) {
          const baseUrl = this.buildBaseUrl(apiRequest);
          const url = baseUrl + apiRequest.getUrlResourcePath();
          return this.httpClient.post(url, entity, { withCredentials: true })
            .pipe(
              map((response: ApiResourceResponse) => {
                if (isTruthy(response) && isTruthy(response.errors)) {
                  response.errors = PostErrorParserService.parseError(response.errors);
                  this.errorNotificationService.notifyError(response.errors);
                  return null;
                }
                return response;
              }),
              tap(() => {
                this.responseTrackingSubject.next({ id: requestId });
              }),
              catchError((err: any) => {
                this.responseTrackingSubject.next({ id: requestId, isError: true });
                const substring = 'BslUserException:Error: ';
                const bslSubstring = 'BslUserException:';
                if (err.error.message.includes(substring)) {
                  err.error.message = err.error.message.replace(substring, '');
                  err.message = 'Error';
                } else if (err.error.message.includes(bslSubstring)) {
                  err.message = err.error.message.replace(bslSubstring, '');
                  err.error.message = null;
                }
                this.errorNotificationService.notifyError(this.parseErrorMessage(err));
                return continueWithError ? of(null) : EMPTY;
              }),
              finalize(() => {
                this.erpSaveService.removeRequest(apiRequest);
              })
            );
        } else {
          return EMPTY;
        }
      }));
  }

  putRequest(apiRequest: ApiRequest, mapFunction: (response: Response) => any) {
    const requestBody = JSON.stringify(apiRequest.postData);

    const baseUrl = this.buildBaseUrl(apiRequest);
    const url = baseUrl + apiRequest.getUrlResourcePath();
    return this.httpClient.put(url, requestBody, { withCredentials: true })
      .pipe(
        map(mapFunction),
        catchError((err: any) => {
          this.errorNotificationService.notifyError(this.parseErrorMessage(err));
          return EMPTY;
        })
      );
  }

  deleteRequest(apiRequest: ApiRequest, mapFunction: (response: Response) => any) {
    const baseUrl = this.buildBaseUrl(apiRequest);
    const url = baseUrl + apiRequest.getUrlResourcePath();
    const requestId: number = this.getNextRequestId();
    this.trackRequest(requestId, AiLoggingRequestType.DELETE, apiRequest.getUrlResourcePath());
    return this.httpClient.delete(url, { withCredentials: true })
      .pipe(
        map(mapFunction),
        tap(() => {
          this.responseTrackingSubject.next({ id: requestId });
        }),
        catchError((err: any) => {
          this.responseTrackingSubject.next({ id: requestId, isError: true });
          this.errorNotificationService.notifyError(this.parseErrorMessage(err));
          return EMPTY;
        })
      );
  }

  private doGetRequest<T>(apiRequest: ApiRequest) {
    const baseUrl = this.buildBaseUrl(apiRequest);
    const url = baseUrl + apiRequest.getUrlResourcePath();
    let httpParams = new HttpParams();
    const filters = apiRequest.getFilters();
    const includeLinks = apiRequest.getLinkOptionsAsString();

    if (includeLinks != null) {
      httpParams = httpParams.append('links', includeLinks);
    }
    if (filters && filters.length > 0) {
      httpParams = httpParams.append('filter', filters);
    }
    if (apiRequest.max) {
      httpParams = httpParams.append('max', apiRequest.max.toString());
    }
    if (apiRequest.key) {
      httpParams = httpParams.append('key', apiRequest.key);
    }
    if (apiRequest.page != null) {
      httpParams = httpParams.append('page', apiRequest.page.toString());
    }
    if (apiRequest.includeCount) {
      httpParams = httpParams.append('includeCount', 'true');
    }
    if (apiRequest.returnUrl) {
      httpParams = httpParams.append('return', apiRequest.returnUrl.toString());
    }
    return this.httpClient.get<T>(
      url,
      { params: httpParams, withCredentials: true }
    );
  }

  private parseErrorMessage(errorResponse: any): string {
    let errMsg: string = errorResponse.message ? errorResponse.message : errorResponse.toString();
    if (errorResponse.error != null && errorResponse.error.message != null) {
      errMsg += ' - ' + errorResponse.error.message;
    }
    return errMsg;
  }

  private buildBaseUrl(apiRequest: ApiRequest): string {
    let url = '';
    if (apiRequest instanceof CoreApiRequest) {
      url = `${window.coreServiceRoot}/services/api/`;
    } else {
      url = `${window.coreServiceRoot}/${environment.applicationApiPrefix}/`;
    }
    return url;
  }

  private getNextRequestId(): number {
    if (this.requestId >= 20000) {
      this.requestId = 0;
    }
    return this.requestId++;
  }

  /**
  * A shame but this timeout 0 is needed as we can only start tracking requests for a view once the component ngOnInit
  * is called. If the api request is started before this (for example in the constructor or service's constructor) then we will
  * lose that api request from the logging.
  * @param requestId
  * @param type
  * @param url
  */
  private trackRequest(requestId: number, type: AiLoggingRequestType, url: string) {
    setTimeout(() => {
      this.requestTrackingSubject.next({ id: requestId, type, url });
    }, 0);
  }
}
