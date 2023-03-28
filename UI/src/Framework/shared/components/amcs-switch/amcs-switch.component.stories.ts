import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateFormControlWrapperDecorator, generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { AmcsSwitchComponent } from './amcs-switch.component';

export default {
  title: StorybookGroupTitles.FormControls + 'Switch',
  component: AmcsSwitchComponent,
  decorators: [generateFormControlWrapperDecorator(), generateModuleMetaDataForStorybook(AmcsSwitchComponent, [])],
  args: {
    disabled: false,
    config: undefined,
  },
} as Meta<AmcsSwitchComponent>;

const Template: Story<AmcsSwitchComponent> = (args: AmcsSwitchComponent) => ({
  component: AmcsSwitchComponent,
  props: args,
});

export const Primary = Template.bind({});
Primary.args = { ...Primary.args };
