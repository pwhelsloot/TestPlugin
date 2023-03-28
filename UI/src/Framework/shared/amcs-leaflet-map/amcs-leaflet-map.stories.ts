import { APP_INITIALIZER } from '@angular/core';
import { MockConfigStore } from '@storybook-util/map/mock-config-store';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { AmcsLeafletMapService } from '../../core/services/amcs-leaflet-map.service';
import { ApplicationConfigurationStore } from '../../core/services/config/application-configuration.store';
import { AmcsLeafletMapComponent } from './amcs-leaflet-map.component';
import { AmcsMapProvider } from './enums/amcs-map-provider.enum';


function initMap(mapService: AmcsLeafletMapService) {
  return () => {
    mapService.setMapProvider(AmcsMapProvider.HereMapsStreet);
  };
}
export default {
  title: StorybookGroupTitles.Data + 'Map',
  component: AmcsLeafletMapComponent,
  parameters: {
    design: {
      disabled: true,
    },
    docs: {
      description: {
        component: 'I am a map',
      },
      source: {
        type: 'code',
      },
    },
  },
  decorators: [
    generateModuleMetaDataForStorybook(AmcsLeafletMapComponent, [
      AmcsLeafletMapService,
      { provide: ApplicationConfigurationStore, useClass: MockConfigStore },
      {
        provide: APP_INITIALIZER,
        useFactory: initMap,
        multi: true,
        deps: [AmcsLeafletMapService, ApplicationConfigurationStore],
      },
    ]),
  ],
} as Meta<AmcsLeafletMapComponent>;

const Template: Story<AmcsLeafletMapComponent> = (args: AmcsLeafletMapComponent) => ({
  component: AmcsLeafletMapComponent,
  props: args,
  template: `<app-amcs-leaflet-map
  [zoom]="zoom"
  [mapHeight]="mapHeight"
  [latitude]="latitude"
  [longitude]="longitude"
  [expandedMode]="expandedMode">
</app-amcs-leaflet-map>`,
});

export const Primary = Template.bind({});
Primary.args = {
  longitude: -8.5751661,
  latitude: 52.6500178,
  zoom: 10,
  mapHeight: 800,
  mapHeightUnit: 'px',
  expandedMode: false,
  customClass: null,
  enableMarkerCluster: false,
};
