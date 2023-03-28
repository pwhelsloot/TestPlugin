import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateFormControlWrapper, generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { componentWrapperDecorator, Meta, Story } from '@storybook/angular';
import { AmcsMultiSelectWithSearchComponent } from './amcs-multi-select-with-search.component';

const items = [
  { id: 0, description: 'Beans', group: 'Food' },
  { id: 1, description: 'Beer', group: 'Drinks' },
  { id: 2, description: 'Beef', group: 'Food' },
  { id: 3, description: 'Coke', group: 'Drinks' },
  { id: 4, description: 'Chicken', group: 'Food' }
];

export default {
  title: StorybookGroupTitles.FormControls + 'Multi-Select With Search',
  component: AmcsMultiSelectWithSearchComponent,
  args: {
    items,
    displayMode: FormControlDisplay.Standard,
    FormControlDisplay,
    bindValue: 'id',
    bindLabel: 'description',
    isOptional: false
  },
  parameters: {
    docs: {
      description: {
        component: 'I am an amcs select. I am the default component to use for a drop-down selection.'
      },
      source: {
        type: 'code'
      }
    }
  },
  argTypes: {
    // Hide these from control options
    label: {
      table: {
        disable: true
      }
    },
    bindValue: {
      table: {
        disable: true
      }
    },
    bindLabel: {
      table: {
        disable: true
      }
    }
  },
  decorators: [
    componentWrapperDecorator((story) => `<form [formGroup] ="form">${generateFormControlWrapper(story)}</form>`),
    generateModuleMetaDataForStorybook(AmcsMultiSelectWithSearchComponent, [])
  ]
} as Meta;

const Template: Story<AmcsMultiSelectWithSearchComponent> = (args) => {
  const formGroup = new FormGroup({
    select: new FormControl(undefined, Validators.required)
  });
  return {
    props: {
      label: args.label,
      form: formGroup,
      items: args.items,
      loading: args.loading,
      bindLabel: args.bindLabel,
      bindValue: args.bindValue,
      groupBy: args.groupBy,
      isDisabled: args.isDisabled,
      customClass: args.customClass,
      csvTextLabel: args.csvTextLabel,
      hasError: args.hasError,
      isOptional: args.isOptional,
      keepOriginalOrder: args.keepOriginalOrder,
      autoFocus: args.autoFocus,
      appendToBody: args.appendToBody,
      appendTo: args.appendTo,
      dropdownPosition: args.dropdownPosition,
      displayMode: args.displayMode,
      isSecondaryUi: args.isSecondaryUi
    }
  };
};
const TemplateWithTemplate: Story<AmcsMultiSelectWithSearchComponent> = (args: AmcsMultiSelectWithSearchComponent) => {
  const formGroup = new FormGroup({
    select: new FormControl(undefined, Validators.required)
  });
  return {
    props: {
      label: args.label,
      form: formGroup,
      items: args.items,
      loading: args.loading,
      bindLabel: args.bindLabel,
      bindValue: args.bindValue,
      groupBy: args.groupBy,
      isDisabled: args.isDisabled,
      customClass: args.customClass,
      csvTextLabel: args.csvTextLabel,
      hasError: args.hasError,
      isOptional: args.isOptional,
      keepOriginalOrder: args.keepOriginalOrder,
      autoFocus: args.autoFocus,
      appendToBody: args.appendToBody,
      appendTo: args.appendTo,
      dropdownPosition: args.dropdownPosition,
      displayMode: args.displayMode,
      isSecondaryUi: args.isSecondaryUi
    },
    template: ` <app-amcs-multi-select-with-search
    [items]="items"
    [bindLabel]="'description'"
    [bindValue]="'id'"
    [rowTemplate]="multisearchTemplate"
    [csvTextLabel]="csvTextLabel"
    [isCheckbox]="false"
    [keepOriginalOrder]="true"
    [label]="'Search Multiselect using Custom Template'"
  ></app-amcs-multi-select-with-search>
  <ng-template #multisearchTemplate let-item="item" let-item$="item$" let-index="index">
    <i *ngIf="item.id % 3 === 0" class="fas fa-map-marker-alt user-source dropdown-padding"></i>
    <i *ngIf="item.id % 3 === 1" class="fas fa-user user-source dropdown-padding"></i>
    <i *ngIf="item.id % 3 === 2" class="fas fa-poop user-source dropdown-padding"></i> {{ item.description }}
  </ng-template>`
  };
};

export const GroupedOptional = Template.bind({});
GroupedOptional.args = { ...GroupedOptional.args, groupBy: 'group', isOptional: true, label: 'Grouped Search Multiselect' };

export const HasError = Template.bind({});
HasError.args = { ...HasError.args, hasError: true, label: 'Search Multiselect with Error' };

export const UsingTemplate = TemplateWithTemplate.bind({});
UsingTemplate.args = { ...UsingTemplate.args, items};
