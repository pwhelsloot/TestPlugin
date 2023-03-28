import { Component, ElementRef, Input, OnChanges, OnDestroy, OnInit, Renderer2, SimpleChanges } from '@angular/core';
import { AmcsLeafletMapService } from '@core-module/services/amcs-leaflet-map.service';
import { AmcsNotificationService } from '@coreservices/amcs-notification.service';
import { AmcsLeafletMapGrouping } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-grouping.model';
import { AmcsLeafletMapShapeEnum } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-shape.enum';
import { OperationalAreaRequest } from '@shared-module/amcs-leaflet-map/models/operational-area-request.model';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { FeatureGroup, Layer } from 'leaflet';
import { Subject, Subscription } from 'rxjs';
import { take } from 'rxjs/operators';
import { isTruthy } from '../../../core/helpers/is-truthy.function';
import { BusinessTypeLookup } from '../../../core/models/lookups/business-type-lookup.model';
import { AmcsMapDrawControl } from '../../amcs-leaflet-map/controls/amcs-map-draw.control';
import { AmcsLeafletDrawCircleHelper } from './amcs-leaflet-draw-circle.helper';
import { AmcsLeafletDrawPolygonHelper } from './amcs-leaflet-draw-polygon.helper';
import { AmcsLeafletMapDrawService } from './amcs-leaflet-map-draw.service';

