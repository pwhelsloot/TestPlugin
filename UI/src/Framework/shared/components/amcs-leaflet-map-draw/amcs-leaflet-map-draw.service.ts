import { Injectable } from '@angular/core';
import { AmcsLeafletMapBaseShape } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-base-shape.model';
import { AmcsLeafletMapGrouping } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-grouping.model';
import { AmcsLeafletMapMarkerIdentification } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-marker-identification.model';
import { AmcsLeafletMapMarker } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-marker.model';
import { AmcsLeafletMapPolygon } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-polygon.model';
import { AmcsLeafletMapShapeEnum } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-shape.enum';
import { OperationalAreaRequest } from '@shared-module/amcs-leaflet-map/models/operational-area-request.model';
import { Control, Layer } from 'leaflet';
import { ReplaySubject, Subject } from 'rxjs';

@Injectable()
export class AmcsLeafletMapDrawService {
  /**
   * Triggered when user selected a item from the list
   *
   * @memberof AmcsLeafletMapDrawService
   */
  editShapeRequest = new Subject<number>();

  /**
   * Triggered when user clicks on save while editing
   *
   * @memberof AmcsLeafletMapDrawService
   */
  editShapeSaveRequest = new Subject<[AmcsLeafletMapShapeEnum, { id: number; request?: OperationalAreaRequest }]>();

  /**
   * Triggered when the edited polygon has been saved
   *
   * @memberof AmcsLeafletMapDrawService
   */
  editShapeSaved = new Subject<AmcsLeafletMapBaseShape>();
  editShapeCancelled = new Subject<AmcsLeafletMapBaseShape>();

  /**
   * Triggered when the edited shape is validated for overlaps
   *
   * @memberof AmcsLeafletMapDrawService
   */
  editShapeDrawn = new Subject<AmcsLeafletMapBaseShape>();
  editCancel = new Subject();

  /**
   * Triggered when a newly created polygon is validated for overlaps
   *
   * @memberof AmcsLeafletMapDrawService
   */
  newShapeRequest = new Subject<Layer>();
  removeShapeRequest = new Subject<number>();
  newPolygonSaveRequest = new Subject<{ request: OperationalAreaRequest }>();
  newPolygonDrawn = new Subject<AmcsLeafletMapPolygon>();

  /**
   * When a shape is clicked on the map
   *
   * @memberof AmcsLeafletMapDrawService
   */
  operationalAreaOnClickShape = new Subject<{ operationalAreaId: number; mapAreaId: number; isServiceExclusion: boolean }>();
  onClickShape = new Subject<number>();

  /**
   * Triggered when the new polygon has been saved
   *
   * @memberof AmcsLeafletMapDrawService
   */
  newPolygonSaved = new Subject<AmcsLeafletMapPolygon>();

  /**
   * When a marker is clicked on the map
   *
   * @memberof AmcsLeafletMapDrawService
   */
  onClickMarker = new Subject<AmcsLeafletMapMarkerIdentification>();

  controls = new ReplaySubject<{ control: Control; controlId: number }>(4);

  /**
   * Used for toggling autoZoom control
   *
   * @memberof AmcsLeafletMapDrawService
   */
  autoZoomSubject = new Subject<boolean>();

  selectedId = 0;

  selectedExclusionId = 0;

  panToMarkerSubject = new Subject<AmcsLeafletMapMarkerIdentification>();

  /**
   * Used for enabling/disabling the automatic fit to bounds feature when clicking on markers
   *
   * @memberof AmcsLeafletMapDrawService
   */
  autoFitToBoundsSubject = new Subject<boolean>();

  /**
   * Update marker icons
   *
   * @memberof AmcsLeafletMapDrawService
   */
  onUpdateMarkerIconSubject = new Subject<AmcsLeafletMapMarker>();

  addGroupSubject = new Subject<AmcsLeafletMapGrouping[]>();
  updateGroupSubject = new Subject<AmcsLeafletMapGrouping>();
  deleteGroupSubject = new Subject<string[]>();
  removeShapeLayersSubject = new Subject();
  internalMapGroupingsSubject = new ReplaySubject<AmcsLeafletMapGrouping[]>(1);
  removeMapMarkerSubject = new Subject<AmcsLeafletMapMarkerIdentification>();
  addMapMarkerSubject = new Subject<AmcsLeafletMapMarkerIdentification>();

  addGroup(groups: AmcsLeafletMapGrouping[]) {
    this.addGroupSubject.next(groups);
  }

  updateGroup(group: AmcsLeafletMapGrouping) {
    this.updateGroupSubject.next(group);
  }

  deleteGroup(groupNames: string[]) {
    this.deleteGroupSubject.next(groupNames);
  }

  removeAllShapeLayers() {
    this.removeShapeLayersSubject.next();
  }
}
