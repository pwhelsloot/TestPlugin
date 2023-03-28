import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateFormControlWrapperDecorator, generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { AmcsTimepickerComponent } from './amcs-timepicker.component';

export default {
  title: StorybookGroupTitles.FormControls + 'Timepicker',
  component: AmcsTimepickerComponent,
  args: {
    label: 'Timepicker',
    hasError: false,
    displayMode: FormControlDisplay.Standard,
    timeToolTip: 'Input a time',
    disabled: false,
    interval: 1,
    displayseconds: false,
  },
  parameters: {
    design: {
      disabled: true,
    },
    docs: {
      description: {
        component: 'I am a timepicker component for selecting a time of the day.',
      },
      source: {
        type: 'dynamic',
      },
    },
  },
  argTypes: {
    displayMode: {
      options: ['standard', 'small', 'grid'],
      mapping: {
        standard: FormControlDisplay.Standard,
        small: FormControlDisplay.Small,
        grid: FormControlDisplay.Grid,
      },
      control: {
        type: 'select',
        labels: {
          standard: 'Standard',
          small: 'Small',
          grid: 'Grid',
        },
      },
    },
    config: {
      table: {
        disable: true,
      },
    },
  },
  decorators: [
    generateFormControlWrapperDecorator(),
    generateModuleMetaDataForStorybook(AmcsTimepickerComponent)
  ],
} as Meta;

const Template: Story<AmcsTimepickerComponent> = (args) => ({
  props: {
    config: args.config,
    label: args.label,
    hasError: args.hasError,
    displayMode: args.displayMode,
    timeToolTip: args.timeTooltip,
    disabled: args.disabled,
    interval: args.interval,
    displaySeconds: args.displayseconds,
  },
});
export const Primary = Template.bind({});
Primary.args = { ...Primary.args, displayMode: 'standard', label: 'I\'m a label for a timepicker' };

export const Secondary = Template.bind({});
Secondary.args = { ...Secondary.args, displayseconds: true, displayMode: 'small', hasError: true };
Secondary.parameters = {
  docs: {
    description: {
      component: 'I am a timepicker with seconds displayed',
    },
    source: {
      type: 'dynamic',
    },
  },
};
