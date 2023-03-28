import { alias } from '@coreconfig/api-dto-alias.function';

export class MapConfiguration {

    @alias('GoogleMapsUrl')
    googleMapsUrl: string;

    @alias('GoogleMapsSatelliteUrl')
    googleMapsSatelliteUrl: string;

    @alias('GoogleMapsTerrainUrl')
    googleMapsTerrainUrl: string;

    @alias('HereMapsUrl')
    hereMapsUrl: string;

    @alias('HereMapsTerrainUrl')
    hereMapsTerrainUrl: string;

    @alias('HereMapsSatelliteUrl')
    hereMapsSatelliteUrl: string;

    @alias('MapDisplayProvider')
    mapDisplayProvider: string;
}
