import { Component, Injector, OnInit, OnDestroy } from '@angular/core';
import { createCustomElement } from '@angular/elements';
import { TemplateTranslationsService } from '../services/template-translations.service';
import { AmcsLeafletMapService } from '@core-module/services/amcs-leaflet-map.service';
import { MapExampleDrawService } from '../services/map-example/map-example-draw.service';
import { MapExampleListService } from '../services/map-example/map-example-list.service';
import { Subscription, ReplaySubject } from 'rxjs';
import { AmcsLeafletMapGrouping } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-grouping.model';
import { MapExampleContext } from '../models/map-example/map-example-context.model';
import { MapExampleListComponent } from './map-example-list/map-example-list.component';
import { MapExampleMapConfig } from '../models/map-example/map-example-map-config.model';
import { MapExamplePointControl } from './map-example.control';
import { take } from 'rxjs/operators';
import { MapExample } from '../models/map-example/map-example.model';
import { AmcsLeafletMapMarker } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-marker.model';
import { AmcsCustomIconOptions } from '@shared-module/amcs-leaflet-map/custom-icon/amcs-custom-icon-options';
import { AmcsLeafletDataType } from '@shared-module/amcs-leaflet-map/enums/amcs-leaflet-datatype.enum';
import { AmcsCustomIconContainerType } from '@shared-module/amcs-leaflet-map/custom-icon/amcs-custom-icon-container-type.enum';
import { AmcsModalService } from '@shared-module/components/amcs-modal/amcs-modal.service';
import { aiComponent } from '@core-module/services/logging/ai-decorators';
import { AiViewReady } from '@core-module/services/logging/ai-view-ready.model';

@Component({
  selector: 'app-map-example',
  templateUrl: './map-example.component.html',
  styleUrls: ['./map-example.component.scss'],
  providers: [MapExampleDrawService, MapExampleListService, AmcsLeafletMapService],
})
@aiComponent('Template Map Example')
export class MapExampleComponent implements OnInit, OnDestroy {
  loading = true;
  subscriptions = new Array<Subscription>();
  mapExampleLayers: AmcsLeafletMapGrouping[] = [];
  config: MapExampleMapConfig;
  viewReady = new AiViewReady();

  constructor(
    private injector: Injector,
    private translationService: TemplateTranslationsService,
    private amcsleafetMapService: AmcsLeafletMapService,
    private mapPointListService: MapExampleListService,
    private mapPointDrawService: MapExampleDrawService,
    private modalService: AmcsModalService
  ) {}

  private mapExampleList: ReplaySubject<MapExampleContext> = new ReplaySubject<MapExampleContext>(1);

