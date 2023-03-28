import { BaseService } from '@core-module/services/base.service';
import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { takeUntil, switchMap } from 'rxjs/operators';
import { MapExampleEditorData } from '@app/template/models/map-example/map-example-editor-data.model';
import { MapExample } from '@app/template/models/map-example/map-example.model';
import { ApiBusinessService } from '@core-module/services/service-structure/api-business.service';
import { ApiFilter } from '@core-module/services/service-structure/api-filter';
import { ApiFilters } from '@core-module/services/service-structure/api-filter-builder';

@Injectable()
export class MapExampleService extends BaseService {
  mapPointsEditorData$: Observable<MapExampleEditorData> = new Observable<MapExampleEditorData>();
  constructor(private readonly businessService: ApiBusinessService) {
    super();
    this.setupServicePointStream();
  }

  private mapPointsEditorData: Subject<MapExampleEditorData> = new Subject<MapExampleEditorData>();
  private mapPointEditorDataRequest: ReplaySubject<number> = new ReplaySubject<number>(1);

  requestEditServicePointEditorData(mapPointId: number) {
    this.mapPointEditorDataRequest.next(mapPointId);
  }

  requestNewServicePointEditorData(editorData: MapExampleEditorData) {
    this.mapPointsEditorData.next(editorData);
  }

  saveMapPoint(serviceArea: MapExample, successMessage: string) {
    return this.businessService.save<MapExample>(serviceArea, successMessage, MapExample);
  }

  private setupServicePointStream() {
    this.mapPointsEditorData$ = this.mapPointsEditorData.asObservable();
    this.mapPointEditorDataRequest
      .pipe(
        takeUntil(this.unsubscribe),
        switchMap((id) =>
          this.businessService.get<MapExampleEditorData>(
            new ApiFilters().add(ApiFilter.equal('mapExampleId', id)).build(),
            MapExampleEditorData
          )
        )
      )
      .subscribe((data: MapExampleEditorData) => {
        this.mapPointsEditorData.next(data);
      });
  }
}
