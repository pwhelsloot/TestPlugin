import { nameof } from '@core-module/helpers/name-of.function';
import { DefaultAction } from '@core-module/models/default-action.model';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { AmcsDropdownIconEnum } from '@shared-module/components/amcs-dropdown/amcs-dropdown-icon-enum.model';
import { GridActionColumnHeaderOptions } from '@shared-module/components/amcs-grid-action-column/amcs-grid-action-column-header/amcs-grid-action-column-header-options';
import { AmcsGridComponent } from '@shared-module/components/amcs-grid/amcs-grid.component';
import { GridColumnAlignment } from '@shared-module/components/amcs-grid/grid-column-alignment.enum';
import { GridColumnConfig } from '@shared-module/components/amcs-grid/grid-column-config';
import { GridColumnType } from '@shared-module/components/amcs-grid/grid-column-type.enum';
import { GridSuperscriptColour } from '@shared-module/components/amcs-grid/grid-superscript-colour.enum';
import { GridTotalsFooterConfig } from '@shared-module/components/amcs-grid/grid-totals-footer-config.model';
import { GridViewport } from '@shared-module/components/amcs-grid/grid-viewport.enum';
import { GridCellColorEnum } from '@shared-module/models/grid-cell-color-enum';
import { ITableItem } from '@shared-module/models/itable-item.model';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { componentWrapperDecorator, Meta, Story } from '@storybook/angular';

export default {
  title: StorybookGroupTitles.Data + 'Grid',
  component: AmcsGridComponent,
  args: {
    data: buildRows(),
    columns: buildColumns(),
    filterPlaceholder: 'Filter',
    rightAlignFilter: false,
    detailsLoading: false,
    loading: false,
    disableSelection: false,
    allowMultiSelect: false,
    includePaging: false,
    allowHorizontalScroll: false,
    totalItemCount: null,
    pageSize: 50,
    pageIndex: 0,
    useDefaultSort: true,
    useDefaultFilter: true,
    removeCellBorders: false,
    alignHeaderWithCell: false,
    truncateText: true,
    gridHeight: 'auto',
    centerRowVertically: false,
    totalsFooterConfig: null,
    totalsHeaderConfig: null,
    AmcsDropdownIconEnum,
    ColumnType: GridColumnType,
    displayMode: FormControlDisplay.Grid,
    Viewport: GridViewport,
    GridSuperscriptColour,
    GridColumnAlignment,
    GridCellColorEnum
  },
  parameters: {
    // Storybook Issue https://github.com/storybookjs/storybook/issues/16865
    // Storybook6.4 is now overriding all public properties + methods and assigning default values so they dont work.
    controls: {
      exclude: ['onRowClicked', 'selectedRowIds', 'onIsSelectedChanged']
    },
    design: {
      disabled: true
    },
    docs: {
      description: {
        component: 'I am an amcs grid. I\'m the default grid component to use for displaying tabular data.'
      },
      source: {
        type: 'code'
      }
    }
  },
  argTypes: {
    // Hide these from control options
    AmcsDropdownIconEnum: {
      table: {
        disable: true
      }
    },
    mobileRowDetailTemplate: {
      table: {
        disable: true
      }
    },
    rowTemplate: {
      table: {
        disable: true
      }
    },
    rowHeaderTemplate: {
      table: {
        disable: true
      }
    },
    rowDetailTemplate: {
      table: {
        disable: true
      }
    },
    totalsFooterConfig: {
      table: {
        disable: true
      }
    },
    totalsHeaderConfig: {
      table: {
        disable: true
      }
    },
    summaryEntity: {
      table: {
        disable: true
      }
    },
    headerTemplate: {
      table: {
        disable: true
      }
    },
    detailTemplate: {
      table: {
        disable: true
      }
    },
    additionalOptionsTemplate: {
      table: {
        disable: true
      }
    },
    actionColumnTemplate: {
      table: {
        disable: true
      }
    },
    popoverTemplate: {
      table: {
        disable: true
      }
    },
    noDataTemplate: {
      table: {
        disable: true
      }
    },
    allowColumnRebind: {
      table: {
        disable: true
      }
    },
    showDeleted: {
      table: {
        disable: true
      }
    }
  },
  decorators: [
    componentWrapperDecorator(
      (story) =>
        `<div class="row"><div class="col-lg-9"><div class="portlet box grey child-portlet bordered"><div class="portlet-body">${story}</div></div></div></div>`
    ),
    generateModuleMetaDataForStorybook(AmcsGridComponent, [])
  ]
} as Meta;

