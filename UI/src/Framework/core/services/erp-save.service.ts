import { Injectable } from '@angular/core';
import { IApiSaveRequest } from '@core-module/models/api/api-save-request.interface';
import { CoreApiRequest } from '@core-module/models/api/core-api-request';
import { BaseService } from '@coreservices/base.service';
import { BehaviorSubject, Observable, of, Subject } from 'rxjs';
import { publishReplay, refCount, scan, startWith, switchMap, take, takeUntil } from 'rxjs/operators';
import { ApiRequest } from '../models/api/api-request.model';

@Injectable({ providedIn: 'root' })
export class ErpSaveService extends BaseService {

    saveAvailable$: Observable<boolean>;

  constructor() {
    super();

    // All requests, initalised as blank but accumulates items via operations (functions) passed into requestUpdatesSubject.
    this.requests$ = this.requestUpdatesSubject.pipe(
      takeUntil(this.unsubscribe),
      // Scan lets you build an array (our requests) and perform actions on it (our add/remove operations)
      scan((requests: IApiSaveRequest[], operation: (requests: IApiSaveRequest[]) => IApiSaveRequest[]) => {
        return operation(requests);
      }, new Array<IApiSaveRequest>()),
      startWith(new Array<IApiSaveRequest>()),
      // Publish stream, this just makes it like a replay subject
      publishReplay(1),
      refCount()
    );

    // Available if no requests pending
    this.saveAvailable$ = this.saveAvailable.asObservable();
    this.requests$.pipe(takeUntil(this.unsubscribe)).subscribe((requests: IApiSaveRequest[]) => {
      this.saveAvailable.next(requests.length <= 0);
    });

    // Add request to list if not already in it
    this.newRequestSubject.pipe(takeUntil(this.unsubscribe)).subscribe((newRequest: IApiSaveRequest) => {
      this.requestUpdatesSubject.next(
        // This is the operation inside the scan function above.
        (requests: IApiSaveRequest[]) => {
          if (requests.some((req) => req.urlResourcePath === newRequest.urlResourcePath)) {
            return requests;
          } else {
            return requests.concat(newRequest);
          }
        }
      );
    });

    // Remove all matching requests from list
    this.removeRequestSubject.pipe(takeUntil(this.unsubscribe)).subscribe((removeRequest: ApiRequest) => {
      this.requestUpdatesSubject.next(
        // This is the operation inside the scan function above.
        (requests: IApiSaveRequest[]) => {
          return requests.filter((req) => req.urlResourcePath !== removeRequest.getUrlResourcePath());
        }
      );
    });
  }

  private readonly requests$: Observable<IApiSaveRequest[]>;
  private readonly newRequestSubject = new Subject<IApiSaveRequest>();
  private readonly removeRequestSubject = new Subject<ApiRequest>();
  private readonly requestUpdatesSubject = new Subject<(requests: IApiSaveRequest[]) => IApiSaveRequest[]>();
  private readonly saveAvailable = new BehaviorSubject<boolean>(true);

  addRequest(newRequest: ApiRequest): Observable<boolean> {
    // Will return true if item doesnt already exist in list else false (CoreApiRequests are always added)
    const newSaveRequest: IApiSaveRequest = { req: newRequest, urlResourcePath: newRequest.getUrlResourcePath() };
    return this.requests$.pipe(
      take(1),
      switchMap((requests: IApiSaveRequest[]) => {
        const isInList = requests.some(
          (request) => !(newSaveRequest.req instanceof CoreApiRequest) && request.urlResourcePath === newSaveRequest.urlResourcePath
        );
        this.newRequestSubject.next(newSaveRequest);
        return of(!isInList);
      })
    );
  }

  removeRequest(removeRequest: ApiRequest) {
    this.removeRequestSubject.next(removeRequest);
  }
}
