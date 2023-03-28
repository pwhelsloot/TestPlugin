import { MarkerOptions } from 'leaflet';

export interface AmcsLeafletMarkerOptions extends MarkerOptions {
  delayDuration: number;
  infoTextFunction: Function;
  infoTextPayload: number;
  infoText: string;
}
