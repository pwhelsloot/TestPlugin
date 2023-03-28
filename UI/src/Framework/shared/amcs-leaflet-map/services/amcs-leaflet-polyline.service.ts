import { Injectable } from '@angular/core';
import { AmcsLeafletMapGrouping } from '../models/amcs-leaflet-map-grouping.model';
import { AmcsLeafletBaseShapeService } from './amcs-leaflet-base-shape.service';

@Injectable()
export class AmcsLeafletPolylineService extends AmcsLeafletBaseShapeService {
  generateLayersFromGroup(group: AmcsLeafletMapGrouping) {
    group.polylines.forEach((polyline) => {
      const layer = polyline.toLayer();
      this.addLayerToMapAndCache(layer);
      polyline.leafletLayerId = layer['_leaflet_id'];
    });
  }

  removeShapesFromMap(group: AmcsLeafletMapGrouping) {
    const ids = group.getPolylineLeafletLayerIds();
    this.removeFromCacheAndMapByIds(ids);
  }
}
