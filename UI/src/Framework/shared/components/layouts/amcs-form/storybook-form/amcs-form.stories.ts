import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { StorybookAmcsFormComponent } from './storybook-amcs-form.component';

export default {
  title: StorybookGroupTitles.Form + 'Forms',
  component: StorybookAmcsFormComponent,
  args: {
    featureTitle: 'Example',
    editorTitle: 'Restaurant Booking',
    enableReturn: false,
    enableLog: false,
    enableSave: true,
    enableContinue: false,
    enableBack: false,
    enableCancel: true,
    enableDelete: false
  },
  parameters: {
    controls: {
      exclude: ['onSave'],
    },
    design: {
      disabled: true
    },
    docs: {
      description: {
        component: 'I am the default AMCS Form Component. I can be put inside different kinds of wrappers, or combined with other AMCS Forms into a group.'
      },
    }
  },
  argTypes: {
    showFormTile: {
      table: {
        disable: true
      }
    }
  },
  decorators: [generateModuleMetaDataForStorybook(StorybookAmcsFormComponent, [])]
} as Meta;

const Template: Story<StorybookAmcsFormComponent> = (args) => ({
  props: {
    showFormTile:args.showFormTile,
    featureTitle: args.featureTitle,
    editorTitle: args.editorTitle,
    enableReturn: args.enableReturn,
    enableLog: args.enableLog,
    enableSave: args.enableSave,
    enableContinue: args.enableContinue,
    enableBack: args.enableBack,
    enableCancel: args.enableCancel,
    enableDelete: args.enableDelete
  }
});
export const Form = Template.bind({});
Form.args = { ...Form.args, showFormTile: false };
Form.argTypes = {
  featureTitle: {
    table: {
      disable: true
    }
  },
  editorTitle: {
    table: {
      disable: true
    }
  },
  enableReturn: {
    table: {
      disable: true
    }
  },
  enableLog: {
    table: {
      disable: true
    }
  },
  enableSave: {
    table: {
      disable: true
    }
  },
  enableContinue: {
    table: {
      disable: true
    }
  },
  enableBack: {
    table: {
      disable: true
    }
  },
  enableCancel: {
    table: {
      disable: true
    }
  },
  enableDelete: {
    table: {
      disable: true
    }
  }
};

export const FormTile = Template.bind({});
FormTile.args = { ...FormTile.args, showFormTile: true };
FormTile.parameters = {
  docs: {
    description: {
      component: 'I am the default AMCS Form Tile Component. I can be used standalone as I have my own tile with title, and buttons.'
    },
  }
};
