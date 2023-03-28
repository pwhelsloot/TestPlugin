import { ControlOptions, Control, Map, DomEvent as LeafLetDomEvent } from 'leaflet';
import { NgElement, WithProperties } from '@angular/elements';
import { MapExampleMapConfig } from '../models/map-example/map-example-map-config.model';
import { MapExampleListComponent } from './map-example-list/map-example-list.component';

export class MapExamplePointControl extends Control {
  options: ControlOptions;

  constructor(private config: MapExampleMapConfig) {
    super();
    this.setPosition('topleft');
  }

  /*onAdd will add the component into the map and make sure the mouse is crontrolled around it. */
  onAdd(map: Map): HTMLElement {
    const mapExampleList: NgElement & WithProperties<MapExampleListComponent> = document.createElement('app-map-example-list') as any;
    mapExampleList.config = this.config;

    this.setZoomControlPosition(map);
    LeafLetDomEvent.disableClickPropagation(mapExampleList);
    LeafLetDomEvent.disableScrollPropagation(mapExampleList);

    // Disable events when user's cursor enters the component
    mapExampleList.addEventListener('mouseover', function () {
      map.dragging.disable();
      map.touchZoom.disable();
      map.doubleClickZoom.disable();
      map.scrollWheelZoom.disable();
      map.boxZoom.disable();
      map.keyboard.disable();
    });

    // Re-enable events when user's cursor leaves the component
    mapExampleList.addEventListener('mouseout', function () {
      map.dragging.enable();
      map.touchZoom.enable();
      map.doubleClickZoom.enable();
      map.scrollWheelZoom.enable();
      map.boxZoom.enable();
      map.keyboard.enable();
    });
    return mapExampleList;
  }

  private setZoomControlPosition(map: Map) {
    if (map.zoomControl) {
      map.zoomControl.setPosition('bottomright');
    }
  }
}