const Template: Story<AmcsGridComponent> = (args) => ({
  props: args
});

// Basic example
export const Basic = Template.bind({});
Basic.args = { ...Basic.args };

export const SingleSelect = Template.bind({});
SingleSelect.args = { ...SingleSelect.args, allowMultiSelect: false, showSelectedCheckbox: true, radioButtonOverride: true };
SingleSelect.argTypes = {
  ...SingleSelect.argTypes,
  allowMultiSelect: {
    table: {
      disable: true
    }
  },
  showSelectedCheckbox: {
    table: {
      disable: true
    }
  },
  radioButtonOverride: {
    table: {
      disable: true
    }
  }
};

export const MultiSelect = Template.bind({});
MultiSelect.args = { ...MultiSelect.args, allowMultiSelect: true, showSelectedCheckbox: true, radioButtonOverride: false };
MultiSelect.argTypes = { ...SingleSelect.argTypes };

// Inputs example
export const Inputs = Template.bind({});
Inputs.args = { ...Inputs.args, columns: buildColumnsForInput() };

// Filtering and paging example
export const FilterAndPaging = Template.bind({});
FilterAndPaging.args = {
  ...FilterAndPaging.args,
  includePaging: true,
  totalItemCount: 5,
  showFilter: true,
  allowMultiSelect: true,
  pageSize: 3,
  pageIndex: 0
};

// Used to attach different templates
const template = `<app-amcs-grid
    [data]="data"
    [columns]="columns"
    [rowDetailTemplate]="rowDetailTemplate"
    [showFilter]="showFilter"
    [filterPlaceholder]="filterPlaceholder"
    [rightAlignFilter]="rightAlignFilter"
    [detailsLoading]="detailsLoading"
    [loading]="loading"
    [disableSelection]="disableSelection"
    [allowMultiSelect]="allowMultiSelect"
    [expandOnSelection]="expandOnSelection"
    [includePaging]="includePaging"
    [allowHorizontalScroll]="allowHorizontalScroll"
    [allowVerticalScroll]="allowVerticalScroll"
    [totalItemCount]="totalItemCount"
    [pageSize]="pageSize"
    [pageIndex]="pageIndex"
    [useDefaultSort]="useDefaultSort"
    [useDefaultFilter]="useDefaultFilter"
    [removeCellBorders]="removeCellBorders"
    [alignHeaderWithCell]="alignHeaderWithCell"
    [truncateText]="truncateText"
    [gridHeight]="gridHeight"
    [centerRowVertically]="centerRowVertically"
    [totalsFooterConfig]="totalsFooterConfig"
    [totalsHeaderConfig]="totalsHeaderConfig"
    [headerTemplate]="headerTemplate"
    [actionColumnTemplate]="actionColumnTemplate"
    [noDataTemplate]="noDataTemplate"
    [actionColumnHeaderOptions]="actionColumnHeaderOptions"
    ></app-amcs-grid>`;

// Row detail example
const rowDetailTemplate = `<ng-template #rowDetailTemplate let-row="row"><h5>{{row.description}}</h5></ng-template>`;

const DetailRowTemplate: Story<AmcsGridComponent> = (args) => ({
  props: args,
  template: template + rowDetailTemplate
});

