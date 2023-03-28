import { ButtonDisplayModeEnum } from '@shared-module/components/amcs-button/button-display-mode.enum';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { AmcsButtonComponent } from './amcs-button.component';

export default {
  title: StorybookGroupTitles.Form + 'Button',
  component: AmcsButtonComponent,
  decorators: [generateModuleMetaDataForStorybook(AmcsButtonComponent, [])],
  args: {
    displayMode: ButtonDisplayModeEnum.Standard,
    customClass: 'noMargin amcs-green',
  },
  parameters: {
    design: {
      disabled: true,
    },
    docs: {
      description: {
        component: 'I am the default amcs Button.',
      },
      source: {
        type: 'dynamic',
      },
    },
  },
  argTypes: {
    displayMode: {
      options: ['standard', 'important'],
      mapping: {
        standard: ButtonDisplayModeEnum.Standard,
        important: ButtonDisplayModeEnum.Important,
      },
      control: {
        type: 'select',
        labels: {
          standard: 'Standard',
          important: 'Important',
        },
      },
    },
    // Hide these from control options
    type: {
      table: {
        disable: true,
      },
    },
  },
  controls: {
    exclude: ['apiFilterChange']
  }

} as Meta;

function clickHandler(_event) {
  console.log('clicked');
}
const Template: Story<AmcsButtonComponent> = (args) => ({
  props: {...args,clickHandler},
  template: `<app-amcs-button
  [buttonTooltip]="buttonTooltip"
  [customClass]="customClass"
  [disabled]="disabled"
  [loading]="loading"
  [minWidth]="minWidth"
  [displayMode]="displayMode"
  (click)="clickHandler($event)"
  ><span>Basic Button</span></app-amcs-button>`,
});

export const Primary = Template.bind({});
Primary.args = { ...Primary.args };

export const Secondary = Template.bind({});
Secondary.args = { ...Secondary.args, disabled: true, customClass: 'btn-default' };
