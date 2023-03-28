import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateFormControlWrapperDecorator, generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { AmcsQuantitySelectorComponent } from './amcs-quantity-selector.component';

export default {
  title: StorybookGroupTitles.FormControls + 'QuantitySelector',
  component: AmcsQuantitySelectorComponent,parameters: {
    // Storybook Issue https://github.com/storybookjs/storybook/issues/16865
    // Storybook6.4 is now overriding all public properties + methods and assigning default values so they dont work.
    controls: {
      exclude: ['onChangeCallback','onTouchedCallback'],
    }},
  decorators: [generateFormControlWrapperDecorator(), generateModuleMetaDataForStorybook(AmcsQuantitySelectorComponent, [])]
} as Meta<AmcsQuantitySelectorComponent>;

const Template: Story<AmcsQuantitySelectorComponent> = (args: AmcsQuantitySelectorComponent) => ({
  component: AmcsQuantitySelectorComponent,
  props: { ...args }
});

export const Primary = Template.bind({});
Primary.args = { ...Primary.args, label: 'I am a quantity select label', minValue: 0, maxValue: 10 };
