import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { Polygon } from '@turf/helpers';
import intersect from '@turf/intersect';
import { LatLng, polygon } from 'leaflet';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { AmcsLeafletMapBaseShape } from './amcs-leaflet-map-base-shape.model';
import { OperationalAreaRequest } from './operational-area-request.model';

export class AmcsLeafletMapPolygon extends AmcsLeafletMapBaseShape {
  // Needed to implement GeoJson Polygon interface (used for overlap check)
  type = 'Polygon';
  coordinates: number[][][];
  constructor(id: number, latLongs: number[][], public op?: OperationalAreaRequest, public title?: string, public onClickFn?: Function) {
    super();
    this.id = id;
    this.coordinates = [latLongs];
  }

  infoTextFunction?(id: number): BehaviorSubject<string>;

  toLayer(): L.Polygon {
    const layer: L.Polygon = polygon(this.convertNumberArrayToLatLongs(this.coordinates[0]), {
      // Dishes out numbers 1-6 for polygon classes
      className: this.op?.isServiceExclusion
        ? 'amcs-draw-polygon-exclusion amcs-exclusion-polygon-hide'
        : 'amcs-draw-polygon-' + (((this.id + 5) % 6) + 1).toString(),
    });
    if (this.onClickFn) {
      layer.addEventListener('click', (x) => this.onClickFn(x.target.id));
    }
    if (this.infoTextFunction) {
      layer.bindPopup('');
      this.infoTextFunction(this.id)
        .pipe(take(1))
        .subscribe((infoText) => {
          layer.setPopupContent(infoText);
        });
    }
    layer['id'] = this.id;
    layer['data'] = this.op;
    layer['title'] = this.title;
    if (isTruthy(this.title) && !this.op?.isServiceExclusion) {
      layer.bindTooltip(this.title, { permanent: true });
    }
    return layer;
  }

  overlaps(area: AmcsLeafletMapPolygon): boolean {
    return isTruthy(intersect(this as Polygon, area as Polygon));
  }

  static fromLayer(layer: L.Polygon): AmcsLeafletMapPolygon {
    const amcsPolygon = new AmcsLeafletMapPolygon(layer['id'], this.convertLatLongsToNumberArray(layer.getLatLngs()), layer['data']);
    return amcsPolygon;
  }

  private convertNumberArrayToLatLongs(latLongs: number[][]): LatLng[] {
    const result: L.LatLng[] = [];
    latLongs.forEach((element) => {
      const latLong = new LatLng(element[0], element[1]);
      result.push(latLong);
    });
    // Here we need to remove the final position as we know that's the duplicate.
    result.pop();
    return result;
  }

  private static convertLatLongsToNumberArray(latLongs: LatLng[] | LatLng[][] | LatLng[][][]): number[][] {
    const result: number[][] = [];
    (latLongs as LatLng[][])[0].forEach((element) => {
      element = element.wrap();
      result.push([element.lat, element.lng]);
    });
    // Turf+SQL requires the first/end point to be the same which leaflet doesnt provide
    result.push(result[0]);
    return result;
  }
}
