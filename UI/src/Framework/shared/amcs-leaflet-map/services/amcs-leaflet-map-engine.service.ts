import { Injectable, OnDestroy } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsLeafletMapService } from '@core-module/services/amcs-leaflet-map.service';
import { ApplicationConfigurationStore } from '@core-module/services/config/application-configuration.store';
import * as L from 'leaflet';
import { Draw, featureGroup, LatLngExpression, Layer, Map, map, Marker, MarkerClusterGroup, point, TileLayer, tileLayer } from 'leaflet';
import 'proj4leaflet';
import { Subject, Subscription } from 'rxjs';
import { combineLatest, first, take, withLatestFrom } from 'rxjs/operators';
import { SharedTranslationsService } from '../../services/shared-translations.service';
import { isClusterGroupLayer } from '../amcs-leaflet-layer.helper';
import { AmcsMapIconLegendControl } from '../controls/amcs-map-icon-legend.control';
import { AmcsMapProviderControl } from '../controls/amcs-map-provider.control';
import { amcsCustomIcon } from '../custom-icon/amcs-custom-icon';
import { AmcsCustomTooltipOptions } from '../custom-icon/amcs-custom-tooltip-options';
import { AmcsMapDisplayProvider } from '../enums/amcs-map-display-provider';
import { AmcsMapProvider } from '../enums/amcs-map-provider.enum';
import { AmcsLeafletMapGrouping } from '../models/amcs-leaflet-map-grouping.model';
import { AmcsLeafletMapMarkerIdentification } from '../models/amcs-leaflet-map-marker-identification.model';
import { AmcsLeafletMapMarker } from '../models/amcs-leaflet-map-marker.model';
import { AmcsLeafletMapOptions } from '../models/amcs-leaflet-map.options';
import { AmcsLeafletCircleService } from './amcs-leaflet-circle.service';
import { AmcsLeafletMarkerService } from './amcs-leaflet-marker.service';
import { AmcsLeafletPolygonService } from './amcs-leaflet-polygon.service';
import { AmcsLeafletPolylineService } from './amcs-leaflet-polyline.service';

@Injectable()
export class AmcsLeafletMapEngineService implements OnDestroy {
  options = new AmcsLeafletMapOptions();
  onMarkerDragged$ = new Subject<[number, number] | string>();
  mapUrl: string;
  configuredDisplayProvider: string;
  crs: L.Proj.CRS;
  kartverketKey = '@KartverketLayer';
  constructor(
    private readonly mapService: AmcsLeafletMapService,
    private readonly appConfigService: ApplicationConfigurationStore,
    private readonly translationService: SharedTranslationsService,
    private readonly polygonService: AmcsLeafletPolygonService,
    private readonly polylineService: AmcsLeafletPolylineService,
    private readonly markerService: AmcsLeafletMarkerService,
    private readonly circleService: AmcsLeafletCircleService
  ) {
    this.appConfigService.configuration$
      .pipe(take(1))
      .subscribe(c => {
        this.configuredDisplayProvider = c.mapProviderConfiguration.mapDisplayProvider?.toLowerCase();
        this.setKartverket();
      });
  }
  private subscriptions = new Array<Subscription>();
  private map: Map;
  private legendControl: AmcsMapIconLegendControl;

  ngOnDestroy() {
    if (this.subscriptions) {
      this.subscriptions.forEach((sub) => sub.unsubscribe());
    }
  }

  initialize(element: string | HTMLElement) {
    this.initializeMapElement(element);
    this.setupMapInteractionEvents();

    this.map.createPane('priorityPane');
    this.addMapProviderControl();

    this.subscriptions.push(
      this.setupMapProviderStream(),
      this.setupMapControlStream(),
      this.setupLegendStream(),
      this.setupLayerStream(),
      this.setupPriorityPaneStream(),
      this.setupFitToBoundsStream(),
      this.setupUpdateIconStream(),
      this.setupPanToMarkerStream(),
      this.setupAddGroupStream(),
      this.setupDeleteGroupStream(),
      this.setupUpdateGroupStream(),
      this.setupRemoveAllShapeLayersStream(),
      this.setupRemoveMapMarkerStream(),
      this.setupAddMapMarkerStream()
    );
    if (this.options.latitude && this.options.longitude) {
      this.map.setView([this.options.latitude, this.options.longitude], 10);
    }
    this.triggerMapResize();
  }

