import { ActivityStatus } from '@core-module/models/lookups/activity-status.enum';
import { Control, ControlOptions, ControlPosition, DomEvent, DomUtil, Map } from 'leaflet';
import { AmcsCustomIcon } from '../custom-icon/amcs-custom-icon';
import { AmcsCustomIconContainerType } from '../custom-icon/amcs-custom-icon-container-type.enum';
import { AmcsCustomIconOptions } from '../custom-icon/amcs-custom-icon-options';
import { AmcsLeafletMapIconType } from '../enums/amcs-leaflet-icon-type.enum';

export class AmcsMapIconLegendControl extends Control {

  icons: AmcsLeafletMapIconType[];
  options: ControlOptions;
  translations: string[] = [];

  constructor(position: ControlPosition, icons: AmcsLeafletMapIconType[] = [], translations: string[] = []) {
    super();

    this.icons = icons;
    this.options = { position };
    this.translations = translations;
  }

  onAdd(map: Map): HTMLElement {
    const container = DomUtil.create('div', 'map-icon-legend-button');
    const link = DomUtil.create('a', 'map-icon-legend-button-link', container);

    DomUtil.create('i', 'fas fa-question-circle', link);

    const mapLegnedBase = DomUtil.create('div', 'map-legend-base', container);

    const mapIconHeader = DomUtil.create('div', 'map-icons-header', mapLegnedBase);
    DomUtil.create('i', 'far fa-map-signs', mapIconHeader);
    const mapIconHeaderLabel = DomUtil.create('span', 'map-icons-header-label', mapIconHeader);
    mapIconHeaderLabel.innerText = this.translations['map.icon-legend.title'];

    mapLegnedBase.appendChild(this.generateMapLegendIcons(this.icons));

    const closeButton = DomUtil.create('button', 'btn amcs-light-green margin-right close-legend-button', mapLegnedBase);
    closeButton.innerText = this.translations['map.icon-legend.close'];

    // We need to keep references to our event handlers in order to dispose of the event itself
    const hideLegend = () => {
      mapLegnedBase.style.display = 'none';
      DomEvent.off(closeButton, { click: hideLegend });
      DomEvent.off(mapLegnedBase, { click: DomEvent.stop, mousedown: DomEvent.stop, dblclick: DomEvent.stop });
      map.off('mousedown', hideLegend);
    };

    const showLegend = () => {
      mapLegnedBase.style.display = 'block';

      DomEvent.on(mapLegnedBase, 'click mousedown dblclick', DomEvent.stop);
      DomEvent.on(closeButton, 'click', DomEvent.stop).on(closeButton, 'click', hideLegend);
      map.on('mousedown', hideLegend);
    };

    DomEvent.on(link, 'click', DomEvent.stop).on(link, 'click', showLegend);

    return container;
  }

  private generateMapLegendIcons(icons: AmcsLeafletMapIconType[]): HTMLElement {
    const mapIcons = DomUtil.create('div', 'map-icons');

    icons.forEach(icon => {
      const mapIcon = DomUtil.create('div', 'map-icon', mapIcons);
      const label = DomUtil.create('span', 'map-icon-label', mapIcon);
      switch (icon) {
        case AmcsLeafletMapIconType.GpsRouteVisitCompleted:
          const completedIcon = this.buildRouteVisitIcon(ActivityStatus.Completed).createIcon(null);
          mapIcon.appendChild(completedIcon);
          label.innerText = this.translations['map.icon-legend.completedStop'];
          break;

        case AmcsLeafletMapIconType.GpsRouteVisitAssigned:
          const assignedIcon = this.buildRouteVisitIcon(ActivityStatus.Assigned).createIcon(null);
          mapIcon.appendChild(assignedIcon);
          label.innerText = this.translations['map.icon-legend.assignedStop'];
          break;

        case AmcsLeafletMapIconType.GpsRouteVisitRejected:
          const rejectedIcon = this.buildRouteVisitIcon(ActivityStatus.Rejected).createIcon(null);
          mapIcon.appendChild(rejectedIcon);
          label.innerText = this.translations['map.icon-legend.missedStop'];
          break;

        case AmcsLeafletMapIconType.GpsRouteVisitSelected:
          const selectedIcon = this.buildRouteVisitIcon(ActivityStatus.Assigned, true).createIcon(null);
          mapIcon.appendChild(selectedIcon);
          label.innerText = this.translations['map.icon-legend.routeVisitCurrentCustomer'];
          break;

        case AmcsLeafletMapIconType.GpsServiceLocation:
          const singleIcon = this.buildServiceLocationIcon().createIcon(null);
          mapIcon.classList.add('adjust');
          mapIcon.appendChild(singleIcon);
          label.innerText = this.translations['map.icon-legend.serviceLocation'];
          break;

        case AmcsLeafletMapIconType.Vehicle:
          const vehicleIcon = this.buildVehicleIcon().createIcon(null);
          mapIcon.classList.add('adjust');
          mapIcon.appendChild(vehicleIcon);
          label.innerText = this.translations['map.icon-legend.vehicle'];
          break;

        case AmcsLeafletMapIconType.Container:
          const containerIcon = this.buildContainerIcon().createIcon(null);
          mapIcon.classList.add('adjust');
          mapIcon.appendChild(containerIcon);
          label.innerText = this.translations['map.icon-legend.container'];
          break;
      }
    });

    return mapIcons;
  }

