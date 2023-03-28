import { EventEmitter, Injectable } from '@angular/core';
import { IGridFormSimpleRowForm } from '@shared-module/components/amcs-grid-form-simple/amcs-grid-form-simple-row-form.interface';
import { AmcsGridFormSimpleRow } from '@shared-module/components/amcs-grid-form-simple/amcs-grid-form-simple-row.model';
import { AmcsGridFormColumn } from '@shared-module/components/amcs-grid-form/amcs-grid-form-column';
import { AmcsGridFormConfig } from '@shared-module/components/amcs-grid-form/amcs-grid-form-config.model';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { isTruthy } from '../../helpers/is-truthy.function';

@Injectable()
export class AmcsGridFormSimpleService {
  rows: AmcsGridFormSimpleRow[];
  initialRows: AmcsGridFormSimpleRow[] = [];
  config: AmcsGridFormConfig;
  onManualFocusError: EventEmitter<any>;
  createForm: (rowId: number) => AmcsFormGroup;
  clearForm: (form: IGridFormSimpleRowForm) => void;
  deleteRow: (form: AmcsGridFormSimpleRow) => void;

  // We always have the following layout
  // Description @ inputted %. Form @ Max % available
  setColumnSizes(columns: AmcsGridFormColumn[], clearEnabled: boolean, deleteEnabled: boolean, actionColumnEnabled: boolean, scrollSize: number) {
    this.checkColumnSizesValid(columns);
    if (isTruthy(this.config)) {
      // 100% - Description % - 2% (for Clear icon);
      let formAvailablePercentage: number = (100 - this.config.rowDescriptionSize - scrollSize);
      if (clearEnabled) {
        formAvailablePercentage -= 2;
      }
      if (deleteEnabled) {
        formAvailablePercentage -= 2;
      }
      if (actionColumnEnabled) {
        formAvailablePercentage -= 3;
      }
      columns.forEach(element => {
        // Although the column might want 50%, thats not 50% of the entire row, it's 50% of the formAvailablePercentage.
        element.relativeWidthMultiplier = (formAvailablePercentage / 100);
        element.afterResize();
      });
    }
  }

  getForms(): { valid: boolean; forms: IGridFormSimpleRowForm[] } {
    const valid = !this.isAnyRowErrored();
    const validForms: IGridFormSimpleRowForm[] = [];
    if (valid) {
      this.rows.forEach(row => {
        if (!row.isError) {
          if (!row.form.htmlFormGroup.invalid) {
            validForms.push(row.form);
          }
        }
      });
    }
    return { valid, forms: validForms };
  }

  setDisabledState() {
    if (!this.config.allowEdit) {
      this.rows.forEach(row => {
        row.form.htmlFormGroup.disable();
      });
    }
  }

  clear(row: AmcsGridFormSimpleRow) {
    this.clearRowForm(row.form);
  }

  delete(row: AmcsGridFormSimpleRow) {
    this.deleteRow(row);
  }

  private isAnyRowErrored(): boolean {
    this.setRowErrors();
    return this.rows.some(row => row.isError);
  }

  private clearRowForm(form: IGridFormSimpleRowForm) {
    this.clearForm(form);
  }

  private setRowErrors() {
    this.rows.forEach(row => {
      row.isError = !row.form.htmlFormGroup.valid;
    });
    this.onManualFocusError.next();
  }

  private checkColumnSizesValid(columns: AmcsGridFormColumn[]) {
    if (columns.map(c => c.widthPercentage).reduce((sum, current) => sum + current) > 100) {
      throw new Error(('Columns sizes exceed 100%'));
    }
  }
}