  setupRemoveAllShapeLayersStream() {
    return this.mapService.removeShapeLayersSubject.subscribe(() => {
      this.removeAllShapeLayers();
    });
  }

  tryFitToBounds() {
    if (this.mapService.autoZoom) {
      this.fitToBounds();
    } else {
      return;
    }
  }

  /**
   * Sets the map view that contains the given
   * bounds for all Markers, Polylines and Polygons
   * @memberof AmcsLeafletMapComponent
   */
  fitToBounds() {
    const currentLayerGroup = featureGroup(this.getAllShapeLayers());
    if (currentLayerGroup.getLayers().length > 0) {
      // TODO: Temporary fix for 8.5. will be fixed soon
      // Steps to recreate : Rebuild both api & ui and edit the site area
      // It is not breaking the functionality atm.
      try {
        this.map.fitBounds(currentLayerGroup.getBounds(), {
          padding: point(15, 15),
          animate: true,
          maxZoom: 18,
        });
      } catch (error) {
        console.log(error);
      }
    } else if (this.options.latitude && this.options.longitude) {
      this.map.panTo([this.options.latitude, this.options.longitude]);
    } else {
      this.map.fitWorld({ maxZoom: 10, animate: false }).zoomIn();
    }
  }

  removeAllShapeLayers() {
    this.circleService.resetLayers();
    this.markerService.resetLayers();
    this.polylineService.resetLayers();
    this.polygonService.resetLayers();
    this.mapService.internalMapGroupingsSubject.next([]);
    this.resetLayers();
  }

  invalidateSize() {
    setTimeout(() => {
      this.map.invalidateSize();
    }, 300);
  }

  /**
   * This is important to call if we dynamically resize ( i.e., expand/deexpand ) the map
   * tile, as without a call to this method, the map will only take up the original space
   * it occupied before the resize event
   *
   * @private
   * @memberof AmcsLeafletMapComponent
   */
  private triggerMapResize() {
    setTimeout(() => {
      this.map.invalidateSize();
      this.tryFitToBounds();
    }, 300);
  }

  private getAllShapeLayers() {
    return [
      ...this.circleService.getLayers(),
      ...this.markerService.getLayers(),
      ...this.polylineService.getLayers(),
      ...this.polygonService.getLayers(),
    ];
  }

  /**
   * "Pans" or "Moves" the map to the given Marker
   *
   * @param {AmcsLeafletMapMarkerIdentification} marker
   * @memberof AmcsLeafletMapComponent
   */
  private panToMarker(marker: AmcsLeafletMapMarkerIdentification) {
    this.mapService.internalMapGroupingsSubject.pipe(take(1)).subscribe((groups) => {
      const markerGroup = groups.find((group) => group.id === marker.groupId);
      const selectedMarker = markerGroup?.markers.find((amcsMarker) => amcsMarker.id === marker.id);
      if (selectedMarker) {
        setTimeout(() => {
          if (selectedMarker.latLng) {
            this.map.panTo(selectedMarker.latLng as LatLngExpression);
          } else {
            this.tryFitToBounds();
          }
        }, 300);
      }
    });
  }

  private removeMapMarker(marker: AmcsLeafletMapMarkerIdentification) {
    this.mapService.internalMapGroupingsSubject.pipe(first()).subscribe((groups) => {
      const markerGroup = groups.find((group) => group.id === marker.groupId);
      if (isTruthy(markerGroup)) {
        const layer: Layer = this.getLayer(marker.id, markerGroup);
        if (isTruthy(layer)) {
          this.markerService.removeLayerFromCluster(layer, markerGroup.clusterGroup);
        }
      }
    });
  }

