import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateFormControlWrapperDecorator, generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { AmcsNumericalInputComponent } from './amcs-numerical-input.component';

export default {
  title: StorybookGroupTitles.FormControls + 'Numerical',
  component: AmcsNumericalInputComponent,
  decorators: [generateFormControlWrapperDecorator(), generateModuleMetaDataForStorybook(AmcsNumericalInputComponent, [])],
  args: { decimalsDeliminator: '.' },
} as Meta<AmcsNumericalInputComponent>;

const Template: Story<AmcsNumericalInputComponent> = (args: AmcsNumericalInputComponent) => {
  return {
    props: {
      label: args.label,
      decimalsDeliminator: args.decimalsDeliminator,
      thousandsDeliminator: args.thousandsDeliminator,
      prefix: args.prefix,
      autoFocus: args.autoFocus,
      alignment: args.alignment,
      noPadding: args.noPadding,
      allowNegative: args.allowNegative,
      suffix: args.suffix,
      displayMode: args.displayMode,
      customClass: args.customClass,
      customWrapperClass: args.customWrapperClass,
      hasError: args.hasError,
      isDisabled: args.isDisabled,
      noMargin: args.noMargin,
      maxLength: args.maxLength,
      precision: args.precision,
    },
  };
};

export const Primary = Template.bind({});
Primary.args = { ...Primary.args, label: 'I am a numerical input label' };
