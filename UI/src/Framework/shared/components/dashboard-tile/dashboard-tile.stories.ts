import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { DashboardTileComponent } from './dashboard-tile.component';

export default {
  title: StorybookGroupTitles.Tiles + 'Dashboard Tile',
  component: DashboardTileComponent,
  parameters: {
    design: {
      disabled: true
    },
    docs: {
      description: {
        component: 'I am AMCS Dashboard Tile. I have bigger fonts/spacing'
      },
      source: {
        type: 'code'
      }
    }
  },
  decorators: [generateModuleMetaDataForStorybook(DashboardTileComponent, [SharedTranslationsService])]
} as Meta;

const WithButtonsTemplate: Story<DashboardTileComponent> = (args: DashboardTileComponent) => {
  const deexpander = { action: () => {} };
  const expander = { action: () => {} };
  const images = { action: () => {} };
  const buttons = [
    { caption: 'Standard Button', action: () => {} },
    {
      caption: 'Link Button',
      action: () => {},
      icon: 'fa-angle-right',
      link: true
    }
  ];
  return {
    props: { ...args, deexpander, expander, images, buttons },
    template: `<app-dashboard-tile  [expander]="expander" [deexpander]="deexpander"
    [buttons]="buttons" [images]="images" caption="Dashboard Tile - With Buttons">
    Standard and Link buttons are custom in this instance, expander and deexpander are built-in and can be removed/listened to
  </app-dashboard-tile>`
  };
};

const BasicTemplate: Story<DashboardTileComponent> = (args: DashboardTileComponent) => {
  const isDynamicHeight = true;
  return {
    props: { ...args, isDynamicHeight },
    template: `<app-dashboard-tile [isDynamicHeight]="isDynamicHeight"  caption="Dashboard Tile - Basic, with Dynamic Height">
   Any HTML can go in here!!
  </app-dashboard-tile>`
  };
};

export const Basic = BasicTemplate.bind({});
Basic.args = { ...Basic.args };

export const WithButtons = WithButtonsTemplate.bind({});
WithButtons.args = { ...WithButtons.args };
