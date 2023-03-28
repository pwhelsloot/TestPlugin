import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateFormControlWrapper, generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { componentWrapperDecorator, Meta, Story } from '@storybook/angular';
import { AmcsSelectWithSearchComponent } from './amcs-select-with-search.component';

export default {
  title: StorybookGroupTitles.FormControls + 'Select With Search',
  component: AmcsSelectWithSearchComponent,
  args: {
    items: [
      { id: 0, description: 'Beans', group: 'Food' },
      { id: 1, description: 'Beer', group: 'Drinks' },
      { id: 2, description: 'Beef', group: 'Food' },
      { id: 3, description: 'Coke', group: 'Drinks' },
      { id: 4, description: 'Chicken', group: 'Food' }
    ],
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
        disable: true,
      },
    },
    bindValue: {
      table: {
        disable: true,
      },
    },
    bindLabel: {
      table: {
        disable: true,
      },
    },
  },
  decorators: [
    componentWrapperDecorator((story) => `<form [formGroup] ="form">${generateFormControlWrapper(story)}</form>`),
    generateModuleMetaDataForStorybook(AmcsSelectWithSearchComponent, [])
  ]
} as Meta;

const Template: Story<AmcsSelectWithSearchComponent> = (args) => {
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

export const GroupedOptional = Template.bind({});
GroupedOptional.args = { ...GroupedOptional.args, groupBy: 'group', isOptional: true, label: 'Grouped Search Select' };

export const HasError = Template.bind({});
HasError.args = { ...HasError.args, hasError: true, label: 'Search Select with Error' };