  private addMapMarker(marker: AmcsLeafletMapMarkerIdentification) {
    this.mapService.internalMapGroupingsSubject.pipe(first()).subscribe((groups) => {
      const markerGroup = groups.find((group) => group.id === marker.groupId);
      if (isTruthy(markerGroup)) {
        const layer: Layer = this.getLayer(marker.id, markerGroup);
        if (isTruthy(layer)) {
          this.markerService.addLayerToCluster(layer, markerGroup.clusterGroup);
        }
      }
    });
  }

  private getLayer(id: number, group: AmcsLeafletMapGrouping): Layer {
    let layer: Layer;
    const selectedMarker = group.markers.find((amcsMarker) => amcsMarker.id === id);
    if (isTruthy(selectedMarker)) {
      layer = this.markerService.getLayers().find(layers => layers['_leaflet_id'] === selectedMarker.leafletLayerId);
    }
    return layer;
  }


  private setupFitToBoundsStream(): Subscription {
    return this.mapService.fitToBoundsRequestSubject.subscribe(() => {
      this.triggerMapResize();
    });
  }

  private setupUpdateIconStream(): Subscription {
    return this.mapService.onUpdateMarkerIconSubject.subscribe((marker) => {
      this.toggleMarker(marker);
    });
  }

  private setupMapProviderStream(): Subscription {
    return this.mapService.selectedProvider$
      .pipe(withLatestFrom(this.appConfigService.configuration$))
      .subscribe(([mapProvider, appConfig]) => {
        switch (mapProvider) {
          case AmcsMapProvider.GoogleMapsRoadmap:
            this.onUpdateMapProvider(appConfig.mapProviderConfiguration.googleMapsUrl);
            break;
          case AmcsMapProvider.GoogleMapsSatellite:
            this.onUpdateMapProvider(appConfig.mapProviderConfiguration.googleMapsSatelliteUrl);
            break;
          case AmcsMapProvider.GoogleMapsTerrain:
            this.onUpdateMapProvider(appConfig.mapProviderConfiguration.googleMapsTerrainUrl);
            break;
          case AmcsMapProvider.HereMapsStreet:
            this.onUpdateMapProvider(appConfig.mapProviderConfiguration.hereMapsUrl);
            break;
          case AmcsMapProvider.HereMapsSatellite:
            this.onUpdateMapProvider(appConfig.mapProviderConfiguration.hereMapsSatelliteUrl);
            break;
          case AmcsMapProvider.HereMapsTerrain:
            this.onUpdateMapProvider(appConfig.mapProviderConfiguration.hereMapsTerrainUrl);
            break;
          case AmcsMapProvider.Norway:
            this.onUpdateMapProvider(this.mapUrl.replace(this.kartverketKey, 'norgeskart_bakgrunn'));
            break;
          case AmcsMapProvider.TopoGraphic:
            this.onUpdateMapProvider(this.mapUrl.replace(this.kartverketKey, 'topo4'));
            break;
          case AmcsMapProvider.TopoGraphicGrayScale:
            this.onUpdateMapProvider(this.mapUrl.replace(this.kartverketKey, 'topo4graatone'));
            break;
          case AmcsMapProvider.Kartdata:
            this.onUpdateMapProvider(this.mapUrl.replace(this.kartverketKey, 'kartdata3'));
            break;
          case AmcsMapProvider.NorwayBaseMap:
            this.onUpdateMapProvider(this.mapUrl.replace(this.kartverketKey, 'norges_grunnkart'));
            break;
          case AmcsMapProvider.NorwayBaseMapGreyScale:
            this.onUpdateMapProvider(this.mapUrl.replace(this.kartverketKey, 'norges_grunnkart_graatone'));
            break;
          case AmcsMapProvider.European:
            this.onUpdateMapProvider(this.mapUrl.replace(this.kartverketKey, 'egk'));
            break;
          case AmcsMapProvider.Seabed:
            this.onUpdateMapProvider(this.mapUrl.replace(this.kartverketKey, 'havbunn_grunnkart'));
            break;
          case AmcsMapProvider.Terrain:
            this.onUpdateMapProvider(this.mapUrl.replace(this.kartverketKey, 'terreng_norgeskart'));
            break;
          case AmcsMapProvider.Nautical:
            this.onUpdateMapProvider(this.mapUrl.replace(this.kartverketKey, 'sjokartraster'));
            break;
          case AmcsMapProvider.Toporaster:
            this.onUpdateMapProvider(this.mapUrl.replace(this.kartverketKey, 'toporaster4'));
            break;
          case AmcsMapProvider.Fjellskygge:
            this.onUpdateMapProvider(this.mapUrl.replace(this.kartverketKey, 'fjellskygge'));
            break;
          case AmcsMapProvider.Basemap:
            this.onUpdateMapProvider(this.mapUrl.replace(this.kartverketKey, 'bakgrunnskart_forenklet'));
            break;

        }
      });
  }

