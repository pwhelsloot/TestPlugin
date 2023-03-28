import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ModalGridSelectorServiceAdapter } from '@core-module/services/forms/modal-grid-selector.adapter';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateFormControlWrapper, generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { componentWrapperDecorator, Meta, Story } from '@storybook/angular';
import { useArgs } from '@storybook/client-api';
import { GridColumnConfig } from '../amcs-grid/grid-column-config';
import { AmcsModalSelectorComponent } from './amcs-modal-selector.component';
import { SBModelSelectorItem, StorybookeModalGridSelectorService } from './storybook-modal-grid-selector.service';

function getGridColumns(): GridColumnConfig[] {
  return [
    new GridColumnConfig('Service', 'column1', [10, 10]),
    new GridColumnConfig('Action', 'column2', [10, 10]),
    new GridColumnConfig('Pricing basis', 'column3', [10, 10])
  ];
}
const formGroup = new FormGroup({
  modalselector: new FormControl(undefined, Validators.required)
});

export default {
  title: StorybookGroupTitles.FormControls + 'Modal Selector',
  component: AmcsModalSelectorComponent,

  args: { modalTitle: '' },
  argTypes: {
    // Hide these from control options
    modalSelectorMode: {
      table: {
        disable: true
      }
    }
  },
  parameters: {
    docs: {
      description: {
        component: 'I am an AMCS Modal Selector component. I display value/values selected in my popup modal selection grid.'
      }
    }
  },
  decorators: [
    componentWrapperDecorator((story) => `<form [formGroup]="form">${generateFormControlWrapper(story)}</form>`),
    generateModuleMetaDataForStorybook(AmcsModalSelectorComponent, [
      SharedTranslationsService,
      StorybookeModalGridSelectorService,
      {
        provide: ModalGridSelectorServiceAdapter,
        useExisting: StorybookeModalGridSelectorService
      }
    ])
  ]
} as Meta<AmcsModalSelectorComponent>;

const Template: Story<AmcsModalSelectorComponent> = (args: AmcsModalSelectorComponent) => {
  const [, updateArgs] = useArgs();

  const defaultActionSelected = (data: SBModelSelectorItem[]) => {
    if (data === null) {
      updateArgs({ description: '' });
      return;
    }
    if (Array.isArray(data)) {
      const defaultAction = data as SBModelSelectorItem[];
      const descript = defaultAction.map((action) => ` ${action.column1} - ${action.column2}`);
      updateArgs({ description: descript });
    } else {
      const item = data as SBModelSelectorItem;
      const descript = `${item.column1}, ${item.column2}, ${item.column3}`;
      updateArgs({ description: descript });
    }
  };
  return {
    props: {
      compName: 'GridDesignsComponent',
      uniqueKey: 'sample',
      description: args.description,
      itemsSelected: defaultActionSelected,
      columns: getGridColumns(),
      modalTitle: args.modalTitle,
      form: formGroup,
      formControlName: 'modalselector',
      modalSelectorMode: args.modalSelectorMode,
      label: args.label
    }
  };
};

export const SingleSelect = Template.bind({});
SingleSelect.args = {
  ...SingleSelect.args,
  modalSelectorMode: 1,
  label: 'Modal Selector - Single Select',
  modalTitle: 'Sample Single Select Modal Title'
};

export const MultiSelect = Template.bind({});
MultiSelect.args = {
  ...MultiSelect.args,
  modalSelectorMode: 2,
  label: 'Modal Selector - Multi Select',
  modalTitle: 'Sample Multi Select Modal Title'
};
