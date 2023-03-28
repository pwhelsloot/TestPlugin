import { amcsCustomIcon } from '@shared-module/amcs-leaflet-map/custom-icon/amcs-custom-icon';
import { AmcsCustomIconOptions } from '@shared-module/amcs-leaflet-map/custom-icon/amcs-custom-icon-options';
import { AmcsLeafletDataType } from '@shared-module/amcs-leaflet-map/enums/amcs-leaflet-datatype.enum';
import { geoJSON, LatLngExpression, Layer, marker, Marker } from 'leaflet';
import { BehaviorSubject } from 'rxjs';
import { AmcsCustomTooltipOptions } from '../custom-icon/amcs-custom-tooltip-options';
import { AmcsLeafletMapBaseShape } from './amcs-leaflet-map-base-shape.model';
import { amcsMarker } from './amcs-leaflet-marker-extensions';

export class AmcsLeafletMapMarker extends AmcsLeafletMapBaseShape {
  priority: boolean;
  isMouseOut = true;
  infoTextPayload: number;

  // Suggestion: refactor to use a Options object so its 1 param instead of 7
  constructor(
    public dataType: AmcsLeafletDataType,
    public latLng: [number, number] | string,
    public infoText?: string,
    public iconOptions?: AmcsCustomIconOptions,
    public isDraggable?: boolean,
    public tooltip?: string,
    public tooltipOptions?: AmcsCustomTooltipOptions,
    public isClickable?: boolean
  ) {
    super();
  }

  infoTextFunction?(id: number): BehaviorSubject<string | HTMLElement>;

  toLayer(): Layer {
    switch (this.dataType) {
      case AmcsLeafletDataType.Default:
        const delayDuration: number = this.tooltipOptions?.delayDuration ?? 0;
        const defaultMarker = amcsMarker(this.latLng as LatLngExpression, {
          draggable: this.isDraggable,
          icon: amcsCustomIcon(this.iconOptions),
          pane: this.priority ? 'priorityPane' : 'markerPane',
          delayDuration,
          infoText: this.infoText,
          infoTextFunction: this.infoTextFunction,
          infoTextPayload: this.infoTextPayload ? this.infoTextPayload : this.id,
        });
        if (this.tooltip) {
          this.setTooltip(defaultMarker);
        }

        return defaultMarker;
      case AmcsLeafletDataType.GeoJson:
        return geoJSON(JSON.parse(this.latLng as string), {
          pointToLayer: (feature, latLng) => {
            const jsonMarker = marker(latLng, {
              draggable: this.isDraggable,
              icon: amcsCustomIcon({
                containerType: this.iconOptions.containerType,
                iconName: this.iconOptions.iconName,
                containerColor: this.iconOptions.containerColor,
              }),
              pane: this.priority ? 'priorityPane' : 'markerPanel',
            });

            if (this.infoText) {
              jsonMarker.bindPopup(this.infoText);
            }
            if (this.tooltip) {
              this.setTooltip(jsonMarker);
            }

            return jsonMarker;
          },
        });
    }
  }

  private setTooltip(leafletMarker: Marker<any>) {
    leafletMarker.bindTooltip(this.tooltip, this.getTooltipOptions());
  }

  private getTooltipOptions() {
    if (this.tooltipOptions) {
      return this.tooltipOptions;
    } else {
      return { permanent: true } as AmcsCustomTooltipOptions;
    }
  }
}
