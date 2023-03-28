import { StorybookPaymentFormComponent } from '@storybook-util/forms/storybook-payment-form/storybook-payment-form.component';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';

export default {
  title: StorybookGroupTitles.Data + 'Payment Form', // Tile of story
  component: StorybookPaymentFormComponent, // Component to display
  args: {
    // All component inputs you want to set a default value for on the storybook doc tab
    featureTitle: 'Customer X',
    editorTitle: 'Payment',
    enableReturn: false,
    enableLog: false,
    enableSave: true,
    enableContinue: false,
    enableBack: false,
    enableCancel: true,
    enableDelete: false,
    fixCss: false,
  },
  parameters: {
    design: {
      disabled: true,
    },
    docs: {
      // Displays a description under the title in the docs tab
      description: {
        component: 'I am a complex payment form for customer X but I clearly have css issues.',
      },
      source: {
        type: 'dynamic', // Controls the 'view code / copy code' functionality on the docs tab
        // Can be set to 'auto' / 'code' or 'dynamic'. Seems a bit flakey and not clear on which one to choose!
      },
    },
  },
  argTypes: {
    fixCss: {
      // This hides inputs from the storybook doc tab
      table: {
        disable: true,
      },
    },
  },
  decorators: [generateModuleMetaDataForStorybook(StorybookPaymentFormComponent, [])],
} as Meta;

const Template: Story<StorybookPaymentFormComponent> = (args) => ({
  props: {
    // This maps the components inputs (args.someInput) to our storybook doc properties
    featureTitle: args.featureTitle,
    editorTitle: args.editorTitle,
    enableReturn: args.enableReturn,
    enableLog: args.enableLog,
    enableSave: args.enableSave,
    enableContinue: args.enableContinue,
    enableBack: args.enableBack,
    enableCancel: args.enableCancel,
    enableDelete: args.enableDelete,
    fixCss: args.fixCss,
  },
});
export const CSSIssues = Template.bind({}); // This will display one StorybookPaymentFormComponent
CSSIssues.args = { ...CSSIssues.args };

export const CSSFixed = Template.bind({}); // This will display a second StorybookPaymentFormComponent
CSSFixed.args = { ...CSSFixed.args, fixCss: true }; // Note how we change the property for the second item here
CSSFixed.parameters = {
  // If you want different sub-text to appear on second item then follow the below
  docs: {
    description: {
      component: 'I am a complex payment form for customer X with css issues fixed!.',
    },
    source: {
      type: 'dynamic',
    },
  },
};
