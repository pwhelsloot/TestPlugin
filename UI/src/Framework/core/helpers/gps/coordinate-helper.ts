export class GpsCoordinateHelper {

  static IsValidCoordinate(latitude: number, longitude: number): boolean {
    if (!latitude || !longitude) {
      // Coordinate doesn not contain latitude or longitude data
      return false;
    } else if (latitude === 0 && longitude === 0) {
      // Both equal zero normally indicates invalid location
      return false;
    } else if (latitude < -90 || latitude > 90) {
      // Latitude value outside acceptable range
      return false;
    } else if (longitude < -180 || longitude > 180) {
      // Longitude outside acceptable range
      return false;
    } else {
      return true;
    }
  }

  // Many latlong properties are in string format so they transfer correctly between UI and API
  static IsValidCoordinateFromStrings(latitude: string, longitude: string): boolean {
    return this.IsValidCoordinate(parseFloat(latitude), parseFloat(longitude));
  }
}
