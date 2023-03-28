import { FormControl, FormGroup, Validators } from '@angular/forms';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateFormControlWrapper, generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { componentWrapperDecorator, Meta, Story } from '@storybook/angular';
import { useArgs } from '@storybook/client-api';
import { of, Subject } from 'rxjs';
import { switchMap, takeUntil } from 'rxjs/operators';
import { AmcsSelectTypeaheadComponent } from './amcs-select-typeahead.component';

interface DummyItem {
  name: string;
  email: string;
  age: number;
  country: string;
}

const unsubscribe = new Subject();
let searchLatency = 500;
const dummyItemTextSubject = new Subject<string>();
const dummyItems$ = new Subject<DummyItem[]>();

const listOfDummyItems: DummyItem[] = [
  { name: 'Jill', email: 'jill@email.com', age: 15, country: 'Latvia' },
  { name: 'Henry', email: 'henry@email.com', age: 10, country: 'Latvia' },
  { name: 'Meg', email: 'meg@email.com', age: 7, country: 'Estonia' },
  { name: 'Adam', email: 'adam@email.com', age: 12, country: 'United States' },
  { name: 'Homer', email: 'homer@email.com', age: 47, country: 'Estonia' },
  { name: 'Samantha', email: 'samantha@email.com', age: 30, country: 'United States' },
  { name: 'Amalie', email: 'amalie@email.com', age: 12, country: 'Argentina' },
  { name: 'Estefanía', email: 'estefania@email.com', age: 21, country: 'Argentina' },
  { name: 'Adrian', email: 'adrian@email.com', age: 21, country: 'Ecuador' },
  { name: 'Wladimir', email: 'wladimir@email.com', age: 30, country: 'Ecuador' },
  { name: 'Natasha', email: 'natasha@email.com', age: 54, country: 'Ecuador' },
  { name: 'Nicole', email: 'nicole@email.com', age: 43, country: 'Colombia' },
  { name: 'Michael', email: 'michael@email.com', age: 15, country: 'Colombia' },
  { name: 'Nicolás', email: 'nicole@email.com', age: 43, country: 'Colombia' },
];

export default {
  title: StorybookGroupTitles.FormControls + 'SelectTypeahead',
  component: AmcsSelectTypeaheadComponent,

  args: {
    inputSubject: dummyItemTextSubject,
    items$: dummyItems$,
    bindDropdownLabel: 'name',
    bindLabel: 'name',
    bindValue: 'name',
    searchLatency_ms: searchLatency,
  },
  parameters: {
    design: {
      disabled: true,
    },
    docs: {
      description: {
        component:
          'I am an AMCS Type Ahead Select component. I filter the dropdown list based on my input field, select the value by clicking on it, or using arrows->enter. Please see in the Inputs below for more clarification.',
      },
    },
  },
  argTypes: {
    // Hide these from control options
    inputSubject: {
      table: {
        disable: true,
      },
    },
    items$: {
      table: {
        disable: true,
      },
    },
    searchLatency_ms: {
      description: 'Storybook only option! To set the "fake API" response latency.',
      control: { type: 'number' },
    },
    bindValue: {
      control: { type: 'select' },
      options: ['name', 'email', 'age', 'country'],
      description: 'Controls what key will be used, while filtering, to compare the input value against.',
    },
    bindDropdownLabel: {
      control: { type: 'select' },
      options: ['name', 'email', 'age', 'country'],
      description: 'Controls what key will be used to display the filtered values in the dropdown.',
    },
    bindLabel: {
      control: { type: 'select' },
      options: ['name', 'email', 'age', 'country'],
      description: 'Controls what key will be used to display the value of the selected item.',
    },
  },
  decorators: [
    componentWrapperDecorator((story) => `<form [formGroup]="form">${generateFormControlWrapper(story)}</form>`),
    generateModuleMetaDataForStorybook(AmcsSelectTypeaheadComponent, [SharedTranslationsService]),
  ],
} as Meta<AmcsSelectTypeaheadComponent>;

const Template: Story<AmcsSelectTypeaheadComponent> = (args: AmcsSelectTypeaheadComponent) => {
  const [argz, updateArgs] = useArgs();
  searchLatency = argz.searchLatency_ms;
  unsubscribe.next();
  dummyItemTextSubject
    .pipe(
      takeUntil(unsubscribe),
      switchMap((text: string) => {
        if (!!text) {
          return of(
            listOfDummyItems.filter((dummyListItem: DummyItem) =>
              dummyListItem[args.bindValue].toString().toLowerCase().includes(text.toString().toLocaleLowerCase())
            )
          );
        } else {
          return of([]);
        }
      })
    )
    .subscribe((result: any[]) => {
      updateArgs({ loading: true });
      setTimeout(() => {
        dummyItems$.next(result);
        updateArgs({ loading: false });
      }, searchLatency);
    });

  const formGroup = new FormGroup({
    typeahead: new FormControl(undefined, Validators.required),
  });
  return {
    props: {
      form: formGroup,
      formControlName: 'typeahead',
      inputSubject: args.inputSubject,
      label: args.label,
      items$: args.items$,
      displayMode: args.displayMode,
      hasError: args.hasError,
      isDisabled: args.isDisabled,
      bindLabel: args.bindLabel,
      bindDropdownLabel: args.bindDropdownLabel,
      bindValue: args.bindValue,
      groupBy: args.groupBy,
      selectableGroup: args.selectableGroup,
      appendToBody: args.appendToBody,
      loading: args.loading,
      autoFocus: args.autoFocus,
      hasWarning: args.hasWarning,
    },
  };
};

export const Primary = Template.bind({});
Primary.args = { ...Primary.args, label: 'Select Typeahead - check Docs for info.' };
