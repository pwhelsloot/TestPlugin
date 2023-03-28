import { DatePipe, DecimalPipe } from '@angular/common';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { AmcsDataGridColDefinitions } from './amcs-column-definitions';
import { AmcsDataGridComponent } from './amcs-data-grid.component';
import { moduleDeclarations, moduleImports } from './amcs-data-grid.module';
import { AmcsDataGridOptionsBuilder } from './amcs-data-grid.options';

export default {
  title: StorybookGroupTitles.Data + 'DataGrid',
  parameters: {
    design: {
      disabled: true,
    },
    docs: {
      description: {
        component: 'I am a data grid, use me for showing lots of data!',
      },
      source: {
        type: 'code',
      },
    },
  },
  decorators: [generateModuleMetaDataForStorybook(AmcsDataGridComponent, [DatePipe, DecimalPipe], moduleImports, moduleDeclarations)],
} as Meta<AmcsDataGridComponent>;

const Template: Story<AmcsDataGridComponent> = (args: AmcsDataGridComponent) => ({
  component: AmcsDataGridComponent,
  props: args,
  template: `
  <div style="height: 100vh">
    <app-amcs-data-grid
    [options]="options"
    [rowData]="rowData"
    [columnDefs]="columnDefs">
    </app-amcs-data-grid>
  </div>`,
});

const columnDefs = [
  AmcsDataGridColDefinitions.createTextColDef({
    field: 'make',
    editable: true,
  }),
  AmcsDataGridColDefinitions.createTextColDef({
    field: 'model',
    editable: true,
  }),
  AmcsDataGridColDefinitions.createCheckboxColDef({
    field: 'insured',
    editable: true,
  }),
  AmcsDataGridColDefinitions.createNumericColDef({
    field: 'price',
    editable: true,
  }),
  AmcsDataGridColDefinitions.createDateColDef({
    field: 'released',
    editable: true,
  }),
];
const releaseDate = new Date();
const anotherReleaseDate = new Date();
anotherReleaseDate.setMonth(0);
const rowData = [
  {
    make: 'Porsche',
    model: 'Macan',
    price: 72000,
    insured: true,
    released: releaseDate,
  },
  {
    make: 'Seat',
    model: 'Ateca',
    insured: true,
    price: 39000,
    released: anotherReleaseDate,
  },
];

const options = AmcsDataGridOptionsBuilder.withCount;

export const Primary = Template.bind({});
Primary.args = {
  columnDefs,
  rowData,
  options,
};
