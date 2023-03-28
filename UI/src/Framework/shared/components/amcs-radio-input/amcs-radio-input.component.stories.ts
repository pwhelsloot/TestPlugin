import { FormControl, FormGroup, Validators } from '@angular/forms';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateFormControlWrapper, generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { componentWrapperDecorator, Meta, Story } from '@storybook/angular';
import { AmcsRadioInputComponent } from './amcs-radio-input.component';

export default {
  title: StorybookGroupTitles.FormControls + 'RadioInput',
  component: AmcsRadioInputComponent,
  args: {
    label: 'A radio input',
  },
  decorators: [
    componentWrapperDecorator((story) => `<form [formGroup]="form" radioControl>${generateFormControlWrapper(story)}</form>`),
    generateModuleMetaDataForStorybook(AmcsRadioInputComponent, []),
  ],
} as Meta<AmcsRadioInputComponent>;

const Template: Story<AmcsRadioInputComponent> = (args) => {
  const formGroup = new FormGroup({
    radio: new FormControl(undefined, Validators.required),
  });
  return {
    props: {
      form: formGroup,
      formControlName: 'radio',
      label: args.label,
      value: args.value,
      isSecondaryColor: args.isSecondaryColor,
      isBlueRadio: args.isBlueRadio,
      extraOptions: args.extraOptions,
      bigRadio: args.bigRadio,
      noMargin: args.noMargin,
      hasError: args.hasError,
      isDisabled: args.isDisabled,
    },
  };
};

export const Primary = Template.bind({});
Primary.args = { ...Primary.args };
