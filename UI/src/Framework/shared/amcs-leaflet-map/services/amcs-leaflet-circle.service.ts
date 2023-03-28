import { Injectable } from '@angular/core';
import { AmcsLeafletMapGrouping } from '../models/amcs-leaflet-map-grouping.model';
import { AmcsLeafletBaseShapeService } from './amcs-leaflet-base-shape.service';

@Injectable()
export class AmcsLeafletCircleService extends AmcsLeafletBaseShapeService {
  generateLayersFromGroup(group: AmcsLeafletMapGrouping) {
    group.circles.forEach((circle) => {
      const layer = circle.toLayer();
      this.addLayerToMapAndCache(layer);
      circle.leafletLayerId = layer['_leaflet_id'];
    });
  }

  removeShapesFromMap(group: AmcsLeafletMapGrouping) {
    const ids = group.getCircleLeafletLayerIds();
    this.removeFromCacheAndMapByIds(ids);
  }
}
