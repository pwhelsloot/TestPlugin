import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiResourceResponse } from '@core-module/config/api-resource-response.interface';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { IApiBaseModel } from '@core-module/models/api/api-base.interface';
import { ApiRequestTracking } from '@core-module/models/api/api-request-tracking.interface';
import { ApiResponseTracking } from '@core-module/models/api/api-response-tracking.interface';
import { CoreApiRequest } from '@core-module/models/api/core-api-request';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import { AiLoggingRequestType } from '@core-module/services/logging/ai-logging-request-type.constants';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { ErrorNotificationService } from '@coreservices/error-notification.service';
import { environment } from '@environments/environment';
import { EMPTY, Observable, of, Subject } from 'rxjs';
import { catchError, filter, finalize, map, switchMap, take, tap } from 'rxjs/operators';
import { AmcsNotificationService } from './amcs-notification.service';
import { ErpSaveService } from './erp-save.service';
import { PostErrorParserService } from './errors/post-error-parser.service';

@Injectable({ providedIn: 'root' })
export class EnhancedErpApiService {
  requestTracking$: Observable<ApiRequestTracking>;
  responseTracking$: Observable<ApiResponseTracking>;

  constructor(
    private erpSaveService: ErpSaveService,
    private httpClient: HttpClient,
    private errorNotificationService: ErrorNotificationService,
    private successNotificationService: AmcsNotificationService
  ) {
    this.requestTracking$ = this.requestTrackingSubject.asObservable();
    this.responseTracking$ = this.responseTrackingSubject.asObservable();
  }

  private requestTrackingSubject = new Subject<ApiRequestTracking>();
  private responseTrackingSubject = new Subject<ApiResponseTracking>();
  private requestId = 0;

  get<T extends IApiBaseModel>(apiRequest: ApiRequest, type: new () => T): Observable<T> {
    const requestId: number = this.getNextRequestId();
    this.trackRequest(requestId, AiLoggingRequestType.GET, apiRequest.getUrlResourcePath());
    return this.buildGetRequest(apiRequest).pipe(
      map((response: ApiResourceResponse) => {
        if (!response) {
          return new type();
        }
        if (response.errors != null) {
          response.errors = PostErrorParserService.parseError(response.errors);
          this.errorNotificationService.notifyError(response.errors);
          return new type();
        }

        if (isTruthy(response) && isTruthy(response.resource)) {
          if (Array.isArray(response.resource) && response.resource.length === 1) {
            return new type().parse(response.resource[0], type);
          } else {
            return new type().parse(response.resource, type);
          }
        } else {
          return new type().parse(response, type);
        }
      }),
      tap(() => {
        this.responseTrackingSubject.next({ id: requestId });
      }),
      catchError((err: any) => {
        this.responseTrackingSubject.next({ id: requestId, isError: true });
        if (err && !apiRequest.suppressErrorModal) {
          this.errorNotificationService.notifyError(this.parseErrorMessage(err));
        }
        // Return an empty instance of the model
        return of(new type());
      })
    );
  }

  getRawResponse(apiRequest: ApiRequest): Observable<any> {
    const requestId: number = this.getNextRequestId();
    this.trackRequest(requestId, AiLoggingRequestType.GET, apiRequest.getUrlResourcePath());
    return this.buildGetRequest(apiRequest).pipe(
      map((response: any) => {
        return response;
      }),
      tap(() => {
        this.responseTrackingSubject.next({ id: requestId });
      }),
      catchError((err: any) => {
        this.responseTrackingSubject.next({ id: requestId, isError: true });
        if (err && !apiRequest.suppressErrorModal) {
          this.errorNotificationService.notifyError(this.parseErrorMessage(err));
        }

        return of(null);
      })
    );
  }

  getArray<T extends IApiBaseModel>(apiRequest: ApiRequest, type: new () => T): Observable<T[]> {
    const requestId: number = this.getNextRequestId();
    this.trackRequest(requestId, AiLoggingRequestType.GET, apiRequest.getUrlResourcePath());
    return this.buildGetRequest(apiRequest).pipe(
      map((response: ApiResourceResponse) => {
        if (!response) {
          return new Array<T>();
        }
        if (response.errors != null) {
          response.errors = PostErrorParserService.parseError(response.errors);
          this.errorNotificationService.notifyError(response.errors);
          return new Array<T>();
        }
        return new type().parseArray(response.resource, type);
      }),
      tap(() => {
        this.responseTrackingSubject.next({ id: requestId });
      }),
      catchError((err: any) => {
        this.responseTrackingSubject.next({ id: requestId, isError: true });
        if (err && !apiRequest.suppressErrorModal) {
          this.errorNotificationService.notifyError(this.parseErrorMessage(err));
        }
        // Return an empty instance of the array
        return of(new Array<T>());
      })
    );
  }

