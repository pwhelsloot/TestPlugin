import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateFormControlWrapper, generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { componentWrapperDecorator, Meta, Story } from '@storybook/angular';
import { AmcsInputComponent } from './amcs-input.component';

export default {
  title: StorybookGroupTitles.FormControls + 'Input',
  component: AmcsInputComponent,
  args: {
    // very odd we need this, it's referenced on the amcs-select html but no idea why we need it as args, only happens as using a template
    FormControlDisplay,
    label: 'Am alphabetical input',
  },
  parameters: {
    design: {
      disabled: true,
    },
    docs: {
      description: {
        component: 'I am an amcs input. I should only be used for **alphabetical** input, not for monetary input!',
      },
      source: {
        type: 'code',
      },
    },
  },
  argTypes: {
    displayMode: {
      options: ['standard', 'small', 'grid'],
      mapping: {
        // 'mapping' maps values to options
        standard: FormControlDisplay.Standard,
        small: FormControlDisplay.Small,
        grid: FormControlDisplay.Grid,
      },
      control: {
        type: 'select',
        labels: {
          // 'labels' maps option values to string labels
          standard: 'Standard',
          small: 'Small',
          grid: 'Grid',
        },
      },
    },
    // Hide these from control options
    autocomplete: {
      table: {
        disable: true,
      },
    },
    min: {
      table: {
        disable: true,
      },
    },
    step: {
      table: {
        disable: true,
      },
    },
    autoFocus: {
      table: {
        disable: true,
      },
    },
  },
  decorators: [
    componentWrapperDecorator((story) => `<form [formGroup]="form">${generateFormControlWrapper(story)}</form>`),
    generateModuleMetaDataForStorybook(AmcsInputComponent, []),
  ],
} as Meta;

const Template: Story<AmcsInputComponent> = (args) => {
  const formGroup = new FormGroup({
    username: new FormControl(undefined, Validators.required),
  });
  return {
    props: {
      form: formGroup,
      formControlName: 'username',
      type: args.type,
      label: args.label,
      inputTooltip: args.inputTooltip,
      displayMode: args.displayMode,
      customClass: args.customClass,
      customWrapperClass: args.customWrapperClass,
      labelClass: args.labelClass,
      placeholder: args.placeholder,
      hasError: args.hasError,
      isDisabled: args.isDisabled,
      isReadOnly: args.isReadOnly,
      noPadding: args.noPadding,
      noMargin: args.noMargin,
      icon: args.icon,
      maxLength: args.maxLength,
      precision: args.precision,
      enableClear: args.enableClear,
    },
  };
};

export const Primary = Template.bind({});
Primary.args = { ...Primary.args };

export const Secondary = Template.bind({});
Secondary.args = { ...Secondary.args, hasError: true };
