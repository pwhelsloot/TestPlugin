import { AmcsLeafletDrawMapCircle } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-draw-map-circle';
import { OperationalAreaRequest } from '@shared-module/amcs-leaflet-map/models/operational-area-request.model';
import { Feature, Polygon } from '@turf/helpers';
import { AmcsLeafletMapPolygon } from '../../shared/amcs-leaflet-map/models/amcs-leaflet-map-polygon.model';
import { AmcsMapArea } from '../models/amcs-map-area.model';
import { AmcsMapPosition } from '../models/amcs-map-position-model';

export class MapAreaHelper {
  static mapPolygonToMapArea(polygon: AmcsLeafletMapPolygon) {
    const mapArea = new AmcsMapArea();
    mapArea.positions = polygon.coordinates[0].map(point => {
      const t = new AmcsMapPosition();
      t.lat = point[0];
      t.long = point[1];
      return t;
    });
    return mapArea;
  }

  static mapAreaToPolygon(id: number, title: string, mapArea: AmcsMapArea, op?: OperationalAreaRequest): AmcsLeafletMapPolygon {
    const polygon = new AmcsLeafletMapPolygon(
      id,
      mapArea.positions.map(position => {
        return [position.lat, position.long];
      }),
      op,
      title
    );
    return polygon;
  }

  static mapAreaToCircle(id: number, title: string, latitude: number, longitude: number, radiusMeters: number): AmcsLeafletDrawMapCircle {
    const latLng: [number, number] = [latitude, longitude];
    const circle = new AmcsLeafletDrawMapCircle(id, latLng, radiusMeters, title);
    return circle;
  }

  static mapCircleToMapArea(circle: Feature<Polygon>) {
    const mapArea = new AmcsMapArea();
    mapArea.positions = circle.geometry.coordinates[0].map(point => {
      const t = new AmcsMapPosition();
      t.lat = point[0];
      t.long = point[1];
      return t;
    });
    return mapArea;
  }
}
