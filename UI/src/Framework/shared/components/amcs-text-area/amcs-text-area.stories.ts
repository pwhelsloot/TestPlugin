import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateFormControlWrapperDecorator, generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { AmcsTextAreaComponent } from './amcs-text-area.component';

export default {
  title: StorybookGroupTitles.FormControls + 'TextArea',
  component: AmcsTextAreaComponent,
  args: {
    label: 'I\'m a TextArea Label',
  },
  decorators: [generateFormControlWrapperDecorator(), generateModuleMetaDataForStorybook(AmcsTextAreaComponent, [])],
} as Meta<AmcsTextAreaComponent>;

const Template: Story<AmcsTextAreaComponent> = (args: AmcsTextAreaComponent) => {
  return {
    props: {
      label: args.label,
      inputTooltip: args.inputTooltip,
      displayMode: args.displayMode,
      customClass: args.customClass,
      hasError: args.hasError,
      isDisabled: args.isDisabled,
      maxLength: args.maxLength,
    },
  };
};

export const Primary = Template.bind({});
Primary.args = { ...Primary.args };