  private setupLayerStream(): Subscription {
    return this.mapService.layerGroups$.pipe(withLatestFrom(this.mapService.mapBounds$)).subscribe(([layerGroups, mapBounds]) => {
      this.removeAllShapeLayers();

      layerGroups.forEach((group) => this.addLayersFromGroup(group));

      if (mapBounds !== null) {
        this.map.fitBounds(mapBounds, {
          padding: point(15, 15),
          animate: true,
          maxZoom: 20,
        });
      } else if (this.mapService.autoFitToBounds) {
        this.triggerMapResize();
      }
    });
  }

  private addLayersFromGroup(group: AmcsLeafletMapGrouping) {
    this.markerService.generateLayersFromGroup(
      group,
      (event) => {
        const latlang = event.target.getLatLng();
        this.onMarkerDragged$.next([latlang.lat, latlang.lng]);
      },
      (mapMarkerIdentification: AmcsLeafletMapMarkerIdentification) => this.mapService.onClickMarker.next(mapMarkerIdentification)
    );
    this.polylineService.generateLayersFromGroup(group);
    this.polygonService.generateLayersFromGroup(group);
    this.circleService.generateLayersFromGroup(group);
    this.mapService.internalMapGroupingsSubject.pipe(take(1)).subscribe((groups) => {
      this.mapService.internalMapGroupingsSubject.next([...groups, group]);
    });
  }

  private setupRemoveMapMarkerStream(): Subscription {
    return this.mapService.removeMapMarkerSubject
      .subscribe((marker) => {
        this.removeMapMarker(marker);
      });
  }

  private setupAddMapMarkerStream(): Subscription {
    return this.mapService.addMapMarkerSubject
      .subscribe((marker) => {
        this.addMapMarker(marker);
      });
  }

  private removeGroupFromMap(groupName: string) {
    this.mapService.internalMapGroupingsSubject.pipe(take(1)).subscribe((groups) => {
      const filteredGroups = groups.filter((group) => group.name === groupName);
      if (isTruthy(filteredGroups)) {
        filteredGroups.forEach((filteredGroup) => {
          this.markerService.removeShapesFromMap(filteredGroup);
          this.polygonService.removeShapesFromMap(filteredGroup);
          this.polylineService.removeShapesFromMap(filteredGroup);
          this.circleService.removeShapesFromMap(filteredGroup);
          this.mapService.internalMapGroupingsSubject.next(groups.filter((group) => group.name !== groupName));
        });
      }
    });
  }

  /**
   * Removes all layers from the map excluding TileLayers
   *
   * @private
   * @memberof AmcsLeafletMapComponent
   */
  private resetLayers() {
    this.map.eachLayer((layer) => {
      if (!(layer instanceof TileLayer) && !isClusterGroupLayer(layer)) {
        this.map.removeLayer(layer);
      }
    });
  }

  private setupPriorityPaneStream(): Subscription {
    return this.mapService.showPriorityPane$.subscribe((show) => {
      this.map.getPane('priorityPane').hidden = show ? false : true;
    });
  }