  /* ngOnInit sets up the map control with subscriptions to the relevent services*/
  ngOnInit(): void {
    this.mapPointListService.mapExampleContext$ = this.mapExampleList.asObservable();
    this.setMapPointMapContext();
    this.registerMapExampleList();
    this.config = new MapExampleMapConfig(
      this.mapPointListService,
      this.mapPointDrawService,
      this.translationService,
      this.amcsleafetMapService,
      this.modalService
    );
    this.amcsleafetMapService.addMapControl(new MapExamplePointControl(this.config), 3);

    this.subscriptions.push(
      this.mapPointListService.mapExampleList$.subscribe((data) => {
        this.setLayers(data);
        this.viewReady.next();
      }),
      this.mapPointDrawService.onAddPoint.subscribe((data) => {
        this.amcsleafetMapService.setState([this.mapMapPointToLayer(data, true, true), ...this.mapExampleLayers]);
      }),
      this.mapPointDrawService.onEditPoint.subscribe((data) => {
        this.amcsleafetMapService.setState(this.editMapPoint(data));
      }),
      this.mapPointDrawService.onCancelPoint.subscribe(() => {
        this.loadServiceAreas();
      }),
      this.mapPointDrawService.onSavePoint.subscribe(() => {
        this.loadServiceAreas();
      }),
      this.mapPointListService.mapExampleContext$.subscribe(() => {
        this.amcsleafetMapService.refreshMapControls();
        this.loadServiceAreas();
      })
    );
    this.loadServiceAreas();
    this.loading = false;
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  markerDragged(latLang: [number, number]) {
    this.mapPointDrawService.onMovePoint.next(latLang);
  }

  /* This would normal come from the API - to set a focal point on the map */
  private setMapPointMapContext() {
    this.mapExampleList.next(new MapExampleContext(51.50325, -0.127895, 1));
  }

  private loadServiceAreas() {
    this.mapPointListService.requestMapExamples();
  }

  private mapMapPointToLayer(mapPoint: MapExample, isDraggable: boolean = false, isNew: boolean = false): AmcsLeafletMapGrouping {
    return this.createLayer(+mapPoint.latitude, +mapPoint.longitude, mapPoint.description, isDraggable, isNew);
  }

  /* This allows us to edit an existing point -
    if more than one than one point is on the same lat long then the first it finds will be picked.
    Ideally you would want to add in a check on save to make sure there isn't already a point on that lat / long */
  private editMapPoint(mapPoint: MapExample): AmcsLeafletMapGrouping[] {
    this.mapExampleLayers.forEach((layer) => {
      if (layer.markers[0].latLng[0] === mapPoint.latitude && layer.markers[0].latLng[1] === mapPoint.longitude) {
        layer.markers[0].isDraggable = true;
        layer.markers[0].iconOptions = this.getMapPointIconOptions(true);
        return;
      }
    });
    return this.mapExampleLayers;
  }

  /* We need to register the component we want to add to the map so it can be displayed
and it needs to be a custom element */
  private registerMapExampleList() {
    const mapListSelector = 'app-map-example-list';
    if (!customElements.get(mapListSelector)) {
      const mapExampleListComponent = createCustomElement(MapExampleListComponent, { injector: this.injector });
      customElements.define(mapListSelector, mapExampleListComponent);
    }
  }

  /* This creates the markers displayed on the map.*/
  private createLayer(
    latitude: number,
    longitude: number,
    description: string,
    isDraggable: boolean = false,
    isNew: boolean = false
  ): AmcsLeafletMapGrouping {
    const mapGrouping = new AmcsLeafletMapGrouping();
    const marker = new AmcsLeafletMapMarker(
      AmcsLeafletDataType.Default,
      [latitude, longitude],
      undefined,
      this.getMapPointIconOptions(isNew),
      isDraggable,
      description
    );
    mapGrouping.markers.push(marker);
    return mapGrouping;
  }

  /* This returns the marker options*/
  private getMapPointIconOptions(isNew: boolean) {
    const iconOptions = new AmcsCustomIconOptions();
    iconOptions.containerType = AmcsCustomIconContainerType.Transparent;
    iconOptions.iconName = 'map-pin';
    iconOptions.iconColor = 'red';
    iconOptions.iconFontSize = 30;
    iconOptions.extraIconClasses = isNew ? 'icon-bounce' : undefined;
    iconOptions.iconAnchor = [9, 0] as [number, number];
    iconOptions.containerColorClass = 'base-gps-icon-color';
    return iconOptions;
  }

  private setLayers(mapPoints: MapExample[]) {
    this.mapPointListService.mapExampleContext$.pipe(take(1)).subscribe((data) => {
      const home = this.createHomeLayer(+data.latitude, +data.longitude);
      this.mapExampleLayers = [home, ...mapPoints.map((d) => this.mapMapPointToLayer(d))];
      this.amcsleafetMapService.setState(this.mapExampleLayers);
    });
  }

  private createHomeLayer(latitude: number, longitude: number) {
    const mapGrouping = new AmcsLeafletMapGrouping();
    const marker = new AmcsLeafletMapMarker(AmcsLeafletDataType.Default, [latitude, longitude], undefined, this.getHomeIconOptions());
    mapGrouping.markers.push(marker);
    return mapGrouping;
  }

  /* This returns the home icon options*/
  private getHomeIconOptions() {
    const iconOptions = new AmcsCustomIconOptions();
    iconOptions.containerType = AmcsCustomIconContainerType.GpsMarker;
    iconOptions.iconName = 'home';
    iconOptions.iconColor = 'white';
    iconOptions.containerColorClass = 'base-gps-icon-color';
    iconOptions.iconAnchor = [-24, -8] as [number, number];
    return iconOptions;
  }
}
