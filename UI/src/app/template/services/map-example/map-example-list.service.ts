import { Injectable } from '@angular/core';
import { BaseService } from '@core-module/services/base.service';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { AmcsNotificationService } from '@core-module/services/amcs-notification.service';
import { takeUntil, switchMap, take } from 'rxjs/operators';
import { MapExampleContext } from '@app/template/models/map-example/map-example-context.model';
import { MapExample } from '@app/template/models/map-example/map-example.model';
import { MapExampleEditorData } from '@app/template/models/map-example/map-example-editor-data.model';
import { ApiBusinessService } from '@core-module/services/service-structure/api-business.service';

@Injectable()
export class MapExampleListService extends BaseService {
  mapExampleContext$: Observable<MapExampleContext>;
  mapExampleList$: Observable<MapExample[]>;
  newMapExample$: Subject<MapExampleEditorData> = new Subject<MapExampleEditorData>();

  constructor(readonly businessService: ApiBusinessService, readonly notificationService: AmcsNotificationService) {
    super();
    this.setUpMapExampleStream();
  }

  private servicePointsList: ReplaySubject<MapExample[]> = new ReplaySubject<MapExample[]>(1);
  private servicePointsRequest: Subject<number> = new Subject<number>();

  requestMapExamples() {
    this.mapExampleContext$.pipe(take(1)).subscribe((context) => {
      this.servicePointsRequest.next(context.mapPointId);
    });
  }

  requestNewMapExampleEditorData() {
    this.mapExampleContext$.pipe(take(1)).subscribe((context) => {
      this.newMapExample$.next({
        mapExample: {
          latitude: context.latitude,
          longitude: context.longitude,
        } as MapExample,
      } as MapExampleEditorData);
    });
  }

  deleteServicePoint(mapPointId: number, successMessage: string) {
    return this.businessService.delete<MapExample>(mapPointId, successMessage, MapExample);
  }

  private setUpMapExampleStream() {
    this.mapExampleList$ = this.servicePointsList.asObservable();
    this.servicePointsRequest
      .pipe(
        takeUntil(this.unsubscribe),
        switchMap(() => this.businessService.getArray<MapExample>([], MapExample))
      )
      .subscribe((data: MapExample[]) => {
        this.servicePointsList.next(data);
      });
  }
}
