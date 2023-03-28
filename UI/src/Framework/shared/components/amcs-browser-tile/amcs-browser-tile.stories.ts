import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook, generateSharedTranslationsWrapper } from '@storybook-util/storybook-metadata.generator';
import { componentWrapperDecorator, Meta, Story } from '@storybook/angular';
import { BrowserOptions } from '../layouts/amcs-browser-grid-layout/browser-options-model';
import { AmcsBrowserTileComponent } from './amcs-browser-tile.component';

export default {
  title: StorybookGroupTitles.Tiles + 'Browser Tile',
  component: AmcsBrowserTileComponent,
  args: {
    loading: false,
    options: new BrowserOptions()
  },
  parameters: {
    design: {
      disabled: true
    },
    docs: {
      description: {
        component:
          'I am AMCS Browser Tile. I wrap any HTML content and make it look cool. I am our standard blue browser tile and include browser based buttons.'
      },
      source: {
        type: 'code'
      }
    }
  },
  decorators: [
    componentWrapperDecorator((story) => `${generateSharedTranslationsWrapper(story)}`),
    generateModuleMetaDataForStorybook(AmcsBrowserTileComponent, [SharedTranslationsService])
  ]
} as Meta;

const BasicTemplate: Story<AmcsBrowserTileComponent> = (args: AmcsBrowserTileComponent) => {
  args.options.title = 'AMCS Browser Tile - Basic';
  args.options.enableAdd = false;
  args.options.enableEdit = false;
  args.options.enableDelete = false;
  args.options.enableDeExpand = false;
  args.options.enableClose = false;
  return {
    props: { ...args, loading: args.loading, options: args.options },
    template: `<app-amcs-browser-tile [loading]="loading" [options]="options" >Any HTML can go here and look awesome!</app-amcs-browser-tile>`
  };
};

const WithButtonstTemplate: Story<AmcsBrowserTileComponent> = (args: AmcsBrowserTileComponent) => {
  args.options.title = 'AMCS Browser Tile - With Buttons';
  args.options.enableAdd = true;
  args.options.enableEdit = true;
  args.options.enableDelete = true;
  args.options.enableDeExpand = true;
  args.options.enableClose = true;
  return {
    props: { ...args, loading: args.loading, options: args.options },
    template: `<app-amcs-browser-tile [loading]="loading" [options]="options" ><span>These are all the built-in buttons, you can pick which ones to enable/disable, and listen to them easily!</span>
    <div>*You can't do this in SB on the fly unfortunately...</div></app-amcs-browser-tile>`
  };
};

export const Basic = BasicTemplate.bind({});
Basic.args = { ...Basic.args };

export const WithButtons = WithButtonstTemplate.bind({});
WithButtons.args = { ...WithButtons.args };