  getArrayWithCount<T extends IApiBaseModel>(apiRequest: ApiRequest, type: new () => T): Observable<CountCollectionModel<T>> {
    const requestId: number = this.getNextRequestId();
    this.trackRequest(requestId, AiLoggingRequestType.GET, apiRequest.getUrlResourcePath());
    return this.buildGetRequest(apiRequest).pipe(
      map((response: ApiResourceResponse) => {
        if (!response) {
          return new CountCollectionModel<T>([], 0);
        }
        if (response.errors != null) {
          response.errors = PostErrorParserService.parseError(response.errors);
          this.errorNotificationService.notifyError(response.errors);
          return new CountCollectionModel<T>([], 0);
        }
        return new CountCollectionModel<T>(
          new type().parseArray(response.resource, type),
          response.extra.count != null ? response.extra.count : 0
        );
      }),
      tap(() => {
        this.responseTrackingSubject.next({ id: requestId });
      }),
      catchError((err: any) => {
        this.responseTrackingSubject.next({ id: requestId, isError: true });
        if (err && !apiRequest.suppressErrorModal) {
          this.errorNotificationService.notifyError(this.parseErrorMessage(err));
        }
        // Return an empty instance of the count collection
        return of(new CountCollectionModel<T>([], 0));
      })
    );
  }

  /// This method is designed for use with the server side IMessageService in cases where we are posting data to the server
  /// but are not saving an object. Unlike the save and saveArray methods, this does not route the request through the save
  /// service to protect against duplicate calls. In all cases where an object is being saved, save or saveArray is the correct
  /// call to use.
  postMessage<T extends IApiBaseModel, G extends IApiBaseModel>(
    apiRequest: ApiRequest,
    object: T,
    type: new () => T,
    returnType: new () => G,
    successMessage?: string
  ): Observable<G> {
    const baseUrl = this.buildBaseUrl(apiRequest);
    const url = baseUrl + apiRequest.getUrlResourcePath();
    const requestId: number = this.getNextRequestId();
    this.trackRequest(requestId, AiLoggingRequestType.POST, apiRequest.getUrlResourcePath());
    return this.httpClient.post(url, new type().post(object, type), { withCredentials: true }).pipe(
      map((response: ApiResourceResponse) => {
        if (!response) {
          return null;
        }
        if (response != null && response.errors != null) {
          response.errors = PostErrorParserService.parseError(response.errors);
          this.errorNotificationService.notifyError(response.errors);
          return null;
        } else {
          if (isTruthy(successMessage)) {
            this.successNotificationService.showNotification(successMessage);
          }
          return new returnType().parse(response, returnType);
        }
      }),
      tap(() => {
        this.responseTrackingSubject.next({ id: requestId });
      }),
      catchError((err: any) => {
        this.responseTrackingSubject.next({ id: requestId, isError: true });
        const substring = 'BslUserException:';
        if (err?.error?.message?.includes(substring)) {
          err.error.message = err.error.message.replace(substring, '');
          err.message = 'Error';
          this.errorNotificationService.notifyError(this.parseBslUserExceptionErrorMessage(err));
        } else if (err?.error) {
          this.errorNotificationService.notifyError(this.parseErrorMessage(err.error));
        } else {
          this.errorNotificationService.notifyError(this.parseErrorMessage(err));
        }
        return of(null);
      })
    );
  }

  // Much like postMessage however we don't have a message payment to give, instead we recieved a reponse payload of type 'T'
  postUrl<T extends IApiBaseModel>(apiRequest: ApiRequest, type: new () => T): Observable<T> {
    const baseUrl = this.buildBaseUrl(apiRequest);
    const url = baseUrl + apiRequest.getUrlResourcePath();
    const requestId: number = this.getNextRequestId();
    this.trackRequest(requestId, AiLoggingRequestType.GET, url);
    return this.httpClient.post(url, '{}', { withCredentials: true }).pipe(
      map((response: ApiResourceResponse) => {
        if (!response) {
          return null;
        } else if (response.errors != null) {
          response.errors = PostErrorParserService.parseError(response.errors);
          this.errorNotificationService.notifyError(response.errors);
          return null;
        } else {
          return new type().parse(response, type);
        }
      }),
      tap(() => {
        this.responseTrackingSubject.next({ id: requestId });
      }),
      catchError((err: any) => {
        this.responseTrackingSubject.next({ id: requestId, isError: true });
        this.errorNotificationService.notifyError(this.parseErrorMessage(err));
        // Return an empty instance of the model
        return of(new type());
      })
    );
  }