  private buildRouteVisitIcon(status: ActivityStatus, selectedIcon = false): AmcsCustomIcon {
    const routeVisitIconOptions = new AmcsCustomIconOptions();
    routeVisitIconOptions.containerAnchor = [-15, 3];
    routeVisitIconOptions.containerType = AmcsCustomIconContainerType.GpsMarker;

    if (selectedIcon) {
      routeVisitIconOptions.containerColorClass = 'selected-route-visit-site-marker legend-marker no-icon';
    } else {
      routeVisitIconOptions.containerColorClass = 'route-1-marker legend-marker';
      routeVisitIconOptions.statusIcon = true;
      routeVisitIconOptions.statusIconColor = this.generateStatusClass(status);
    }

    routeVisitIconOptions.iconAnchor = [-24, -4] as [number, number];

    return new AmcsCustomIcon(routeVisitIconOptions);
  }

  private buildServiceLocationIcon(): AmcsCustomIcon {
    const defaultMarker = new AmcsCustomIconOptions();
    defaultMarker.containerType = AmcsCustomIconContainerType.GpsMarker;
    defaultMarker.containerColorClass = 'route-1-marker legend-marker';
    defaultMarker.iconName = 'home';
    defaultMarker.iconColor = 'white';
    defaultMarker.iconFontSize = 16;
    defaultMarker.iconAnchor = [-24, -8] as [number, number];

    return new AmcsCustomIcon(defaultMarker);
  }

  private buildVehicleIcon(): AmcsCustomIcon {
    const vehicleIconOptions = new AmcsCustomIconOptions();
    vehicleIconOptions.containerType = AmcsCustomIconContainerType.Circle;
    vehicleIconOptions.containerColorClass = 'vehicle-marker legend-marker';
    vehicleIconOptions.iconName = 'bus';
    vehicleIconOptions.extraIconClasses = 'vehicle-icon-color circle-icon';
    vehicleIconOptions.iconAnchor = [8, 6] as [number, number];

    return new AmcsCustomIcon(vehicleIconOptions);
  }

  private buildContainerIcon(): AmcsCustomIcon {
    const containerIconOptions = new AmcsCustomIconOptions();
    containerIconOptions.containerType = AmcsCustomIconContainerType.Circle;
    containerIconOptions.containerColorClass = 'route-3-marker';
    containerIconOptions.extraIconClasses = 'circle-icon';
    containerIconOptions.iconName = 'trash';
    containerIconOptions.iconColor = 'white';
    containerIconOptions.iconAnchor = [9, 7] as [number, number];

    return new AmcsCustomIcon(containerIconOptions);
  }

  private generateStatusClass(status: ActivityStatus): 'completed' | 'assigned' | 'rejected' {
    switch (status) {
      case ActivityStatus.Rejected:
        return 'rejected';
      case ActivityStatus.Completed:
        return 'completed';
      case ActivityStatus.Assigned:
        return 'assigned';
      default:
        throw new Error('Unhandled activity status');
    }
  }
}
