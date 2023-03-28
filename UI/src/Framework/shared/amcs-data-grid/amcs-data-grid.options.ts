// eslint-disable-next-line max-classes-per-file
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { GridOptions } from 'ag-grid-enterprise';
import { AmcsDataGridRowData } from './amcs-data-grid-row-data';
import { AmcsColDef } from './amcs-column-definitions';
import { CheckboxCellEditorComponent } from './cells/checkbox/checkbox-cell-editor/checkbox-cell-editor.component';
import { CheckboxCellRendererComponent } from './cells/checkbox/checkbox-cell-renderer/checkbox-cell-renderer.component';
import { DateCellEditorComponent } from './cells/date/date-cell-editor/date-cell-editor.component';
import { DateCellRendererComponent } from './cells/date/date-cell-renderer/date-cell-renderer.component';
import { NumericCellEditorComponent } from './cells/numeric/numeric-cell-editor/numeric-cell-editor.component';
import { NumericCellRendererComponent } from './cells/numeric/numeric-cell-renderer/numeric-cell-renderer.component';
import { ValidationCellRendererComponent } from './cells/validation/validation-cell-renderer/validation-cell-renderer.component';
import { SingleClickCheckboxCellRendererComponent } from '@shared-module/amcs-data-grid/cells/checkbox/single-click-checkbox-cell-renderer/single-click-checkbox-cell-renderer.component';

// eslint-disable-next-line @typescript-eslint/no-empty-interface
export interface AmcsDataGridOptions<AmcsDataGridRowData> extends GridOptions<AmcsDataGridRowData> {
  /**
   * The field used for displaying Errors
   */
  errorField?: string;
}

export class AmcsDataGridOptionsBuilder {
  static default: AmcsDataGridOptions<AmcsDataGridRowData> = {
    defaultColDef: {
      editable: true,
      sortable: true,
      filter: true,
      resizable: true,
    },
    errorField: 'errors',
  };

  static withCount: AmcsDataGridOptions<AmcsDataGridRowData> = Object.assign(
    {
      statusBar: {
        statusPanels: [
          {
            statusPanel: 'agTotalAndFilteredRowCountComponent',
            align: 'left',
          },
        ],
      },
    },
    this.default
  );
}

export interface AmcsDataGridRichSelectOptions {
  cellHeight: 20;
  values: any[];
  cellRenderer: ICellRendererAngularComp;
  formatValue: Function;
  searchDebounceDelay: number;
}

export interface AmcsDataGridSelectDefinition extends AmcsColDef {
  cellEditorParams: {
    values: any[];
  };
}

export interface AmcsDataGridLargeTextOptions {
  maxLength: number;
  rows: number;
  cols: number;
}

export class AmcsDataGridColumnEditorTypes {
  /**
   * https://www.ag-grid.com/angular-data-grid/provided-cell-editors/#text-cell-editor
   */
  static text = 'agTextCellEditor';

  /**
   * https://www.ag-grid.com/angular-data-grid/provided-cell-editors/#large-text-cell-editor
   */
  static largeText = 'agLargeTextCellEditor';

  /**
   * https://www.ag-grid.com/angular-data-grid/provided-cell-editors/#select-cell-editor
   */
  static select = 'agSelectCellEditor';

  /**
   * https://www.ag-grid.com/angular-data-grid/provided-cell-editors/#rich-select-cell-editor
   */
  static richSelect = 'agRichSelectCellEditor';

  static date = DateCellEditorComponent;

  static checkbox = CheckboxCellEditorComponent;

  static numeric = NumericCellEditorComponent;
}

export class AmcsDataGridColumnRenderers {
  static checkbox = CheckboxCellRendererComponent;
  static numeric = NumericCellRendererComponent;
  static date = DateCellRendererComponent;
  static validation = ValidationCellRendererComponent;
  static singleClickCheckbox = SingleClickCheckboxCellRendererComponent;
}
