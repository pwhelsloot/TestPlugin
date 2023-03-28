import { Injectable } from '@angular/core';
import { AmcsLeafletMapGrouping } from '../models/amcs-leaflet-map-grouping.model';
import { AmcsLeafletBaseShapeService } from './amcs-leaflet-base-shape.service';

@Injectable()
export class AmcsLeafletPolygonService extends AmcsLeafletBaseShapeService {
  generateLayersFromGroup(group: AmcsLeafletMapGrouping) {
    group.polygons.forEach((polygon) => {
      const layer = polygon.toLayer();
      this.addLayerToMapAndCache(layer);
      polygon.leafletLayerId = layer['_leaflet_id'];
    });
  }

  removeShapesFromMap(group: AmcsLeafletMapGrouping) {
    const ids = group.getPolygonLeafletLayerIds();
    this.removeFromCacheAndMapByIds(ids);
  }
}