export const ExpandingDetailsRow = DetailRowTemplate.bind({});
ExpandingDetailsRow.args = { ...ExpandingDetailsRow.args, expandOnSelection: true };

// Header / Footer / Action example
const headerTemplate = `<ng-template #headerTemplate>
    <div class="caption-subject bold uppercase title">
        <span placement="bottom">Custom Header</span>
        <span placement="bottom" [tooltip]="'You can put any html you like here'"> - Hover here</span>
    </div>
</ng-template>`;

const actionColumnTemplate = `<ng-template #actionColumnTemplate let-row="row" let-index="index">
  <app-amcs-dropdown class="dropdown-button"
    [dropdownIcon]="AmcsDropdownIconEnum.Ellipsis">
    <ng-container ngProjectAs="menu">
      <li>
        <a>
          <span>Action One</span></a>
      </li>
      <li>
        <a>
          <span>Action Two</span></a>
      </li>
    </ng-container>
  </app-amcs-dropdown>
</ng-template>`;

const HeaderRowTemplate: Story<AmcsGridComponent> = (args) => ({
  props: args,
  template: template + headerTemplate + actionColumnTemplate
});

export const HeaderFooterAndAction = HeaderRowTemplate.bind({});
HeaderFooterAndAction.args = {
  ...HeaderFooterAndAction.args,
  showFilter: true,
  columns: buildColumnsForFooter(),
  totalsHeaderConfig: { title: 'A totals header' } as GridTotalsFooterConfig,
  totalsFooterConfig: { title: 'A totals footer' } as GridTotalsFooterConfig,
  actionColumnHeaderOptions: new GridActionColumnHeaderOptions()
};

// Vertical Scrolling example
const VerticalScrollingTemplate: Story<AmcsGridComponent> = (args) => ({
  props: args,
  template: template + headerTemplate + actionColumnTemplate
});
export const VerticalScrolling = VerticalScrollingTemplate.bind({});
VerticalScrolling.args = {
  ...VerticalScrolling.args,
  columns: buildColumnsForFooter(),
  totalsHeaderConfig: { title: 'A totals header' } as GridTotalsFooterConfig,
  totalsFooterConfig: { title: 'A totals footer' } as GridTotalsFooterConfig,
  allowVerticalScroll: true,
  gridViewHeight: 75,
  gridHeight: 300
};

// Empty grid example
const emptyTemplate = `<ng-template #noDataTemplate><div>No results in grid</div></ng-template>`;
const EmptyGridTemplate: Story<AmcsGridComponent> = (args) => ({
  props: args,
  template: template + emptyTemplate
});
export const Empty = EmptyGridTemplate.bind({});
Empty.args = { ...Empty.args, data: [] };

function buildColumns(): GridColumnConfig[] {
  const translations: string[] = [];
  translations['defaultActionSelector.columns.service'] = 'Service';
  translations['defaultActionSelector.columns.action'] = 'Action';
  translations['defaultActionSelector.columns.material'] = 'Material';
  translations['defaultActionSelector.columns.pricingBasis'] = 'Pricing Basis';
  translations['defaultActionSelector.columns.unitOfMeasure'] = 'Unit Of Measurement';
  return DefaultAction.getGridColumns(translations);
}

function buildColumnsForFooter(): GridColumnConfig[] {
  return [
    new GridColumnConfig('Service', nameof<DefaultAction>('serviceDescription'), [16]),
    new GridColumnConfig('Action', nameof<DefaultAction>('actionDescription'), [16]),
    new GridColumnConfig('Material', nameof<DefaultAction>('materialDescription'), [16]),
    new GridColumnConfig('Pricing Basis', nameof<DefaultAction>('priceBasisDescription'), [16]),
    new GridColumnConfig('Unit Of Measurement', nameof<DefaultAction>('unitOfMeasurement'), [16]),
    new GridColumnConfig('Vat Rate', nameof<DefaultAction>('vatRate'), [16])
      .withType(GridColumnType.currency)
      .withCurrencyCode('GBP')
      .withIncludeInTotal(true)
  ];
}

