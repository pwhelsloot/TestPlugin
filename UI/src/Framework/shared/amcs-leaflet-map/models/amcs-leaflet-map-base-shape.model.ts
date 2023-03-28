export abstract class AmcsLeafletMapBaseShape {
  id: number;
  leafletLayerId: number;
  abstract toLayer();
}
