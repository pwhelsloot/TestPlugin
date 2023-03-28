import { Injectable } from '@angular/core';
import { DragEndEventHandlerFn, Layer } from 'leaflet';
import { AmcsLeafletMapGrouping } from '../models/amcs-leaflet-map-grouping.model';
import { AmcsLeafletMapMarkerIdentification } from '../models/amcs-leaflet-map-marker-identification.model';
import { AmcsLeafletMapMarker } from '../models/amcs-leaflet-map-marker.model';
import { AmcsLeafletBaseShapeService } from './amcs-leaflet-base-shape.service';

@Injectable()
export class AmcsLeafletMarkerService extends AmcsLeafletBaseShapeService {
  generateLayersFromGroup(group: AmcsLeafletMapGrouping, draggedFunction?: DragEndEventHandlerFn, clickFunction?: Function) {
    group.markers.forEach((mapMarker) => {
      const layer = mapMarker.toLayer();
      this.setDraggedFunction(mapMarker, layer, draggedFunction);
      this.setIsClickable(mapMarker, group, layer, clickFunction);
      this.addLayerToMapAndCache(layer, group.clusterGroup);
      mapMarker.leafletLayerId = layer['_leaflet_id'];
    });
    this.refreshClusterLayer();
  }

  removeShapesFromMap(group: AmcsLeafletMapGrouping) {
    const ids = group.getMarkerLeafletLayerIds();
    this.removeFromCacheAndMapByIds(ids);
    this.refreshClusterLayer();
  }

  setIsClickable(mapMarker: AmcsLeafletMapMarker, group: AmcsLeafletMapGrouping, layer: Layer, clickFunction: Function) {
    if (mapMarker.isClickable) {
      const mapMarkerIdentification = new AmcsLeafletMapMarkerIdentification();
      mapMarkerIdentification.id = mapMarker.id;
      mapMarkerIdentification.groupId = group.id;
      layer.on('click', () => clickFunction(mapMarkerIdentification));
    }
  }

  setDraggedFunction(mapMarker: AmcsLeafletMapMarker, layer: Layer, draggedFunction: DragEndEventHandlerFn) {
    if (mapMarker.isDraggable) {
      layer.on('dragend', draggedFunction);
    }
  }
}