  private setupMapInteractionEvents() {
    this.map.on(Draw.Event.CREATED, this.onLayerDrawn.bind(this));
    this.map.on(Draw.Event.DRAWSTART, this.onLayerDrawStart.bind(this));
    this.map.on('click', function(event) {
      if (event.target.scrollWheelZoom.enabled()) {
        event.target.scrollWheelZoom.disable();
      } else {
        event.target.scrollWheelZoom.enable();
      }
    });

    this.map.addEventListener('touchstart', onTwoFingerDrag);
    this.map.addEventListener('touchend', onTwoFingerDrag);
    function onTwoFingerDrag(e) {
      if (e.type === 'touchstart' && e.originalEvent.touches.length === 1) {
        e.originalEvent.currentTarget.classList.add('swiping');
      } else {
        e.originalEvent.currentTarget.classList.remove('swiping');
      }
    }
  }

  private setupPanToMarkerStream(): Subscription {
    return this.mapService.panToMarkerSubject.subscribe((marker) => {
      this.panToMarker(marker);
    });
  }

  private setupMapControlStream(): Subscription {
    return this.mapService.controls$.subscribe((controls) => {
      controls.forEach((control) => {
        this.map.removeControl(control);
        this.map.addControl(control);
      });
    });
  }

  private setupLegendStream(): Subscription {
    return this.mapService.legend$.pipe(combineLatest(this.translationService.translations)).subscribe(([icons, translations]) => {
      if (this.legendControl) {
        // We only want the control added once ...
        this.map.removeControl(this.legendControl);
      }

      this.legendControl = new AmcsMapIconLegendControl('topleft', icons, translations);
      this.map.addControl(this.legendControl);
    });
  }

  private initializeMapElement(element: string | HTMLElement) {
    if (this.configuredDisplayProvider === AmcsMapDisplayProvider.Kartverket) {
      this.map = map(element, {
        zoom: this.options.zoom,
        scrollWheelZoom: false,
        crs: this.crs
      });
    } else {
      this.map = map(element, {
        zoom: this.options.zoom,
        scrollWheelZoom: false
      });
    }

    this.initializeMarkerServices();
  }

  private initializeMarkerServices() {
    const group: MarkerClusterGroup = this.CreateClusterGroupIfEnabled();
    this.polygonService.initialize(this.map);
    this.markerService.initialize(this.map, group);
    this.polylineService.initialize(this.map);
    this.circleService.initialize(this.map);
  }

  private CreateClusterGroupIfEnabled() {
    let group: MarkerClusterGroup;
    if (this.options.enableMarkerCluster) {
      group = new MarkerClusterGroup({
        disableClusteringAtZoom: 18,
      });
    }
    return group;
  }

  private addMapProviderControl() {
    this.translationService.translations.pipe(take(1)).subscribe((translations) => {
      this.mapService.addMapControl(new AmcsMapProviderControl(this.mapService, translations, this.appConfigService), 2);
    });
  }

  private setupAddGroupStream() {
    return this.mapService.addGroupSubject.subscribe((groups) => {
      groups.forEach((group) => this.addLayersFromGroup(group));
      this.tryFitToBounds();
    });
  }

  private setupDeleteGroupStream() {
    return this.mapService.deleteGroupSubject.subscribe((groupNames) => {
      groupNames.forEach((groupName) => this.removeGroupFromMap(groupName));
      this.tryFitToBounds();
    });
  }

  private setupUpdateGroupStream() {
    return this.mapService.updateGroupSubject.subscribe((group) => {
      this.removeGroupFromMap(group.name);
      this.addLayersFromGroup(group);
      this.tryFitToBounds();
    });
  }

  private onUpdateMapProvider(url: string, maxZoom = 20, subdomains = ['mt0', 'mt1', 'mt2', 'mt3'], detectRetina = true) {
    if (this.configuredDisplayProvider === AmcsMapDisplayProvider.Kartverket) {
      subdomains = ['', '2', '3'];
    }
    this.map.addLayer(
      tileLayer(url, {
        maxZoom,
        subdomains,
        detectRetina,
      })
    );
    this.map.eachLayer((layer) => {
      if (layer instanceof TileLayer) {
        this.map.removeLayer(layer);
        this.map.addLayer(
          tileLayer(url, {
            maxZoom,
            subdomains,
            detectRetina,
          })
        );
      }
    });

    if (this.configuredDisplayProvider === AmcsMapDisplayProvider.Kartverket) {
      L.tileLayer.wms('https://openwms.statkart.no/skwms1/wms.matrikkel?', {
        layers: 'matrikkel_WMS',
        transparent: true,
        format: 'image/png',
      }).addTo(this.map);
    }
  }

