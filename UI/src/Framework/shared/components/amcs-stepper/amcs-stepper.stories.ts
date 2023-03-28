import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook, generateSharedTranslationsWrapper } from '@storybook-util/storybook-metadata.generator';
import { componentWrapperDecorator, Meta, Story } from '@storybook/angular';
import { AmcsStepperComponent } from './amcs-stepper.component';

export default {
  title: StorybookGroupTitles.Form + 'Stepper',
  component: AmcsStepperComponent,
  decorators: [
    componentWrapperDecorator((story) => `${generateSharedTranslationsWrapper(story)}`),
    generateModuleMetaDataForStorybook(AmcsStepperComponent, [SharedTranslationsService])
  ],
  parameters: {
    design: {
      disabled: true
    },
    docs: {
      description: {
        component: 'I am the default AMCS Stepper Component for both desktop and mobile.'
      },
      source: {
        type: 'dynamic'
      }
    }
  }
} as Meta;

const Template: Story<AmcsStepperComponent> = (args) => ({
  props: args,
  template: ` <app-amcs-stepper  title="Stepper" stepContentHeight="150px">
         <app-amcs-step>
           <div style="text-align: center" #step>
           <h4>First Step{{heading}}</h4>
           </div>        
         </app-amcs-step>
         <app-amcs-step>
         <div style="text-align: center" #step>
         <h4>Second Step</h4>
         </div> 
         </app-amcs-step>
         <app-amcs-step>
         <div style="text-align: center" #step>
         <h4>Third Step</h4>
         </div> 
         </app-amcs-step>
       </app-amcs-stepper>`
});

export const Primary = Template.bind({});
Primary.args = { ...Primary.args };
