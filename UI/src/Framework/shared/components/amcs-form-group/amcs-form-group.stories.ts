import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { componentWrapperDecorator, Meta, Story } from '@storybook/angular';
import { AmcsFormGroupComponent } from './amcs-form-group.component';

export default {
  title: StorybookGroupTitles.FormControls + 'FormGroup',
  component: AmcsFormGroupComponent,
  decorators: [
    componentWrapperDecorator(
      (story) =>
        `<div class="row"><div class="col-lg-3"><div class="portlet box grey child-portlet bordered"><div class="portlet-body">
          <app-amcs-form-group><app-amcs-numerical-input [precision]="2"></app-amcs-numerical-input></app-amcs-form-group>
        </div></div></div></div>`
    ),
    generateModuleMetaDataForStorybook(AmcsFormGroupComponent, []),
  ],
} as Meta<AmcsFormGroupComponent>;

const Template: Story<AmcsFormGroupComponent> = (args: AmcsFormGroupComponent) => {
  return {
    props: {
      hasError: args.hasError,
      hasSuccess: args.hasSuccess,
      inline: args.inline,
      hasActions: args.hasActions,
    },
  };
};

export const Primary = Template.bind({});
Primary.args = { ...Primary.args };
