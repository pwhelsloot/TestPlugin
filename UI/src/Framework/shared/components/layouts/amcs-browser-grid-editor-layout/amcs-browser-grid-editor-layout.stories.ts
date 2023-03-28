import { FormControl, FormGroup, Validators } from '@angular/forms';
import { nameof } from '@core-module/helpers/name-of.function';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { PersonalAccessTokenForm } from '@core-module/models/personal-access-token/personal-access-token-form.model';
import { PersonalAccessToken } from '@core-module/models/personal-access-token/personal-access-token.model';
import { ActionColumnButton } from '@shared-module/components/amcs-grid-action-column/amcs-grid-action-button';
import { GridColumnConfig } from '@shared-module/components/amcs-grid/grid-column-config';
import { GridColumnType } from '@shared-module/components/amcs-grid/grid-column-type.enum';
import { GridTotalsHeaderConfig } from '@shared-module/components/amcs-grid/grid-totals-header-config.model';
import { AmcsBrowserGridEditorLayoutComponent } from '@shared-module/components/layouts/amcs-browser-grid-editor-layout/amcs-browser-grid-editor-layout.component';
import { BrowserGridEditorActionButtonEnum } from '@shared-module/components/layouts/amcs-browser-grid-editor-layout/browser-grid-editor-action-button.enum';
import { BrowserGridEditorOptions } from '@shared-module/components/layouts/amcs-browser-grid-editor-layout/browser-grid-editor-options.model';
import { BaseFormGroup } from '@shared-module/forms/base-form-group.model';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { componentWrapperDecorator, Meta, Story } from '@storybook/angular';
import { useArgs } from '@storybook/client-api';

export default {
  title: StorybookGroupTitles.Layouts + 'Browser Grid Editor',
  component: AmcsBrowserGridEditorLayoutComponent,
  args: {
    options: buildOptions(),
    form: null,
    users: buildUsers(),
    gridLoading: true,
    editorLoading: false
  },
  argTypes: {
    form: {
      table: {
        disable: true
      }
    }
  },
  parameters: {
    design: {
      disabled: true
    },
    docs: {
      description: {
        component:
          'Use this component to quickly create an entire screen with a browser portlet, grid with inline edit/delete buttons and a full width form editor.'
      },
      source: {
        type: 'code'
      }
    }
  },

  decorators: [
    componentWrapperDecorator((story) => `${story}`),
    generateModuleMetaDataForStorybook(AmcsBrowserGridEditorLayoutComponent, [])
  ]
} as Meta;

const Template: Story<AmcsBrowserGridEditorLayoutComponent> = (args) => {
  const [, updateArgs] = useArgs();
  const handleAdd = () => {
    updateArgs({ ...args, editorLoading: true });
    setTimeout(() => {
      updateArgs({ ...args, form: buildForm(), editorLoading: false });
    }, 1000);
  };
  const handleEdit = (row) => {
    updateArgs({ ...args, editorLoading: true });
    setTimeout(() => {
      updateArgs({ ...args, form: buildForm(row), editorLoading: false });
    }, 1000);
  };
  const handleSave = () => {
    updateArgs({ ...args, editorLoading: true });
    setTimeout(() => {
      args.options.gridOptions = { ...args.options.gridOptions };
      args.options.gridOptions.data = updateRow(args.options.gridOptions.data, args.form);
      updateArgs({ ...args, form: null, editorLoading: false });
    }, 1000);
  };
  const handleDelete = (row) => {
    updateArgs({ ...args, gridLoading: true });
    setTimeout(() => {
      args.options.gridOptions = { ...args.options.gridOptions };
      args.options.gridOptions.data = removeRow(args.options.gridOptions.data, row);
      updateArgs({ ...args, form: null, gridLoading: false });
    }, 1000);
  };
  const handleCancel = () => {
    updateArgs({ ...args, form: null });
  };
  if (args.gridLoading) {
    setTimeout(() => {
      updateArgs({ ...args, gridLoading: false });
    }, 1000);
  }
  return {
    props: {
      ...args,
      onAdd: handleAdd,
      onEdit: handleEdit,
      onSave: handleSave,
      onDelete: handleDelete,
      onCancel: handleCancel
    },
    template: `<app-amcs-browser-grid-editor-layout [options]="options" [form]="form" [editorLoading]="editorLoading" (onCancel)="onCancel()"
          [gridLoading]="gridLoading" (onAdd)="onAdd()" (onEdit)="onEdit($event)" (onDelete)="onDelete($event)" (onSave)="onSave()">
          <ng-template>
                    <div class="row">
                        <div class="col-lg-12">
                            <app-amcs-input [autoFocus]="true" [maxLength]="60"
                                formControlName="description">
                                <ng-container ngProjectAs="label">
                                    <span>Description</span>
                                    <span class="required" aria-required="true">*</span>
                                </ng-container>
                            </app-amcs-input>
                        </div>
                        <div class="col-lg-6">
                            <app-amcs-select formControlName="sysUserId" [isOptional]="false"
                                [options]="users">
                                <span>User Name</span>
                                <span class="required" aria-required="true">*</span>
                            </app-amcs-select>
                        </div>
                        <div class="col-lg-6">
                            <app-amcs-datepicker formControlName="expire">
                                <span>Expiry</span>
                                <span class="required" aria-required="true">*</span>
                            </app-amcs-datepicker>
                        </div>
                    </div>
          </ng-template>
      </app-amcs-browser-grid-editor-layout>`
  };
};