  delete(apiRequest: ApiRequest, successMessage?: string): Observable<boolean> {
    const baseUrl = this.buildBaseUrl(apiRequest);
    const url = baseUrl + apiRequest.getUrlResourcePath();
    const requestId: number = this.getNextRequestId();
    this.trackRequest(requestId, AiLoggingRequestType.DELETE, url);
    return this.httpClient.delete(url, { withCredentials: true }).pipe(
      map((response: ApiResourceResponse) => {
        if (!response) {
          return null;
        }
        if (response.errors != null) {
          response.errors = PostErrorParserService.parseError(response.errors);
          this.errorNotificationService.notifyError(response.errors);
          return false;
        } else {
          if (isTruthy(successMessage)) {
            this.successNotificationService.showNotification(successMessage);
          }
          return true;
        }
      }),
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

  save<T extends IApiBaseModel>(apiRequest: ApiRequest, object: T, type: new () => T, successMessage?: string): Observable<number> {
    const requestId: number = this.getNextRequestId();
    return this.erpSaveService.addRequest(apiRequest).pipe(
      filter((x) => x),
      take(1),
      switchMap(() => {
        const baseUrl = this.buildBaseUrl(apiRequest);
        const url = baseUrl + apiRequest.getUrlResourcePath();
        this.trackRequest(requestId, AiLoggingRequestType.POST, url);
        return this.httpClient.post(url, new type().post(object, type), { withCredentials: true });
      }),
      map((response: ApiResourceResponse) => {
        if (!response) {
          return null;
        }
        if (response.errors != null) {
          response.errors = PostErrorParserService.parseError(response.errors);
          this.errorNotificationService.notifyError(response.errors);
          return null;
        } else {
          if (isTruthy(successMessage)) {
            this.successNotificationService.showNotification(successMessage);
          }
          if (!isTruthy(response.resource)) {
            return +response;
          } else {
            return +response.resource;
          }
        }
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
  }

  saveArray<T extends IApiBaseModel>(
    apiRequest: ApiRequest,
    objectArray: T[],
    type: new () => T,
    successMessage?: string
  ): Observable<ApiResourceResponse> {
    const requestId: number = this.getNextRequestId();
    return this.erpSaveService.addRequest(apiRequest).pipe(
      filter((x) => x),
      take(1),
      switchMap(() => {
        const baseUrl = this.buildBaseUrl(apiRequest);
        const url = baseUrl + apiRequest.getUrlResourcePath();
        this.trackRequest(requestId, AiLoggingRequestType.POST, url);
        return this.httpClient.post(url, new type().postArray(objectArray, type), { withCredentials: true });
      }),
      map((response: ApiResourceResponse) => {
        if (!response) {
          return null;
        }
        if (response.errors != null) {
          response.errors = PostErrorParserService.parseError(response.errors);
          this.errorNotificationService.notifyError(response.errors);
          return null;
        } else {
          if (isTruthy(successMessage)) {
            this.successNotificationService.showNotification(successMessage);
          }
          return response;
        }
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
  }

  private buildGetRequest(apiRequest: ApiRequest) {
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
    if (apiRequest.includeDeleted) {
      httpParams = httpParams.append('includeDeleted', 'true');
    }
    if (apiRequest.returnUrl) {
      httpParams = httpParams.append('return', apiRequest.returnUrl.toString());
    }
    return this.httpClient.get<ApiResourceResponse>(url, { params: httpParams, withCredentials: true });
  }

  private parseErrorMessage(errorResponse: any): string {
    let errMsg: string = errorResponse.message ? errorResponse.message : errorResponse.toString();
    if (errorResponse.error != null && errorResponse.error.message != null) {
      errMsg += ' - ' + errorResponse.error.message;
    }
    return errMsg;
  }

  private parseBslUserExceptionErrorMessage(errorResponse: any): string {
    let errMsg: string = errorResponse.message ? errorResponse.message : errorResponse.toString();
    if (errorResponse.error != null && errorResponse.error.message != null) {
      errMsg = errorResponse.error.message;
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