  private onLayerDrawn(e: any) {
    this.mapService.shapeDrawnSubject.next(e.layer);
  }

  private onLayerDrawStart(e: any) {
    this.mapService.drawShapeStarted.next();
  }

  private toggleMarker(selectedMarker: AmcsLeafletMapMarker) {
    let found = false;
    this.mapService.internalMapGroupingsSubject.subscribe((groups) => {
      groups.every((group) => {
        if (found) {
          return false;
        } else {
          const foundMarker = group.markers.find(
            (marker) => marker.id === selectedMarker.id && marker.leafletLayerId === selectedMarker.leafletLayerId
          );
          if (foundMarker) {
            this.map.eachLayer((mapLayer) => {
              if (mapLayer['_leaflet_id'] === foundMarker.leafletLayerId) {
                // setIcon calls bindPopup which removes the click event from the marker,
                // so we find it if exists and add we add it again
                let clickFn;
                if (foundMarker.isClickable) {
                  clickFn = mapLayer['_events']['click'][0].fn;
                }
                (mapLayer as Marker).setIcon(amcsCustomIcon(selectedMarker.iconOptions));
                if (clickFn) {
                  mapLayer.on('click', clickFn);
                }
                this.updateTooltip(mapLayer);
                found = true;
              }
            });
          }
          return true;
        }
      });
    });
  }

  private updateTooltip(mapLayer: Layer) {
    const tooltip = (mapLayer as Marker).getTooltip();
    if (tooltip) {
      const content = tooltip.getContent();
      (mapLayer as Marker).unbindTooltip();
      const tooltipOptions = tooltip.options as AmcsCustomTooltipOptions;
      const newTooltipOptions = this.createNewTooltipOptions(tooltipOptions);
      if (tooltip.options.className === tooltipOptions.originalTooltipClassName) {
        newTooltipOptions.className = tooltipOptions.selectedTooltipClassName;
      } else if (tooltip.options.className === tooltipOptions.selectedTooltipClassName) {
        newTooltipOptions.className = tooltipOptions.originalTooltipClassName;
      }
      (mapLayer as Marker).bindTooltip(content, newTooltipOptions);
    }
  }

  private createNewTooltipOptions(tooltipOptions: AmcsCustomTooltipOptions) {
    return {
      permanent: tooltipOptions.permanent,
      direction: tooltipOptions.direction,
      originalTooltipClassName: tooltipOptions.originalTooltipClassName,
      selectedTooltipClassName: tooltipOptions.selectedTooltipClassName,
      delay: tooltipOptions.delayDuration,
    } as AmcsCustomTooltipOptions;
  }

  private setKartverket() {
    if (this.configuredDisplayProvider === AmcsMapDisplayProvider.Kartverket) {
      const crsStr = 'EPSG:25833';
      const crsProj4 = '+proj=utm +zone=33 +ellps=GRS80 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs';
      const crsBaseResolution = 21664;
      const crsMaxZoom = this.options.zoom;
      this.crs = new L.Proj.CRS(crsStr, crsProj4,
        {
          origin: [-2500000, 9045984],
          resolutions: Array.from(Array(crsMaxZoom + 1), (e, zoomLevel) => crsBaseResolution / Math.pow(2, zoomLevel))
        });
      this.mapUrl = 'https://opencache{s}.statkart.no/gatekeeper/gk/gk.open_wmts?&layer=' + this.kartverketKey + '&style=default&tilematrixset=' + crsStr +
        '&Service=WMTS&Request=GetTile&Version=1.0.0&Format=image%2Fpng&TileMatrix=' + crsStr + ':{z}&TileCol={x}&TileRow={y}';
    }
  }
}
