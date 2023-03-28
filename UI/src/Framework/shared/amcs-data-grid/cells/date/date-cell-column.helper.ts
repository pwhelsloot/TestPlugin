import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { AmcsColDef } from '@shared-module/amcs-data-grid/amcs-column-definitions';
import { ITooltipParams } from 'ag-grid-community';
import { DateCellEditorConfig } from './date-cell-editor/date-celll-editor.config';

/**
 * Helper for creating a Data grid Date column
 */
export class AmcsDateCellColumnHelper {
  /**
   * Check if the field is between the min and max date
   * @param field Input date field
   * @param min the minimum date
   * @param max the maximum date
   * @returns true if the field is between the min and max date's
   */
  static isInBetween(field: Date | string, min: Date | string, max: Date | string) {
    const dateField = this.convert(field);
    return dateField >= this.convert(min) && dateField <= this.convert(max);
  }

  /**
   * Check if the field is greater than the min date
   * @param field Input date field
   * @param min the minimum date
   * @returns
   */
  static isGreaterThan(field: Date | string, min: Date | string) {
    return this.convert(field) >= this.convert(min);
  }

  static convert(field: Date | string) {
    return AmcsDate.createFrom(field);
  }

  /**
   * Generate the Tooltip and CSS for a given config
   * @param config
   * @param definition
   * @returns
   */
  static generateDateCellValidation(config: DateCellEditorConfig, definition: AmcsColDef) {
    let tooltipValueGetter: (params: ITooltipParams) => {};
    let cellClassRules: {};
    if (config?.range) {
      if (config.range.min && config.range.max) {
        tooltipValueGetter = (params: ITooltipParams) => {
          if (!AmcsDateCellColumnHelper.Between(params, definition, config)) {
            return config.error
              .replace('{0}', definition.headerName)
              .replace('{1}', params.data[config.range.min].toLocaleDateString())
              .replace('{2}', params.data[config.range.max].toLocaleDateString());
          }
          return null;
        };
        cellClassRules = {
          'date-cell-invalid-date': (params: ITooltipParams) => {
            return !AmcsDateCellColumnHelper.Between(params, definition, config);
          },
        };
      } else if (config.range.min) {
        tooltipValueGetter = (params: ITooltipParams) => {
          if (!AmcsDateCellColumnHelper.GreaterThan(params, definition, config)) {
            return config.error.replace('{0}', definition.headerName).replace('{1}', params.data[config.range.min].toLocaleDateString());
          }
          return null;
        };
        cellClassRules = {
          'date-cell-invalid-date': (params: ITooltipParams) => {
            return !AmcsDateCellColumnHelper.GreaterThan(params, definition, config);
          },
        };
      }
    }
    return { cellClassRules, tooltipValueGetter };
  }

  private static Between(params: ITooltipParams<any, any>, definition: AmcsColDef, config: DateCellEditorConfig) {
    if (!this.IsValidationRequired(params, definition, config)) {
      return true;
    }
    return this.isInBetween(params.data[definition.field], params.data[config.range.min], params.data[config.range.max]);
  }

  private static GreaterThan(params: ITooltipParams<any, any>, definition: AmcsColDef, config: DateCellEditorConfig) {
    if (!this.IsValidationRequired(params, definition, config)) {
      return true;
    }
    return this.isGreaterThan(params.data[definition.field], params.data[config.range.min]);
  }

  private static IsValidationRequired(params: ITooltipParams<any, any>, definition: AmcsColDef, config: DateCellEditorConfig) {
    return config.isRequired || params.data[definition.field];
  }
}
