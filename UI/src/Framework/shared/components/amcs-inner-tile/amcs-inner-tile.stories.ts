import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { AmcsInnerTileComponent } from './amcs-inner-tile.component';

export default {
  title: StorybookGroupTitles.Tiles + 'Inner Tile',
  component: AmcsInnerTileComponent,
  parameters: {
    design: {
      disabled: true
    },
    docs: {
      description: {
        component: 'I am AMCS Inner Tile. I am our standard grey inner tile with smaller fonts/spacing and usually sit inside a parent tile'
      },
      source: {
        type: 'code'
      }
    }
  },
  decorators: [generateModuleMetaDataForStorybook(AmcsInnerTileComponent, [SharedTranslationsService])]
} as Meta;

const Template: Story<AmcsInnerTileComponent> = (args: AmcsInnerTileComponent) => {
  return {
    props: args,
    template: `<app-amcs-inner-tile  caption="Inner Tile">
    <h4>Any HTML can go in here!!</h4>
  </app-amcs-inner-tile>`
  };
};

const WrappedTemplate: Story<AmcsInnerTileComponent> = (args: AmcsInnerTileComponent) => {
  return {
    props: args,
    template: `<app-dashboard-tile [isDynamicHeight]="true" caption="Dashboard Tile With Inner Tile">
    <app-amcs-inner-tile [isDynamicHeight]="true" caption="Inner Tile">
    <h4>Tile within a tile...TILECEPTION!</h4>
    </app-amcs-inner-tile>
  </app-dashboard-tile>`
  };
};

export const Primary = Template.bind({});
Primary.args = { ...Primary.args };

export const Wrapped = WrappedTemplate.bind({});
Wrapped.args = { ...Wrapped.args };
