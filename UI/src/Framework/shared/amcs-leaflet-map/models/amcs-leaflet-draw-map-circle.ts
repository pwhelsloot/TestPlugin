import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsLeafletDataType } from '@shared-module/amcs-leaflet-map/enums/amcs-leaflet-datatype.enum';
import * as T from '@turf/circle';
import { Feature, Polygon, Units } from '@turf/helpers';
import intersect from '@turf/intersect';
import { circle, CircleMarkerOptions, Layer } from 'leaflet';
import { AmcsLeafletMapCircle } from './amcs-leaflet-map-circle';

export class AmcsLeafletDrawMapCircle extends AmcsLeafletMapCircle {

    circlePolygon: Feature<Polygon>;

    constructor(public id: number, coordinates: [number, number], radiusMetres: number, public title?: string, public onClickFn?: Function, public options?: CircleMarkerOptions) {
        super(AmcsLeafletDataType.Default, coordinates, radiusMetres);
    }

    toLayer(): Layer {
        const circleOptions: L.CircleMarkerOptions = {
            radius: this.radiusMetres,
            opacity: this.opacity,
            interactive: true,
            // Dishes out numbers 1-6 for polygon classes
            className: this.getClassName(),
        };

        if(this.options) {
            this.options.radius = this.radiusMetres;
            // Dishes out numbers 1-6 for polygon classes
            this.options.className = this.getClassName();
        }

        const layer = circle(this.coordinates, this.options ?? circleOptions);
        if (this.onClickFn) {
            layer.addEventListener('click', (x) => this.onClickFn(x.target.id));
        }

        layer['id'] = this.id;

        if (isTruthy(this.title)) {
            layer.bindTooltip(this.title, { permanent: true });
        }

        this.circlePolygon = this.createPolygonFromCircle();
        return layer;
    }

    overlaps(area: AmcsLeafletDrawMapCircle): boolean {
        const newCirclePolygon = this.createPolygonFromCircle();
        return isTruthy(intersect(newCirclePolygon, area.circlePolygon));
    }

    createPolygonFromCircle(): Feature<Polygon> {
        const units: Units = 'meters';
       return T.default(this.coordinates, this.radiusMetres, { units });
    }

    static fromLayer(layer: L.Circle): AmcsLeafletDrawMapCircle {

        const amcsCircle = new AmcsLeafletDrawMapCircle(
            layer['id'],
            [layer.getLatLng().lat, layer.getLatLng().lng],
            layer.getRadius(),
        );
        return amcsCircle;
    }

    private getClassName() {
        return 'amcs-draw-circle-' + (((this.id + 5) % 6) + 1).toString();
    }
}
