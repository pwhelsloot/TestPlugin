import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { AmcsSelectComponent } from '@shared-module/components/amcs-select/amcs-select.component';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateFormControlWrapper, generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { componentWrapperDecorator, Meta, Story } from '@storybook/angular';

export default {
  title: StorybookGroupTitles.FormControls + 'Select',
  component: AmcsSelectComponent,
  args: {
    options: buildRows(),
    displayMode: FormControlDisplay.Standard,
    customPlaceholder: 'Select',
    FormControlDisplay,
  },
  parameters: {
    // Storybook Issue https://github.com/storybookjs/storybook/issues/16865
    // Storybook6.4 is now overriding all public properties + methods and assigning default values so they dont work.
    controls: {
      exclude: ['typeCheck'],
    },
    design: {
      disabled: true,
    },
    docs: {
      description: {
        component: 'I am an amcs select. I\'m the default component to use for a drop-down selection.',
      },
      source: {
        type: 'code',
      },
    },
  },
  argTypes: {
    displayMode: {
      options: ['standard', 'small', 'grid'],
      mapping: {
        // 'mapping' maps values to options
        standard: FormControlDisplay.Standard,
        small: FormControlDisplay.Small,
        grid: FormControlDisplay.Grid,
      },
      control: {
        type: 'select',
        labels: {
          // 'labels' maps option values to string labels
          standard: 'Standard',
          small: 'Small',
          grid: 'Grid',
        },
      },
    },
    // Hide these from control options
    selectTooltip: {
      table: {
        disable: true,
      },
    },
    customPlaceholder: {
      table: {
        disable: true,
      },
    },
    previewChanges: {
      table: {
        disable: true,
      },
    },
    keepOriginalOrder: {
      table: {
        disable: true,
      },
    },
    useNumericValues: {
      table: {
        disable: true,
      },
    },
    autoFocus: {
      table: {
        disable: true,
      },
    },
  },
  decorators: [
    componentWrapperDecorator((story) => `<form [formGroup]="form">${generateFormControlWrapper(story)}</form>`),
    generateModuleMetaDataForStorybook(AmcsSelectComponent, []),
  ],
} as Meta;

const Template: Story<AmcsSelectComponent> = (args) => {
  const formGroup = new FormGroup({
    select: new FormControl(undefined, Validators.required),
  });
  return {
    props: {
      form: formGroup,
      formControlName: 'select',
      label: args.label,
      displayMode: args.displayMode,
      customClass: args.customClass,
      customPlaceholder: args.customPlaceholder,
      isLargeSelect: args.isLargeSelect,
      hasError: args.hasError,
      loading: args.loading,
      isSecondaryUi: args.isSecondaryUi,
      isOptional: args.isOptional,
      isDisabled: args.isDisabled,
      options: args.options,
    },
  };
};

export const Primary = Template.bind({});
Primary.args = { ...Primary.args, label: 'I\'m a label for this select control' };

export const Secondary = Template.bind({});
Secondary.args = { ...Secondary.args, hasError: true, label: 'I\'m a label for this select control' };

function buildRows(): ILookupItem[] {
  const rows = [];
  const row1: ILookupItem = { id: 0, description: '' };
  row1.id = 1;
  row1.description = 'Description One';

  const row2: ILookupItem = { id: 0, description: '' };
  row2.id = 2;
  row2.description = 'Description Two';

  const row3: ILookupItem = { id: 0, description: '' };
  row3.id = 3;
  row3.description = 'Description Three';

  rows.push(row1);
  rows.push(row2);
  rows.push(row3);
  return rows;
}
