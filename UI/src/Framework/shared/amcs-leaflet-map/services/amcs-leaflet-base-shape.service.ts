import { Injectable } from '@angular/core';
import { Layer, Map, MarkerClusterGroup } from 'leaflet';
import { isTruthy } from '../../../core/helpers/is-truthy.function';
import { isClusterGroupLayer } from '../amcs-leaflet-layer.helper';
import { AmcsLeafletMapGrouping } from '../models/amcs-leaflet-map-grouping.model';

@Injectable()
export abstract class AmcsLeafletBaseShapeService {
  protected map: Map;
  protected cachedLayers = new Array<Layer>();
  protected defaultClusterGroup: MarkerClusterGroup;
  protected clusterGroups = new Array<MarkerClusterGroup>();
  protected clustersEnabled = false;

  abstract generateLayersFromGroup(group: AmcsLeafletMapGrouping): void;
  abstract removeShapesFromMap(group: AmcsLeafletMapGrouping): void;

  /**
   * Initialize Factory with a given Map reference
   *
   * @param {Map} map
   * @memberof AmcsLeafletShapeService
   */
  initialize(map: Map, clusterGroup?: MarkerClusterGroup) {
    this.map = map;
    this.defaultClusterGroup = clusterGroup;
    if (isTruthy(clusterGroup)) {
      this.clustersEnabled = true;
      this.defaultClusterGroup.addTo(this.map);
    } else {
      this.clustersEnabled = false;
    }
  }

  /**
   * Add layer to map and layer cache
   *
   * @param {Layer} layer
   * @memberof AmcsLeafletShapeService
   */
  addLayerToMapAndCache(layer: Layer, clusterGroup?: MarkerClusterGroup) {
    this.addLayerToMap(layer, clusterGroup);
    this.cachedLayers.push(layer);
  }

  /**
   * Get all Cached Layers
   *
   * @returns
   * @memberof AmcsLeafletShapeService
   */
  getLayers() {
    return this.cachedLayers;
  }

  getClusterGroup() {
    return this.defaultClusterGroup;
  }

  /**
   * Empties cached Layers
   *
   * @memberof AmcsLeafletShapeService
   */
  resetLayers() {
    this.cachedLayers = new Array<Layer>();
    if (this.clustersEnabled) {
      this.defaultClusterGroup.clearLayers();
      this.clusterGroups.forEach((group) => group.clearLayers());
    }
  }

  removeLayerFromCluster(layer: Layer, clusterGroup?: MarkerClusterGroup) {
    if (this.clustersEnabled) {
      if (clusterGroup) {
        clusterGroup.removeLayer(layer);
        this.clusterGroups.every(group => {
          if(group['_leaflet_id'] === clusterGroup['_leaflet_id']) {
            group = clusterGroup;
            return false;
          }
          return true;
        });
      } else {
        this.defaultClusterGroup.removeLayer(layer);
      }
      this.map.addLayer(layer);
    }
  }

  addLayerToCluster(layer: Layer, clusterGroup?: MarkerClusterGroup) {
    if (this.clustersEnabled) {
      this.map.removeLayer(layer);
      if (clusterGroup) {
        clusterGroup.addLayer(layer);
        this.clusterGroups.every(group => {
          if(group['_leaflet_id'] === clusterGroup['_leaflet_id']) {
            group = clusterGroup;
            return false;
          }
          return true;
        });
      } else {
        this.defaultClusterGroup.addLayer(layer);
      }
    }
  }

  /**
   * Add layer to map
   *
   * @param {Layer} layer
   * @memberof AmcsLeafletShapeService
   */
  protected addLayerToMap(layer: Layer, clusterGroup?: MarkerClusterGroup) {
    if (this.clustersEnabled) {
      if (clusterGroup) {
        const existingCluster = this.clusterGroups.find((cluster) => cluster['_leaflet_id'] === clusterGroup['_leaflet_id']);
        if (existingCluster) {
          existingCluster.addLayer(layer);
        } else {
          clusterGroup.addLayer(layer);
          clusterGroup.addTo(this.map);
          this.clusterGroups.push(clusterGroup);
        }
      } else {
        this.defaultClusterGroup.addLayer(layer);
      }
    } else {
      this.map.addLayer(layer);
    }
  }

  protected removeFromCacheAndMapByIds(ids: number[]) {
    const layers = this.cachedLayers.filter((layer) => ids.some((id) => id === layer['_leaflet_id']));
    this.removeFromCacheAndMap(layers);
  }

  protected refreshClusterLayer() {
    if (this.clustersEnabled) {
      if (isTruthy(this.defaultClusterGroup)) {
        this.defaultClusterGroup.removeFrom(this.map);
      }
      this.clusterGroups.forEach((group) => group.removeFrom(this.map));

      this.defaultClusterGroup.addTo(this.map);
      this.clusterGroups.forEach((group) => group.addTo(this.map));
    }
  }

  private removeFromCacheAndMap(layers: Layer[]) {
    if (this.clustersEnabled) {
      this.defaultClusterGroup.removeLayers(layers);
      this.clusterGroups.forEach((group) => {
        group.removeLayers(layers);
        if (group.getLayers().length === 0) {
          group.removeFrom(this.map);
        }
      });
      this.clusterGroups = this.clusterGroups.filter((group) => group.getLayers().length !== 0);
    }
    layers.forEach((layer) => {
      const index = this.cachedLayers.findIndex((d) => d === layer);
      this.cachedLayers.splice(index, 1);
      if (!isClusterGroupLayer(layer)) {
        this.map.removeLayer(layer);
      }
    });
  }
}
