import { AmcsLeafletDataType } from '@shared-module/amcs-leaflet-map/enums/amcs-leaflet-datatype.enum';
import { circle, Layer } from 'leaflet';
import { AmcsLeafletMapBaseShape } from './amcs-leaflet-map-base-shape.model';

export class AmcsLeafletMapCircle extends AmcsLeafletMapBaseShape {
  dataType: AmcsLeafletDataType;
  coordinates: [number, number];
  radiusMetres: number;
  color = 'red';
  opacity = 0.3;

  constructor(dataType: AmcsLeafletDataType, coordinates: [number, number], radiusMetres: number) {
    super();
    this.dataType = dataType;
    this.coordinates = coordinates;
    this.radiusMetres = radiusMetres;
  }

  toLayer(): Layer {
    switch (this.dataType) {
      case AmcsLeafletDataType.Default:
        const circleOptions = {
          radius: this.radiusMetres,
          color: this.color,
          opacity: this.opacity,
          interactive: false,
        };

        return circle(this.coordinates, circleOptions);

      default:
        throw new Error('Unhandled layer datatype');
    }
  }
}
