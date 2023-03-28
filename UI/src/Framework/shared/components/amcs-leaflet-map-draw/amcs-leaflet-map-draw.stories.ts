/* eslint-disable max-classes-per-file */
import { APP_INITIALIZER } from '@angular/core';
import { TestLayers } from '@storybook-util/map/amcs-leaflet-map-draw-storybook-test-data';
import { MockConfigStore } from '@storybook-util/map/mock-config-store';
import { MockSharedTranslationsService } from '@storybook-util/map/mock-shared-translations.service';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { AmcsLeafletMapService } from '../../../core/services/amcs-leaflet-map.service';
import { ApplicationConfigurationStore } from '../../../core/services/config/application-configuration.store';
import { AmcsMapProvider } from '../../amcs-leaflet-map/enums/amcs-map-provider.enum';
import { AmcsLeafletMapShapeEnum } from '../../amcs-leaflet-map/models/amcs-leaflet-map-shape.enum';
import { SharedTranslationsService } from '../../services/shared-translations.service';
import { AmcsLeafletMapDrawComponent } from './amcs-leaflet-map-draw.component';
import { AmcsLeafletMapDrawService } from './amcs-leaflet-map-draw.service';

function initMap(mapService: AmcsLeafletMapService) {
  return () => {
    mapService.setMapProvider(AmcsMapProvider.HereMapsStreet);
  };
}

export default {
  title: StorybookGroupTitles.Data + 'Map draw',
  component: AmcsLeafletMapDrawComponent,
  parameters: {
    design: {
      disabled: true
    },
    docs: {
      description: {
        component: 'I am a map which has shapes'
      },
      source: {
        type: 'code'
      }
    }
  },
  decorators: [
    generateModuleMetaDataForStorybook(AmcsLeafletMapDrawComponent, [
      { provide: SharedTranslationsService, useClass: MockSharedTranslationsService },
      AmcsLeafletMapDrawService,
      AmcsLeafletMapService,
      { provide: ApplicationConfigurationStore, useClass: MockConfigStore },
      {
        provide: APP_INITIALIZER,
        useFactory: initMap,
        multi: true,
        deps: [AmcsLeafletMapService, ApplicationConfigurationStore]
      }
    ])
  ]
} as Meta<AmcsLeafletMapDrawComponent>;

const Template: Story<AmcsLeafletMapDrawComponent> = (args: AmcsLeafletMapDrawComponent) => ({
  component: AmcsLeafletMapDrawComponent,
  props: args,
  template: `
  <app-amcs-leaflet-map-draw
    [enableMarkerCluster]="enableMarkerCluster"
    [enableDrawSelection]="enableDrawSelection"
    [latitude]="latitude"
    [longitude]="longitude"
    [layers]="layers"
    [zoom]="zoom"
    [drawIntersectErrorText]="'Cannot overlap'" class="flex-box-map">
  </app-amcs-leaflet-map-draw>`
});

export const Primary = Template.bind({});
Primary.args = {
  layers: [TestLayers()],
  mapHeight: 800,
  drawIntersectErrorText: '',
  longitude: -8.5751661,
  latitude: 52.6500178,
  zoom: 10,
  businessTypeId: 0,
  businessTypes: undefined,
  enableDrawSelection: true,
  enableMarkerCluster: true,
  shapesSupported: [AmcsLeafletMapShapeEnum.polygon],
  customClass: null,
  mapRedrawSubject: undefined
};
