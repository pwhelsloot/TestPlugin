import { FormBuilder } from '@angular/forms';
import { AmcsFormBuilder } from '@shared-module/forms/amcs-form-builder.model';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { useArgs } from '@storybook/client-api';
import { SBExampleFormModal } from '../amcs-form-modal/amcs-form-modal-storybook/storybook-example-form-data.model';
import { SBExampleModalForm } from '../amcs-form-modal/amcs-form-modal-storybook/storybook-example-form.model';
import { FormOptions } from '../layouts/amcs-form/form-options.model';
import { AmcsListViewComponent } from './amcs-list-view.component';

function buildGridData() {
  const row1 = new SBExampleFormModal();
  row1.id = 1;
  row1.testId = 1;
  row1.testIdTwo = 11;
  row1.testName = 'Type One';

  const row2 = new SBExampleFormModal();
  row2.id = 2;
  row2.testId = 2;
  row2.testIdTwo = 22;
  row2.testName = 'Type Two';

  const row3 = new SBExampleFormModal();
  row3.id = 3;
  row3.testId = 3;
  row3.testIdTwo = 33;
  row3.testName = 'Type Three';

  return [row1, row2, row3];
}

const formBuilder = new FormBuilder();
const formOptions = new FormOptions();

let gridData: SBExampleFormModal[] = buildGridData();
let addNewItem = false;
let nextId = 4;
let form: SBExampleModalForm = null;

export default {
  title: StorybookGroupTitles.Data + 'List View',
  component: AmcsListViewComponent,
  args: {
    items: gridData,
    addNewItem
  },
  parameters: {
    design: {
      disabled: true
    },
    docs: {
      description: {
        component: 'I am amcs-list-view.'
      },
      source: {
        type: 'code'
      }
    }
  },
  argTypes: {
    expandable: {
      table: {
        disable: true
      }
    }
  },
  decorators: [generateModuleMetaDataForStorybook(AmcsListViewComponent, [])]
} as Meta;

const template = `
<a (click)="openNewListView(!addNewItem)" appAutomationLocator [compName]="'ListViewStorybookComponent'" [uniqueKey]="'addNew'">Add New Item</a>
<app-amcs-list-view
  [compName]="'ListViewStorybookComponent'"
  [items]="items"
  [listItemHeaderTemplate]="listItemHeaderTemplate"
  [listItemTemplate]="listItemTemplate"
  [formTemplate]="formTemplate"
  [addNewItem]="addNewItem"
>
</app-amcs-list-view>`;

const headerTemplate = `
<ng-template #listItemHeaderTemplate>
  <div class="col-lg-3 col-md-3 col-sm-3">
    <span class="bold">Test Id</span>
  </div>
  <div class="col-lg-3 col-md-3 col-sm-3">
    <span class="bold">Test Quantity</span>
  </div>
  <div class="col-lg-3 col-md-3 col-sm-3">
    <span class="bold">Test Type</span>
  </div>
</ng-template>`;

const listTemplate = `
<ng-template #listItemTemplate let-item="item">
  <div class="col-lg-3 col-md-3 col-sm-3">{{item.testId}}</div>
  <div class="col-lg-2 col-md-2 col-sm-2">
    <span>{{item.testIdTwo}}</span>
  </div>
  <div class="col-lg-2 col-md-2 col-sm-2">
    <span>{{item.testName}}</span>
  </div>
  <div class="col-lg-1 col-md-1 col-sm-1 align-right">
    <a class="action-button" (click)="editItem(item)" appAutomationLocator [compName]="'ListViewStorybookComponent'" [uniqueKey]="'edit'"
      ><i class="fas fa-pencil-alt"></i
    ></a>
    <a
      class="action-button"
      (click)="deleteItem(item)"
      appAutomationLocator
      [compName]="'ListViewStorybookComponent'"
      [uniqueKey]="'delete'"
      ><i class="fas fa-trash-alt"></i
    ></a>
  </div>
</ng-template>`;

const formTemplate = `
<ng-template #formTemplate>
  <div class="row form-container">
    <app-amcs-form [compName]="'ListViewStorybookComponent'" [form]="form" [options]="formOptions" (onSave)="saveListItem()" (onCancel)="openNewListView(false) ">
      <ng-template>
        <ng-container>
          <div class="row">
            <div class="col-lg-12">
              <app-amcs-numerical-input
                [compName]="'ListViewStorybookComponent'"
                formControlName="testId"
                [precision]="0"
                [maxLength]="5"
                [label]="'Input Test Id'"
                [thousandsDeliminator]="''"
              >
              </app-amcs-numerical-input>
            </div>
            <div class="col-lg-12">
              <app-amcs-numerical-input
                [compName]="'ListViewStorybookComponent'"
                formControlName="testIdTwo"                
                [precision]="0"
                [maxLength]="5"
                [label]="'Input Test Quantity'"
              >
              </app-amcs-numerical-input>
            </div>
            <div class="col-lg-12">
              <app-amcs-input
                [compName]="'ListViewStorybookComponent'"
                formControlName="testName"
                [maxLength]="50"
                [label]="'Input Test Type'"
              >
              </app-amcs-input>
            </div>
          </div>
        </ng-container>
      </ng-template>
    </app-amcs-form>
  </div>
</ng-template>`;

const tileWrapperOpenTemplate = `<div class="row">
<div class="col-lg-6">
  <div class="portlet box grey child-portlet bordered"><div class="portlet-body">`;

const tileWrapperCloseTemplate = `</div></div></div>
</div>`;

const Template: Story<AmcsListViewComponent> = (args) => {
  const [, updateArgs] = useArgs();
  const openNewListView = (shouldItOpen) => {
    if (shouldItOpen) {
      form = AmcsFormBuilder.buildForm(formBuilder, new SBExampleFormModal(), SBExampleModalForm);
      form.id.setValue(nextId);
    } else {
      form = null;
    }
    gridData.forEach((x) => ((x as any).editing = false));
    addNewItem = shouldItOpen;
    setTimeout(() => {
      updateArgs({ addNewItem });
    }, 200);
  };

  const editItem = (item) => {
    openNewListView(false);
    form = AmcsFormBuilder.buildForm(formBuilder, item, SBExampleModalForm);
    item.editing = true;
    updateArgs({});
  };

  const deleteItem = (item) => {
    gridData = gridData.filter((data) => data.id !== item.id);
    updateArgs({ items: gridData });
  };

  const saveListItem = () => {
    const newItemToAdd = AmcsFormBuilder.parseForm(form, SBExampleModalForm);
    let matchFound = false;
    gridData = gridData.map((data) => {
      if (data.id === newItemToAdd.id) {
        matchFound = true;
        return newItemToAdd;
      }
      return data;
    });
    if (!matchFound) {
      gridData.push(newItemToAdd);
    }
    nextId++;
    setTimeout(() => {
      updateArgs({ items: gridData });
      //timeout to prevent SB from refreshing (updateArgs issue)
    }, 200);
    openNewListView(false);
  };

  return {
    props: {
      ...args,
      openNewListView,
      editItem,
      saveListItem,
      deleteItem,
      form,
      formOptions
    },
    template: args.expandable
      ? tileWrapperOpenTemplate + template + tileWrapperCloseTemplate + headerTemplate + listTemplate + formTemplate
      : template + headerTemplate + listTemplate + formTemplate
  };
};

export const Primary = Template.bind({});
Primary.args = { ...Primary.args, expandable: false };

export const WithWrapper = Template.bind({});
WithWrapper.args = { ...WithWrapper.args, expandable: true };
