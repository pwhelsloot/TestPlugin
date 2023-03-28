import { FormModelSizeEnum } from '@shared-module/models/form-modal-size.enum';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { AmcsFormModalStorybookLoaderComponent } from './amcs-form-modal-storybook/amcs-form-modal-storybook-loader.component';

export default {
  title: StorybookGroupTitles.Modals + 'Form Modal',
  component: AmcsFormModalStorybookLoaderComponent,
  decorators: [generateModuleMetaDataForStorybook(AmcsFormModalStorybookLoaderComponent, [])],
  args: { title: 'Amcs Form Modal', isExpandable: true },
  argTypes: {
    // Hide these from control options
    formModalSize: {
      table: {
        disable: true
      }
    },
    formModalString: {
      table: {
        disable: true
      }
    }
  },
  buttonTitle: {
    table: {
      disable: true
    }
  },
  parameters: {
    docs: {
      description: {
        component:
          'I am the default Amcs Form Modal. I can expand to fullscreen using the expand button, which can be shown using isExpandable bool.'
      }
    },
    controls: {
      exclude: ['triggerModalSubject']
    }
  }
} as Meta<AmcsFormModalStorybookLoaderComponent>;

const Template: Story<AmcsFormModalStorybookLoaderComponent> = (args) => ({
  props: args
});

export const Standard = Template.bind({});
Standard.args = { ...Standard.args, formModalSize: FormModelSizeEnum.Standard };

export const Double = Template.bind({});
Double.args = { ...Double.args, formModalSize: FormModelSizeEnum.Double };
