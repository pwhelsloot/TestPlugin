import { coerceBooleanProperty } from '@angular/cdk/coercion';
import { ColDef, ColGroupDef, ValueFormatterFunc } from 'ag-grid-enterprise';
import {
  AmcsDataGridColumnEditorTypes,
  AmcsDataGridColumnRenderers,
  AmcsDataGridLargeTextOptions,
  AmcsDataGridRichSelectOptions
} from './amcs-data-grid.options';
import { AmcsDateCellColumnHelper } from './cells/date/date-cell-column.helper';
import { DateCellEditorConfig } from './cells/date/date-cell-editor/date-celll-editor.config';

export type AmcsColDefs = (AmcsColDef | AmcsColGroupDef)[] | null | undefined;

export type AmcsColDef = ColDef;

export type AmcsColGroupDef = ColGroupDef;

export class AmcsDataGridColDefinitions {
  /**
   * Create Column definition for a date
   * @returns
   */
  static createDateColDef(definition: AmcsColDef = {}, config?: DateCellEditorConfig): AmcsColDef {
    const { cellClassRules, tooltipValueGetter } = AmcsDateCellColumnHelper.generateDateCellValidation(config, definition);

    return Object.assign(definition, {
      cellRenderer: AmcsDataGridColumnRenderers.date,
      cellEditor: AmcsDataGridColumnEditorTypes.date,
      cellEditorParams: {
        config,
      },
      cellClassRules,
      tooltipValueGetter,
    } as AmcsColDef);
  }

  /**
   * Create Column definition for a number
   * @returns
   */
  static createNumericColDef(definition: AmcsColDef = {}, precision = 4): AmcsColDef {
    return Object.assign(definition, {
      cellRenderer: AmcsDataGridColumnRenderers.numeric,
      cellEditorParams: {
        precision,
      },
      cellRendererParams: {
        precision,
      },
      cellEditor: AmcsDataGridColumnEditorTypes.numeric,
    } as AmcsColDef);
  }

  /**
   * Create Column definition for a select
   * @param values
   * @returns
   */
  static createSelectColDef(definition: AmcsColDef = {}, values: any[]): AmcsColDef {
    return Object.assign(definition, {
      cellEditor: AmcsDataGridColumnEditorTypes.select,
      cellEditorParams: {
        values,
      },
    } as AmcsColDef);
  }

  /**
   * Create Column definition for a normal text cell
   * @param valueFormatter
   * @param useFormatter
   * @returns
   */
  static createTextColDef(
    definition: AmcsColDef = {},
    valueFormatter?: string | ValueFormatterFunc<any>,
    useFormatter = false
  ): AmcsColDef {
    return Object.assign(definition, {
      cellEditor: AmcsDataGridColumnEditorTypes.text,
      valueFormatter,
      cellEditorParams: {
        useFormatter,
      },
    } as AmcsColDef);
  }

  /**
   * Create Column definition for a large text cell
   * @param valueFormatter
   * @param useFormatter
   * @returns
   */
  static createLargeTextColDef(
    definition: AmcsColDef = {},
    options = {
      maxLength: 100,
      rows: 10,
      cols: 50,
    } as AmcsDataGridLargeTextOptions
  ) {
    return Object.assign(definition, {
      cellEditor: AmcsDataGridColumnEditorTypes.largeText,
      cellEditorPopup: true,
      cellEditorParams: {
        maxLength: options.maxLength ?? 100,
        rows: options.rows ?? 10,
        cols: options.cols ?? 50,
      },
    });
  }

  static createRichSelectColDef(definition: AmcsColDef = {}, options: AmcsDataGridRichSelectOptions) {
    return Object.assign(definition, {
      cellEditor: AmcsDataGridColumnEditorTypes.richSelect,
      cellEditorPopup: true,
      cellEditorParams: {
        values: options.values,
        cellHeight: options.cellHeight,
        formatValue: options.formatValue,
        cellRenderer: options.cellRenderer,
        searchDebounceDelay: options.searchDebounceDelay,
      },
    });
  }

  /**
   * Create Columns definition for a checkbox cell
   */
  static createCheckboxColDef(definition: AmcsColDef = {}) {
    return Object.assign(definition, {
      valueParser: (params) => {
        return coerceBooleanProperty(params[params.colDef.field].value.toString().toLowerCase());
      },
      cellRenderer: AmcsDataGridColumnRenderers.checkbox,
      cellEditor: AmcsDataGridColumnEditorTypes.checkbox,
    } as AmcsColDef);
  }

  /**
   * Create Columns definition for a checkbox cell that can be selected with a single click
   */
  static createSingleClickCheckboxColDef(definition: AmcsColDef = {}, isEditable: boolean) {
    return Object.assign(definition, {
      editable: false, //In order to make a single click checkbox, we make uneditable here and set it if editable or not inside the component
      valueParser: (params) => {
        return coerceBooleanProperty(params[params.colDef.field].value.toString().toLowerCase());
      },
      cellRenderer: AmcsDataGridColumnRenderers.singleClickCheckbox,
      cellEditorParams: {
        isEditable,
      },
    } as AmcsColDef);
  }

  /**
   * Create Columns definition for a validation cell
   */
  static createValidationColDef(definition: AmcsColDef = {}) {
    return Object.assign(definition, {
      cellRenderer: AmcsDataGridColumnRenderers.validation,
    } as AmcsColDef);
  }
}
