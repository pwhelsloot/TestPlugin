import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { AmcsModalStorybookLoaderComponent } from './amcs-modal-storybook-loader.component';
import { ModalStorybookLoaderSize, ModalStorybookLoaderType } from './amcs-modal-storybook.enum';

export default {
  title: StorybookGroupTitles.Modals + 'Modal',
  component: AmcsModalStorybookLoaderComponent,
  decorators: [generateModuleMetaDataForStorybook(AmcsModalStorybookLoaderComponent, [])],
  args: { modalSize: ModalStorybookLoaderSize.Standard, title: 'Amcs Modal' },
  argTypes: {
    // Hide these from control options
    modalType: {
      table: {
        disable: true
      }
    }
  },
  parameters: {
    docs: {
      description: {
        component: 'I am the default Amcs Modal. I can hide my buttons, hide close [x] button, change color to different base colors, change to mobile version using simple controls below.  '
      }
    }
  }
} as Meta<AmcsModalStorybookLoaderComponent>;

const Template: Story<AmcsModalStorybookLoaderComponent> = (args) => ({
  props: args
});

export const Alert = Template.bind({});
Alert.args = { ...Alert.args, modalType: ModalStorybookLoaderType.Alert };

export const Confirmation = Template.bind({});
Confirmation.args = { ...Confirmation.args, modalType: ModalStorybookLoaderType.Confirmation };
