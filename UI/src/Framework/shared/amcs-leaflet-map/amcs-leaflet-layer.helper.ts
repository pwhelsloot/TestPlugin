import { isTruthy } from '@core-module/helpers/is-truthy.function';

export function isClusterGroupLayer(layer) {
  return isTruthy(layer.options.disableClusteringAtZoom);
}
