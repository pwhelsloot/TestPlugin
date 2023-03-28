import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateFormControlWrapperDecorator, generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { AmcsFormLabelComponent } from './amcs-form-label.component';


export default {
  title: StorybookGroupTitles.FormControls + 'Label',
  component: AmcsFormLabelComponent,
  decorators: [generateFormControlWrapperDecorator(), generateModuleMetaDataForStorybook(AmcsFormLabelComponent, [])],
} as Meta<AmcsFormLabelComponent>;

const Template: Story<AmcsFormLabelComponent> = (args: AmcsFormLabelComponent) => ({
  component: AmcsFormLabelComponent,
  props: args,
});

export const Primary = Template.bind({});
Primary.args = { ...Primary.args, label:'I am a standalone Label, you can use me in a custom Form control' };
