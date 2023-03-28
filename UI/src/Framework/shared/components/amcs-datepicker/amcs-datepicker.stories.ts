import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateFormControlWrapper, generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { componentWrapperDecorator, Meta, Story } from '@storybook/angular';
import { AmcsDatepickerComponent } from './amcs-datepicker.component';

export default {
  title: StorybookGroupTitles.FormControls + 'Datepicker',
  component: AmcsDatepickerComponent,
  args: {
    label: 'My Datepicker Label',
    displayMode: FormControlDisplay.Standard,
    errors: null,
  },
  parameters: {
    design: {
      disabled: true,
    },
    docs: {
      description: {
        component: 'I am an amcs datepicker. One day I\'ll get replaced!',
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
    config: {
      table: {
        disable: true,
      },
    },
    previewChanges: {
      table: {
        disable: true,
      },
    },
    hasFieldDefs: {
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
    generateModuleMetaDataForStorybook(AmcsDatepickerComponent),
  ],
} as Meta;

const Template: Story<AmcsDatepickerComponent> = (args) => {
  const formGroup = new FormGroup({
    datepick: new FormControl(undefined, Validators.required),
  });

  return {
    props: {
      form: formGroup,
      formControlName: 'datepick',
      label: args.label,
      displayMode: args.displayMode,
      customClass: args.customClass,
      placement: args.placement,
      hasError: args.hasError,
      isSecondaryColor: args.isSecondaryColor,
      dateTooltip: args.dateTooltip,
      disabled: args.disabled,
      readonly: args.readonly,
      errors: args.errors,
    },
  };
};

export const Primary = Template.bind({});
Primary.args = { ...Primary.args };

export const Secondary = Template.bind({});
Secondary.args = { ...Secondary.args, hasError: true };
