import { AmcsLeafletDataType } from '@shared-module/amcs-leaflet-map/enums/amcs-leaflet-datatype.enum';
import { Layer, polyline } from 'leaflet';
import { antPath } from 'leaflet-ant-path';
import { AmcsLeafletMapBaseShape } from './amcs-leaflet-map-base-shape.model';
import { AmcsLeafletMapPulseOptions } from './amcs-leaflet-map-pulse-options.model';

export class AmcsLeafletMapPolyline extends AmcsLeafletMapBaseShape {
  dataType: AmcsLeafletDataType;
  coordinates: [number, number][] | string;
  infoText: string | false;
  color = 'blue';
  colorClass = '';
  useAntPath = false;
  pulsOpts: AmcsLeafletMapPulseOptions;

  constructor(
    dataType: AmcsLeafletDataType,
    coordinates: [number, number][] | string,
    infoText?: string | false,
    color?: string,
    colorClass?: string,
    useAntPath?: boolean,
    pulsOpts?: AmcsLeafletMapPulseOptions
  ) {
    super();
    this.dataType = dataType;
    this.coordinates = coordinates;
    this.infoText = infoText;
    this.color = color || this.color;
    this.colorClass = colorClass || '';
    this.useAntPath = useAntPath || this.useAntPath;
    this.pulsOpts = pulsOpts || this.pulsOpts;
  }

  toLayer(): Layer {
    switch (this.dataType) {
      case AmcsLeafletDataType.Default:
        let pathOptions = { opacity: 0.5, weight: 5 };

        if (this.useAntPath && this.pulsOpts) {
          pathOptions = { ...pathOptions, ...this.pulsOpts};
        }
        if (this.colorClass !== '') {
          pathOptions['className'] = this.colorClass;
        } else {
          pathOptions['color'] = this.color;
        }

        const tempPolyline = this.useAntPath
          ? antPath([...(this.coordinates as [number, number][])]).setStyle(pathOptions)
          : polyline([...(this.coordinates as [number, number][])]).setStyle(pathOptions);

        if (this.infoText) {
          tempPolyline.bindTooltip(this.infoText);
        }

        return tempPolyline;

      default:
        throw new Error('Unhandled layer datatype');
    }
  }
}
