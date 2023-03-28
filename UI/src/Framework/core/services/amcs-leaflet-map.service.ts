import { Injectable } from '@angular/core';
import { BaseService } from '@coreservices/base.service';
import { ApplicationConfigurationStore } from '@coreservices/config/application-configuration.store';
import { AmcsLeafletMapIconType } from '@shared-module/amcs-leaflet-map/enums/amcs-leaflet-icon-type.enum';
import { AmcsMapDisplayProvider } from '@shared-module/amcs-leaflet-map/enums/amcs-map-display-provider';
import { AmcsMapProvider } from '@shared-module/amcs-leaflet-map/enums/amcs-map-provider.enum';
import { AmcsLeafletMapGrouping } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-grouping.model';
import { AmcsLeafletMapMarkerIdentification } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-marker-identification.model';
import { AmcsLeafletMapMarker } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-marker.model';
import { AmcsLeafletMapPolygon } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-polygon.model';
import { AmcsLeafletMapShapeEnum } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-shape.enum';
import { Control, LatLngBounds, Layer } from 'leaflet';
import { BehaviorSubject, Observable, ReplaySubject, Subject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators';

@Injectable()
export class AmcsLeafletMapService extends BaseService {
  layerGroups$: Observable<AmcsLeafletMapGrouping[]>;
  showPriorityPane$: Observable<boolean>;
  legend$: Observable<AmcsLeafletMapIconType[]>;

  // this allows us to override the map bounds.
  // if this is null, the map will fit to bounds based on the contents of the map layers.
  mapBounds$: Observable<LatLngBounds>;

  // a stream of custom leaflet controls.
  // this allows a map consumer to add their own leaflet controls.
  // the map should listen to this and add these controls.
  controls$: Observable<Control[]>;

  selectedProvider$: Observable<AmcsMapProvider>;

  fitToBoundsRequestSubject = new Subject();

  panToMarkerSubject = new Subject<AmcsLeafletMapMarkerIdentification>();

  shapeDrawnSubject = new Subject<Layer>();

  addShapeSubject = new Subject<{ type: AmcsLeafletMapShapeEnum; shape: Layer }>();

  addGroupSubject = new Subject<AmcsLeafletMapGrouping[]>();
  updateGroupSubject = new Subject<AmcsLeafletMapGrouping>();
  deleteGroupSubject = new Subject<string[]>();

  drawShapeStarted = new Subject<AmcsLeafletMapPolygon>();

  onClickMarker = new Subject<AmcsLeafletMapMarkerIdentification>();

  onUpdateMarkerIconSubject = new Subject<AmcsLeafletMapMarker>();

  removeShapeLayersSubject = new Subject();

  removeMapMarkerSubject = new Subject<AmcsLeafletMapMarkerIdentification>();
  addMapMarkerSubject = new Subject<AmcsLeafletMapMarkerIdentification>();

  // if set to true, the map will fit to bounds every time the map marker layers are changed
  autoFitToBounds = true;

  // if set to true, the map will fit to bounds when tryFitToBounds is called
  autoZoom = true;

  // Needs to be refacored, do not use this
  internalMapGroupingsSubject = new ReplaySubject<AmcsLeafletMapGrouping[]>(1);

  constructor(private appConfigService: ApplicationConfigurationStore) {
    super();
    this.layerGroups = new ReplaySubject<AmcsLeafletMapGrouping[]>(1);
    this.layerGroups$ = this.layerGroups.asObservable();

    this.showPriorityPane = new Subject<boolean>();
    this.showPriorityPane$ = this.showPriorityPane.asObservable();

    this.legend = new ReplaySubject<AmcsLeafletMapIconType[]>(1);
    this.legend$ = this.legend.asObservable();

    this.mapBounds = new BehaviorSubject<LatLngBounds>(null);
    this.mapBounds$ = this.mapBounds.asObservable();

    this.controlsSubject = new BehaviorSubject<Control[]>([]);
    this.controls$ = this.controlsSubject.asObservable();

    this.selectedProviderSubject = new ReplaySubject<AmcsMapProvider>(1);
    this.selectedProvider$ = this.selectedProviderSubject.asObservable();

    this.appConfigService.configuration$
      .pipe(takeUntil(this.unsubscribe))
      .subscribe(c => {
        if (c.mapProviderConfiguration.mapDisplayProvider.toLowerCase() === AmcsMapDisplayProvider.Google) {
          this.selectedProviderSubject.next(AmcsMapProvider.GoogleMapsRoadmap);
        } else if (c.mapProviderConfiguration.mapDisplayProvider.toLowerCase() === AmcsMapDisplayProvider.Kartverket) {
          this.selectedProviderSubject.next(AmcsMapProvider.Norway);
        } else {
          this.selectedProviderSubject.next(AmcsMapProvider.HereMapsStreet);
        }
      });
  }

  private layerGroups: ReplaySubject<AmcsLeafletMapGrouping[]>;
  private showPriorityPane: Subject<boolean>;
  private legend: ReplaySubject<AmcsLeafletMapIconType[]>;
  private mapBounds: BehaviorSubject<LatLngBounds>;
  private controlsSubject: BehaviorSubject<Control[]>;
  private selectedProviderSubject: ReplaySubject<AmcsMapProvider>;

  // used to ensure the same control is not added more than once.
  private controlIds: number[] = [];

  setMapProvider(provider: AmcsMapProvider) {
    this.selectedProviderSubject.next(provider);
  }

  setLegend(availableIcons: AmcsLeafletMapIconType[]) {
    this.legend.next(availableIcons);
  }

  setState(groupings: AmcsLeafletMapGrouping[], bounds: LatLngBounds = null) {
    this.mapBounds.next(bounds);
    this.layerGroups.next(groupings);
  }

  clearState() {
    this.layerGroups.next([]);
  }

  setPriorityPaneVisible(show: boolean) {
    this.showPriorityPane.next(show);
  }

  addMapControl(control: Control, controlId: number) {
    if (!this.controlIds.find((x) => x === controlId)) {
      this.controlIds.push(controlId);
      this.controlsSubject.pipe(take(1)).subscribe((controls) => {
        controls.push(control);
        this.controlsSubject.next(controls);
      });
    }
  }

  refreshMapControls() {
    this.controlsSubject.pipe(take(1)).subscribe((controls) => {
      this.controlsSubject.next(controls);
    });
  }

  removeShapeLayers() {
    this.removeShapeLayersSubject.next();
  }
}
