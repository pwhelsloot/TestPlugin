import { AmcsLeafletMapHeightUnit } from './amcs-leaflet-map-height-unit';

export class AmcsLeafletMapOptions {
  /**
   * Longitude the map should initially center on
   *
   * @type {number}
   * @memberof AmcsLeafletMapComponent
   */
  longitude: number;

  /**
   * Latitude the map should initially center on
   *
   * @type {number}
   * @memberof AmcsLeafletMapComponent
   */
  latitude: number;

  /**
   * Initial map zoom
   *
   * @memberof AmcsLeafletMapComponent
   */
  zoom = 20;

  /**
   * Actual height of the map being displayed
   *
   * @memberof AmcsLeafletMapComponent
   */
  mapHeight = 600;

  mapHeightUnit: AmcsLeafletMapHeightUnit;

  /**
   * Whether to show 'expanded' features on the map
   *
   * @memberof AmcsLeafletMapComponent
   */
  expandedMode = false;

  /**
   * Enable Marker Cluster Groups
   *
   * @memberof AmcsLeafletMapOptions
   */
  enableMarkerCluster = false;
}
