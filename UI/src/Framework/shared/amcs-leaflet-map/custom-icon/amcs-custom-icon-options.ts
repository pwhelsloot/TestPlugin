import { AmcsCustomIconContainerType } from '@shared-module/amcs-leaflet-map/custom-icon/amcs-custom-icon-container-type.enum';
import { BaseIconOptions } from 'leaflet';

export class AmcsCustomIconOptions implements BaseIconOptions {
    containerType: AmcsCustomIconContainerType;
    containerSize?: number;
    containerColor?: string;
    containerColorClass?: string;
    containerAnchor?: [number, number];
    iconAnchor?: [number, number];
    popupAnchor?: [number, number];
    tooltipAnchor?: [number, number];
    iconName?: string;
    iconColor?: string;
    iconFontSize?: number;
    extraIconClasses?: string;
    statusIcon?: boolean;
    statusIconSize?: number;
    statusIconColor?: 'completed' | 'assigned' | 'rejected';
}