// Basic example
export const Layout = Template.bind({});
Layout.args = { ...Layout.args };

export const LargeLayout = Template.bind({});
LargeLayout.args = { ...LargeLayout.args, options: buildLargeOptions() };

function buildOptions(): BrowserGridEditorOptions {
  const options = new BrowserGridEditorOptions();
  options.browserOptions.title = 'Personal Access Tokens';
  options.gridOptions.data = buildRows();
  options.gridOptions.columns = buildColumns();
  return options;
}

function buildLargeOptions(): BrowserGridEditorOptions {
  const options = buildOptions();
  options.editLinkColumnKey = nameof<PersonalAccessToken>('description');
  options.gridOptions.data = options.gridOptions.data.concat(buildLargeRows());
  options.gridOptions.allowVerticalScroll = true;
  // These should match to ensure no resizing of the portlet between grid/editor
  options.gridOptions.gridHeight = 300;
  options.editorMinHeight = 300;
  options.gridOptions.totalsHeaderConfig = { title: 'An example with hyperlinks (no pencil icon)' } as GridTotalsHeaderConfig;
  const extraButton = new ActionColumnButton();
  extraButton.id = 3;
  extraButton.icon = 'fal fa-file-signature';
  options.gridOptions.actionColumnButtons.push(extraButton);
  options.gridOptions.actionColumnButtons = options.gridOptions.actionColumnButtons.filter(
    (x) => x.id !== BrowserGridEditorActionButtonEnum.Delete
  );
  return options;
}

function buildForm(row?: PersonalAccessToken): BaseFormGroup {
  const form = new PersonalAccessTokenForm();
  // Don't copy building, use AmcsFormBuilder.buildForm, only doing this as can't inject formBuilder into story
  form.htmlFormGroup = new FormGroup({
    id: new FormControl(row?.id),
    description: new FormControl(row?.description, [Validators.required, Validators.maxLength(60)]),
    sysUserId: new FormControl(row?.sysUserId, [Validators.required]),
    expire: new FormControl(row?.expire, [Validators.required])
  });
  return form;
}

function removeRow(rows: PersonalAccessToken[], rowToRemove: PersonalAccessToken): PersonalAccessToken[] {
  return rows.filter((x) => x.id !== rowToRemove.id);
}

function updateRow(rows: PersonalAccessToken[], form: BaseFormGroup): PersonalAccessToken[] {
  // Don't copy parsing, use AmcsFormBuilder.parseForm, only doing this as can't inject formBuilder into story
  const parsedForm = new PersonalAccessToken();
  parsedForm.id = form.htmlFormGroup.get('id').value;
  parsedForm.description = form.htmlFormGroup.get('description').value;
  parsedForm.sysUserId = form.htmlFormGroup.get('sysUserId').value;
  parsedForm.expire = form.htmlFormGroup.get('expire').value;
  parsedForm.userName = buildUsers().find((x) => x.id).description;
  let existingRow = rows.find((x) => x.id === parsedForm.id);
  if (!existingRow) {
    parsedForm.id =
      Math.max.apply(
        Math,
        rows.map(function(o) {
          return o.id;
        })
      ) + 1;
    parsedForm.creationDate = new Date();
    rows.push(parsedForm);
  } else {
    existingRow.sysUserId = parsedForm.sysUserId;
    existingRow.userName = parsedForm.userName;
    existingRow.description = parsedForm.description;
    existingRow.expire = parsedForm.expire;
  }
  return rows;
}