@Component({
  selector: 'app-amcs-leaflet-map-draw',
  templateUrl: './amcs-leaflet-map-draw.component.html',
  styleUrls: ['./amcs-leaflet-map-draw.component.scss'],
  providers: [AmcsLeafletMapService],
})
export class AmcsLeafletMapDrawComponent extends AutomationLocatorDirective implements OnInit, OnDestroy, OnChanges {
  @Input() layers: AmcsLeafletMapGrouping[] = [];
  @Input() mapHeight = 810;
  @Input() drawIntersectErrorText: string;
  @Input() latitude: number;
  @Input() longitude: number;
  @Input() zoom = 5;
  @Input() businessTypeId: number;
  @Input() businessTypes: BusinessTypeLookup[];
  @Input() enableDrawSelection = true;
  @Input() enableMarkerCluster = false;
  @Input() shapesSupported: AmcsLeafletMapShapeEnum[] = [];
  @Input() customClass: string = null;
  @Input() mapRedrawSubject: Subject<boolean>;

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    private amcsLeafletMapDrawService: AmcsLeafletMapDrawService,
    private mapService: AmcsLeafletMapService,
    private translationsService: SharedTranslationsService,
    private notificationService: AmcsNotificationService
  ) {
    super(elRef, renderer);
  }

  private subscriptions = new Array<Subscription>();
  private polygonHelper: AmcsLeafletDrawPolygonHelper;
  private circleHelper: AmcsLeafletDrawCircleHelper;

  private drawControl: AmcsMapDrawControl;

  ngOnChanges(changes: SimpleChanges) {
    if (changes['layers']) {
      this.refreshDrawHelperLayers();
      if (isTruthy(this.businessTypeId) && this.businessTypeId > 0) {
        this.mapService.setState(this.layers.filter((x) => x.polygons[0].op.businessTypeIds.some((t) => t === this.businessTypeId)));
      } else {
        this.mapService.setState(this.layers);
        this.mapService.autoFitToBounds = true;
      }
    }
  }

  ngOnInit() {
    if (!isTruthy(this.polygonHelper) || !isTruthy(this.circleHelper)) {
      this.initHelpers();
    }
    this.subscriptions.push(
      this.amcsLeafletMapDrawService.newShapeRequest.subscribe((shape: Layer) => {
        this.drawControl.addShape(shape);
      }),
      this.amcsLeafletMapDrawService.removeShapeRequest.subscribe((shapeId: number) => {
        this.drawControl.removeShape(shapeId);
      }),
      this.mapService.shapeDrawnSubject.subscribe((layer: Layer) => {
        this.amcsLeafletMapDrawService.newShapeRequest.next(layer);
      }),
      this.translationsService.translations.pipe(take(1)).subscribe((translations: string[]) => {
        if (this.enableDrawSelection) {
          this.drawControl = new AmcsMapDrawControl('topright', this.shapesSupported, translations, new FeatureGroup());
          this.mapService.addMapControl(this.drawControl, 3);
        }
        if (isTruthy(this.businessTypeId) && this.businessTypeId > 0) {
          this.mapService.setState(this.layers.filter((x) => x.polygons[0].op.businessTypeIds.some((t) => t === this.businessTypeId)));
        } else {
          this.mapService.setState(this.layers);
        }
      }),
      this.amcsLeafletMapDrawService.controls.subscribe(({ control, controlId }) => {
        this.mapService.addMapControl(control, controlId);
      }),
      this.amcsLeafletMapDrawService.editShapeRequest.subscribe((shapeId: number) => {
        if (this.drawControl.currentlyEditableShape === shapeId) {
          this.amcsLeafletMapDrawService.editShapeCancelled.next();
        } else {
          this.drawControl.toggleEditShape(shapeId);
        }
      }),
      this.amcsLeafletMapDrawService.editShapeSaveRequest
        .subscribe((request: [AmcsLeafletMapShapeEnum, { id: number; request?: OperationalAreaRequest }]) => {
          const shapeType: AmcsLeafletMapShapeEnum = request[0];
          const shapeId: number = request[1].id;
          const editingLayer = this.drawControl.editableShapes.getLayers().find((l) => l['id'] === shapeId);
          request[1].request.layer = editingLayer;
          if (shapeType === AmcsLeafletMapShapeEnum.polygon) {
            this.polygonHelper.handleEditPolygonDrawn(request[1].request, this.businessTypes);
          } else if (shapeType === AmcsLeafletMapShapeEnum.circle) {
            this.circleHelper.handleEditCircleDrawn(editingLayer);
          }
        }),
      this.amcsLeafletMapDrawService.newPolygonSaveRequest.subscribe(({ request }) => {
        this.polygonHelper.handlePolygonDrawn(request, this.businessTypes);
      }),
      this.amcsLeafletMapDrawService.editShapeSaved.subscribe(shape => {
        this.drawControl.currentlyEditableShape = 0;
        this.drawControl.updateShape(shape);
        this.drawControl.resetEditMode();
        this.mapService.fitToBoundsRequestSubject.next();
      }),
      this.amcsLeafletMapDrawService.editShapeCancelled.subscribe(() => {
        this.drawControl.cancelEditing();
        this.mapService.fitToBoundsRequestSubject.next();
        this.amcsLeafletMapDrawService.editCancel.next();
      }),
      this.amcsLeafletMapDrawService.autoFitToBoundsSubject.subscribe((autoFitToBounds) => {
        this.mapService.autoFitToBounds = autoFitToBounds;
      }),
      this.amcsLeafletMapDrawService.autoZoomSubject.subscribe((autoZoom) => {
        this.mapService.autoZoom = autoZoom;
      }),
      this.amcsLeafletMapDrawService.addGroupSubject.subscribe((groups) => {
        this.mapService.addGroupSubject.next(groups);
      }),
      this.amcsLeafletMapDrawService.updateGroupSubject.subscribe((group) => {
        this.mapService.updateGroupSubject.next(group);
      }),
      this.amcsLeafletMapDrawService.deleteGroupSubject.subscribe((groupNames) => {
        this.mapService.deleteGroupSubject.next(groupNames);
      }),
      this.amcsLeafletMapDrawService.removeShapeLayersSubject.subscribe(() => {
        this.mapService.removeShapeLayers();
      }),
      this.mapService.drawShapeStarted.subscribe(() => {
        this.drawControl.cancelEditing();
        this.amcsLeafletMapDrawService.selectedId = 0;
      }),
      this.mapService.onClickMarker.subscribe((markerId) => {
        this.amcsLeafletMapDrawService.autoFitToBoundsSubject.next(false);
        this.amcsLeafletMapDrawService.onClickMarker.next(markerId);
      }),
      this.amcsLeafletMapDrawService.panToMarkerSubject.subscribe((marker) => {
        this.mapService.panToMarkerSubject.next(marker);
      }),
      this.amcsLeafletMapDrawService.removeMapMarkerSubject.subscribe((marker) => {
        this.mapService.removeMapMarkerSubject.next(marker);
      }),
      this.amcsLeafletMapDrawService.addMapMarkerSubject.subscribe((marker) => {
        this.mapService.addMapMarkerSubject.next(marker);
      }),
      this.amcsLeafletMapDrawService.onUpdateMarkerIconSubject.subscribe((marker) => {
        this.mapService.onUpdateMarkerIconSubject.next(marker);
      }),
      this.mapService.internalMapGroupingsSubject.subscribe((group) => {
        this.amcsLeafletMapDrawService.internalMapGroupingsSubject.next(group);
      })
    );

    if (this.mapRedrawSubject) {
      this.subscriptions.push(this.mapRedrawSubject.subscribe((resize: boolean) => {
        if (resize) {
          this.mapService.fitToBoundsRequestSubject.next();
        }
      }));
    }
  }

  ngOnDestroy() {
    if (this.subscriptions && Array.isArray(this.subscriptions)) {
      this.subscriptions.forEach((sub) => sub.unsubscribe());
    }
  }

  private initHelpers() {
    this.polygonHelper = new AmcsLeafletDrawPolygonHelper(
      this.layers,
      this.notificationService,
      this.drawIntersectErrorText,
      this.amcsLeafletMapDrawService
    );
    this.circleHelper = new AmcsLeafletDrawCircleHelper(
      this.layers,
      this.notificationService,
      this.drawIntersectErrorText,
      this.amcsLeafletMapDrawService
    );
  }

  private refreshDrawHelperLayers() {
    if (!isTruthy(this.polygonHelper) || !isTruthy(this.circleHelper)) {
      this.initHelpers();
    }
    this.polygonHelper.layers = this.layers;
    this.circleHelper.layers = this.layers;
  }
}
