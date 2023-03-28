import { of } from 'rxjs';
import { ApplicationConfiguration } from '../../core/models/application-configuration.model';
import { MapConfiguration } from '../../core/models/map-configuration.model';
import { ApplicationConfigurationStore } from '../../core/services/config/application-configuration.store';

export class MockConfigStore implements Partial<ApplicationConfigurationStore> {
  configuration$ = of({
    mapProviderConfiguration: {
      googleMapsUrl: 'https://{s}.google.com/vt/lyrs=m&x={x}&y={y}&z={z}',
      googleMapsSatelliteUrl: 'https://{s}.google.com/vt/lyrs=s&x={x}&y={y}&z={z}',
      googleMapsTerrainUrl: 'https://{s}.google.com/vt/lyrs=p&x={x}&y={y}&z={z}',
      hereMapsUrl:
        'https://1.base.maps.cit.api.here.com/maptile/2.1/maptile/newest/normal.day/{z}/{x}/{y}/256/png8?app_id=PrT4jl18ziV9P3NB6yBs&app_code=WC-Vbk8rztbJsEeCd8u4mA',
      hereMapsSatelliteUrl:
        'https://1.aerial.maps.api.here.com/maptile/2.1/maptile/newest/satellite.day/{z}/{x}/{y}/256/png8?app_id=PrT4jl18ziV9P3NB6yBs&app_code=WC-Vbk8rztbJsEeCd8u4mA',
      hereMapsTerrainUrl:
        'https://1.aerial.maps.api.here.com/maptile/2.1/maptile/newest/terrain.day/{z}/{x}/{y}/256/png8?app_id=PrT4jl18ziV9P3NB6yBs&app_code=WC-Vbk8rztbJsEeCd8u4mA',
      mapDisplayProvider: 'google',
    } as MapConfiguration,
  } as ApplicationConfiguration);
}
