import { AmcsLeafletMapCircle } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-circle';
import { AmcsLeafletMapMarker } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-marker.model';
import { AmcsLeafletMapPolyline } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-polyline.model';
import { AmcsLeafletMapMarkerClusterGroup } from './amcs-leaflet-map-marker-cluster-group.model';
import { AmcsLeafletMapPolygon } from './amcs-leaflet-map-polygon.model';

export class AmcsLeafletMapGrouping {
  id: number;
  name: string;
  markers: AmcsLeafletMapMarker[] = [];
  polylines: AmcsLeafletMapPolyline[] = [];
  circles: AmcsLeafletMapCircle[] = [];
  polygons: AmcsLeafletMapPolygon[] = [];
  clusterGroup: AmcsLeafletMapMarkerClusterGroup;

  getLeafletLayerIds() {
    return [
      this.getCircleLeafletLayerIds(),
      this.getMarkerLeafletLayerIds(),
      this.getPolygonLeafletLayerIds(),
      this.getPolylineLeafletLayerIds(),
    ];
  }

  getCircleLeafletLayerIds() {
    return this.circles.map((circle) => circle.leafletLayerId);
  }
  getMarkerLeafletLayerIds() {
    return this.markers.map((marker) => marker.leafletLayerId);
  }
  getPolygonLeafletLayerIds() {
    return this.polygons.map((polygon) => polygon.leafletLayerId);
  }
  getPolylineLeafletLayerIds() {
    return this.polylines.map((polyline) => polyline.leafletLayerId);
  }
}
