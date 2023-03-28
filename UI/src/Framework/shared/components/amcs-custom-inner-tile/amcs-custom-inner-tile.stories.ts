import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { AmcsCustomInnerTileComponent } from './amcs-custom-inner-tile.component';

export default {
  title: StorybookGroupTitles.Tiles + 'Custom Inner Tile',
  component: AmcsCustomInnerTileComponent,
  parameters: {
    design: {
      disabled: true
    },
    docs: {
      description: {
        component: 'I am AMCS Custom Inner Tile. I am our customisable grey inner tile. You can supply custom html for a more bespoke look.'
      },
      source: {
        type: 'code'
      }
    }
  },
  decorators: [generateModuleMetaDataForStorybook(AmcsCustomInnerTileComponent, [SharedTranslationsService])]
} as Meta;

const Template: Story<AmcsCustomInnerTileComponent> = (args: AmcsCustomInnerTileComponent) => {
  return {
    props: args,
    template: ` <app-amcs-custom-inner-tile>
    <ng-container ngProjectAs="title">
      <div>
        Custom tile - We can put any html we like in here 
      </div>
    </ng-container>
    <ng-container ngProjectAs="body">
      <p>
        When using inner tile it'll only let us put custom html into the portlet body, a custom inner
        tile allows us to customise both the header and the body.
      </p>
    </ng-container>
  </app-amcs-custom-inner-tile>`
  };
};

const WrappedTemplate: Story<AmcsCustomInnerTileComponent> = (args: AmcsCustomInnerTileComponent) => {
  return {
    props: args,
    template: ` <app-amcs-custom-tile>
    <ng-container ngProjectAs="title">
      <div>
        Custom tile - We can put any html we like in here 
      </div>
    </ng-container>
    <ng-container ngProjectAs="body">
    <app-amcs-custom-inner-tile>
    <ng-container ngProjectAs="title">
      <div>
        Custom Inner tile - We can put any html we like in here 
      </div>
    </ng-container>
    <ng-container ngProjectAs="body">
      <p>
        When using inner tile it'll only let us put custom html into the portlet body, a custom inner
        tile allows us to customise both the header and the body.
      </p>
    </ng-container>
  </app-amcs-custom-inner-tile>
    </ng-container>
  </app-amcs-custom-tile>`
  };
};

export const Basic = Template.bind({});
Basic.args = { ...Basic.args };

export const Wrapped = WrappedTemplate.bind({});
Wrapped.args = { ...Wrapped.args };
