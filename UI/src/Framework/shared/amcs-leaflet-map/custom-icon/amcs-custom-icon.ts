import { AmcsCustomIconContainerType } from '@shared-module/amcs-leaflet-map/custom-icon/amcs-custom-icon-container-type.enum';
import { AmcsCustomIconOptions } from '@shared-module/amcs-leaflet-map/custom-icon/amcs-custom-icon-options';
import { DivIcon } from 'leaflet';

/**
 * Simple extension class of DivIcon for easily using a library like FontAwesome along with
 * different icon/marker colors and shapes
 */
export class AmcsCustomIcon extends DivIcon {
  options: AmcsCustomIconOptions;

  constructor(iconOptions: AmcsCustomIconOptions) {
    super();
    this.options = iconOptions;
    this.options.containerSize = this.options.containerSize || 26;
    this.options.containerColor = this.options.containerColor || '';
    this.options.containerColorClass = this.options.containerColorClass || '';
    this.options.iconColor = this.options.iconColor || 'white';
    this.options.iconFontSize = this.options.iconFontSize || 17;
    this.options.popupAnchor = this.options.popupAnchor || [0, 0];
    this.options.containerAnchor = this.options.containerAnchor || [-13, 4];
    this.options.iconAnchor = this.options.iconAnchor || [-24, -12];
    this.options.tooltipAnchor = this.options.tooltipAnchor || [0, 0];
    this.options.iconName = this.options.iconName || '';
    this.options.extraIconClasses = this.options.extraIconClasses || '';
    this.options.statusIcon = this.options.statusIcon || false;
    this.options.statusIconSize = this.options.statusIconSize || 9;
    this.options.statusIconColor = this.options.statusIconColor || 'assigned';
  }

  createIcon(oldIcon: HTMLElement): HTMLElement {
    const baseContainerElement = document.createElement('div');
    baseContainerElement.setAttribute('class', 'base-icon-container');

    const pinContainerElement = document.createElement('div');
    const containerAnchor = 'top: ' + this.options.containerAnchor[0] + 'px; left: ' + this.options.containerAnchor[1] + 'px;';

    const markerTypeClass = this.generateContainerClass(this.options.containerType);
    const containerColorClass = this.options.containerColorClass ? this.options.containerColorClass : '';
    pinContainerElement.setAttribute('class', `amcs-map-icon ${markerTypeClass} ${containerColorClass}`);

    if (this.options.containerType === AmcsCustomIconContainerType.GpsMarker) {
      pinContainerElement.setAttribute('style', `height: ${this.options.containerSize}px; width: ${this.options.containerSize}px`);
    }

    let style = '';

    if (this.options.containerColor) {
      const color = this.options.containerColor.length > 0 ? this.options.containerColor : 'blue';
      style = `background-color: ${color};`;
    }

    if (this.options.containerType === AmcsCustomIconContainerType.GpsMarker) {
      style = `${style} ${containerAnchor}`;
    }

    if (this.options.containerType === AmcsCustomIconContainerType.GpsMarker) {
      style = `${style} height: ${this.options.containerSize}px; width: ${this.options.containerSize}px;`;
    }

    if (style.length > 0) {
      pinContainerElement.setAttribute('style', style);
    }

    if (this.options.statusIcon) {
      const statusIconContainerElement = document.createElement('div');
      statusIconContainerElement.setAttribute('class', `status-icon-base status-${this.options.statusIconColor}-color`);
      statusIconContainerElement
        // tslint:disable: max-line-length
        // eslint-disable-next-line max-len
        .setAttribute(
          'style',
          `top: ${this.options.iconAnchor[0]}px; left: ${this.options.iconAnchor[1]}px; height: ${this.options.statusIconSize}px; width: ${this.options.statusIconSize}px; border-radius: ${this.options.statusIconSize}px;`
        );

      baseContainerElement.appendChild(statusIconContainerElement);
    } else if (this.options.iconName) {
      const iconContainerElement = document.createElement('i');
      iconContainerElement.setAttribute('class', `fas fa-${this.options.iconName} amcsContainerIcon ${this.options.extraIconClasses}`);
      iconContainerElement.setAttribute('style', this.generateIconStyle());

      baseContainerElement.appendChild(iconContainerElement);
    }

    baseContainerElement.appendChild(pinContainerElement);

    return baseContainerElement;
  }

  private generateIconStyle(): string {
    const iconAnchor = this.options.iconAnchor
      ? `top: ${this.options.iconAnchor[0]}px; left: ${this.options.iconAnchor[1]}px`
      : 'top: -24px; left: -12px';
    let style = `color: ${this.options.iconColor}; ${iconAnchor};`;

    if (this.options.iconFontSize) {
      style += `font-size: ${this.options.iconFontSize}px;`;
    }

    return style;
  }

  private generateContainerClass(containerType: AmcsCustomIconContainerType): string {
    switch (containerType) {
      case AmcsCustomIconContainerType.Box:
        return 'amcsContainerTypeBoxMarker';
      case AmcsCustomIconContainerType.GpsMarker:
        return 'amcsContainerTypeGpsMarker';
      case AmcsCustomIconContainerType.Circle:
        return 'amcsContainerTypeCircleMarker';
      case AmcsCustomIconContainerType.Transparent:
        return 'amcsContainerTypeTransparentMarker';
    }
  }
}

export function amcsCustomIcon(options: AmcsCustomIconOptions): AmcsCustomIcon {
  return new AmcsCustomIcon(options);
}
