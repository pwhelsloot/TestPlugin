import { AmcsCustomIconContainerType } from '@shared-module/amcs-leaflet-map/custom-icon/amcs-custom-icon-container-type.enum';

/**
 * @deprecated Move to PlatformUI + ScaleUI (or new module) https://dev.azure.com/amcsgroup/Platform/_workitems/edit/188268
 */
export class AddressValidatorMapConfig {
    static markerIconOptions = {
        containerType: AmcsCustomIconContainerType.GpsMarker,
        iconName: 'home',
        containerColorClass: 'base-gps-icon-color',
        iconAnchor: [-24, -8] as [number, number]
    };
}
