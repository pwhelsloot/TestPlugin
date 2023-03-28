export class MapExampleContext {
  latitude: number;
  longitude: number;
  mapPointId: number;

  constructor(lat: number, long: number, id: number) {
    this.latitude = lat;
    this.longitude = long;
    this.mapPointId = id;
  }
}