function buildColumnsForInput(): GridColumnConfig[] {
  const selectColumn = new GridColumnConfig('Service', nameof<DefaultAction>('serviceId'), [16]).withType(GridColumnType.selectInput);
  selectColumn.selectItems = buildSelectColumnItems();
  return [
    selectColumn,
    new GridColumnConfig('Action', nameof<DefaultAction>('actionDescription'), [16]),
    new GridColumnConfig('Material', nameof<DefaultAction>('materialDescription'), [16]),
    new GridColumnConfig('Pricing Basis', nameof<DefaultAction>('priceBasisDescription'), [16]),
    new GridColumnConfig('Unit Of Measurement', nameof<DefaultAction>('unitOfMeasurement'), [16]),
    new GridColumnConfig('Vat Rate', nameof<DefaultAction>('vatRate'), [16]).withType(GridColumnType.numericInput)
  ];
}

function buildSelectColumnItems(): ILookupItem[] {
  return [
    { id: 1, description: 'Gate' },
    { id: 2, description: 'Material Sale' },
    { id: 3, description: 'Routes' }
  ];
}

function buildRows(): ITableItem[] {
  const rows = [];
  const row1 = new DefaultAction();
  row1.id = 1;
  row1.serviceId = 1;
  row1.serviceDescription = 'Gate';
  row1.actionDescription = 'Processing';
  row1.materialDescription = 'Cardboard';
  row1.priceBasisDescription = 'Per Weight';
  row1.unitOfMeasurement = 'Kgs';
  row1.description = 'I am the first rows detail';
  row1.vatRate = 10;

  const row2 = new DefaultAction();
  row2.id = 2;
  row2.serviceId = 1;
  row2.serviceDescription = 'Gate';
  row2.actionDescription = 'Processing';
  row2.materialDescription = 'Metal Recycling';
  row2.priceBasisDescription = 'Per Weight';
  row2.unitOfMeasurement = 'Kgs';
  row2.description = 'I am the second rows detail';
  row2.vatRate = 20;

  const row3 = new DefaultAction();
  row3.id = 3;
  row3.serviceId = 1;
  row3.serviceDescription = 'Gate';
  row3.actionDescription = 'Gate Arrival';
  row3.materialDescription = null;
  row3.priceBasisDescription = 'Per Job';
  row3.unitOfMeasurement = null;
  row3.description = 'I am the third rows detail';
  row3.vatRate = 30;

  const row4 = new DefaultAction();
  row4.id = 4;
  row4.serviceId = 1;
  row4.serviceDescription = 'Gate';
  row4.actionDescription = 'Processing';
  row4.materialDescription = 'Cardboard';
  row4.priceBasisDescription = 'Per Weight';
  row4.unitOfMeasurement = 'Kgs';
  row4.description = 'I am the fourth rows detail';
  row4.vatRate = 15;

  const row5 = new DefaultAction();
  row5.id = 5;
  row5.serviceId = 1;
  row5.serviceDescription = 'Gate';
  row5.actionDescription = 'Processing';
  row5.materialDescription = 'Metal Recycling';
  row5.priceBasisDescription = 'Per Weight';
  row5.unitOfMeasurement = 'Kgs';
  row5.description = 'I am the fifth rows detail';
  row5.vatRate = 25;

  const row6 = new DefaultAction();
  row6.id = 6;
  row6.serviceId = 1;
  row6.serviceDescription = 'Gate';
  row6.actionDescription = 'Gate Arrival';
  row6.materialDescription = null;
  row6.priceBasisDescription = 'Per Job';
  row6.unitOfMeasurement = null;
  row6.description = 'I am the sixth rows detail';
  row6.vatRate = 35;

  rows.push(row1);
  rows.push(row2);
  rows.push(row3);
  rows.push(row4);
  rows.push(row5);
  rows.push(row6);
  return rows;
}