function buildUsers(): ILookupItem[] {
  return [
    {
      id: 1,
      description: 'Admin'
    },
    {
      id: 2,
      description: 'GPRS User'
    },
    {
      id: 3,
      description: 'Rob Mcil'
    }
  ];
}

function buildColumns(): GridColumnConfig[] {
  const translations: string[] = [];
  translations['personalAccessToken.description'] = 'Description';
  translations['personalAccessToken.userName'] = 'UserName';
  translations['personalAccessToken.creationDate'] = 'Creation Date';
  translations['personalAccessToken.expiryDate'] = 'Expiry Date';
  const columns: GridColumnConfig[] = PersonalAccessToken.getGridColumns(translations, null);
  columns.find((x) => x.key === nameof<PersonalAccessToken>('creationDate')).withType(GridColumnType.shortDateTime);
  columns.find((x) => x.key === nameof<PersonalAccessToken>('expire')).withType(GridColumnType.shortDateTime);
  return columns.filter((x) => x.key !== nameof<PersonalAccessToken>('privateKey'));
}

function buildRows(): PersonalAccessToken[] {
  const rows = [];
  const row1 = new PersonalAccessToken();
  row1.id = 1;
  row1.description = 'PAT 1';
  row1.userName = 'Admin';
  row1.sysUserId = 1;
  row1.creationDate = new Date();
  row1.expire = new Date();
  row1.expire.setMonth(new Date().getMonth() + 2);

  const row2 = new PersonalAccessToken();
  row2.id = 2;
  row2.description = 'PAT 2';
  row2.userName = 'GPRS User';
  row2.sysUserId = 2;
  row2.creationDate = new Date();
  row2.expire = new Date();
  row2.expire.setMonth(new Date().getMonth() + 3);

  const row3 = new PersonalAccessToken();
  row3.id = 3;
  row3.description = 'PAT 3';
  row3.userName = 'Rob Mcil';
  row3.sysUserId = 3;
  row3.creationDate = new Date();
  row3.expire = new Date();
  row3.expire.setMonth(new Date().getMonth() + 4);

  rows.push(row1);
  rows.push(row2);
  rows.push(row3);
  return rows;
}

function buildLargeRows(): PersonalAccessToken[] {
  const rows = [];
  const row4 = new PersonalAccessToken();
  row4.id = 4;
  row4.description = 'PAT 4';
  row4.userName = 'Admin';
  row4.sysUserId = 1;
  row4.creationDate = new Date();
  row4.expire = new Date();
  row4.expire.setMonth(new Date().getMonth() + 2);

  const row5 = new PersonalAccessToken();
  row5.id = 5;
  row5.description = 'PAT 5';
  row5.userName = 'GPRS User';
  row5.sysUserId = 2;
  row5.creationDate = new Date();
  row5.expire = new Date();
  row5.expire.setMonth(new Date().getMonth() + 8);

  const row6 = new PersonalAccessToken();
  row6.id = 6;
  row6.description = 'PAT 6';
  row6.userName = 'Rob Mcil';
  row6.sysUserId = 3;
  row6.creationDate = new Date();
  row6.expire = new Date();
  row6.expire.setMonth(new Date().getMonth() + 5);

  const row7 = new PersonalAccessToken();
  row7.id = 7;
  row7.description = 'PAT 7';
  row7.userName = 'Rob Mcil';
  row7.sysUserId = 3;
  row7.creationDate = new Date();
  row7.expire = new Date();
  row7.expire.setMonth(new Date().getMonth() + 1);

  const row8 = new PersonalAccessToken();
  row8.id = 8;
  row8.description = 'PAT 8';
  row8.userName = 'Rob Mcil';
  row8.sysUserId = 3;
  row8.creationDate = new Date();
  row8.expire = new Date();
  row8.expire.setMonth(new Date().getMonth() + 14);

  rows.push(row4);
  rows.push(row5);
  rows.push(row6);
  rows.push(row7);
  rows.push(row8);
  return rows;
}
