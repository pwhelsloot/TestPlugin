import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { AmcsCustomTileComponent } from './amcs-custom-tile.component';

export default {
  title: StorybookGroupTitles.Tiles + 'Custom Tile',
  component: AmcsCustomTileComponent,
  parameters: {
    design: {
      disabled: true
    },
    docs: {
      description: {
        component: 'I am AMCS Custom Tile. I host any HTML in my title and body. You can supply custom html for a more bespoke look.'
      },
      source: {
        type: 'code'
      }
    }
  },
  decorators: [generateModuleMetaDataForStorybook(AmcsCustomTileComponent, [SharedTranslationsService])]
} as Meta;

const Template: Story<AmcsCustomTileComponent> = (args: AmcsCustomTileComponent) => {
  return {
    props: args,
    template: ` <app-amcs-custom-tile>
    <ng-container ngProjectAs="title">
      <div>
        Custom tile - We can put any html we like in here 
      </div>
    </ng-container>
    <ng-container ngProjectAs="body">
      <p>
        When using Dashboard and browser tiles they'll only let us put custom html into the portlet body, a custom
        tile allows us to
        customise both the header and the body.
      </p>
    </ng-container>
  </app-amcs-custom-tile>`
  };
};

// Basic example
export const Primary = Template.bind({});
Primary.args = { ...Primary.args };
