import { EventEmitter, Injectable } from '@angular/core';
import { AmcsGridFormColumn } from '@shared-module/components/amcs-grid-form/amcs-grid-form-column';
import { AmcsGridFormConfig } from '@shared-module/components/amcs-grid-form/amcs-grid-form-config.model';
import { AmcsGridFormRow } from '@shared-module/components/amcs-grid-form/amcs-grid-form-row.model';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';

@Injectable()
export class AmcsGridFormService {
    rows: AmcsGridFormRow[];
    config: AmcsGridFormConfig;
    canEnableCheckboxSelectionCallback: boolean;
    onManualFocusError: EventEmitter<any>;
    onCheckboxSelection: EventEmitter<any>;
    createForm: (rowId: number) => AmcsFormGroup;

    actionRelativeSizePercentage: number;

    rowSelected(row: AmcsGridFormRow, selected: boolean) {
        if (selected) {
            row.isSelected = true;
            row.formItems.push(this.createBlankForm(row.id));
        } else {
            row.isSelected = false;
            row.formItems = [];
            this.setRowErrors();
        }
    }

    addForm(row: AmcsGridFormRow, index: number) {
        if (this.config.allowAdd) {
            if (!this.isAnyRowErrored()) {
                row.formItems.splice(index, 0, this.createBlankForm(row.id));
            }
        }
    }

    removeForm(row: AmcsGridFormRow, index: number) {
        if (this.config.allowDelete) {
            row.formItems.splice(index, 1);
            this.setRowErrors();
            if (row.formItems.length <= 0) {
                row.isSelected = false;
            }
        }
    }

    // We always have the following layout
    // Checkbox @ 4%, Description @ inputted %. Form @ Max % available, Action @ 4%.
    setColumnSizes(columns: AmcsGridFormColumn[]) {
        this.checkColumnSizesValid(columns);
        // 100% - Checkbox % - Description % - Action %;
        const formAvailablePercentage: number = 100 - 4 - this.config.rowDescriptionSize - 4;
        columns.forEach(element => {
            // Although the column might want 50%, thats not 50% of the entire row, it's 50% of the formAvailablePercentage.
            element.relativeWidthMultiplier = (formAvailablePercentage / 100);
            element.afterResize();
        });
        // The action column goes inside the form then there is > 1 row so we need it's relative % too.
        this.actionRelativeSizePercentage = (4 / ((96 - this.config.rowDescriptionSize) / 100));
    }

    // Prevents checkbox selection if rows not all valid
    checkCanSelectRow(row: AmcsGridFormRow, event: any) {
        if(this.canEnableCheckboxSelectionCallback) {
            this.onCheckboxSelection.emit({row, event});
        }

        if (!row.isSelected && this.isAnyRowErrored()) {
            event.preventDefault();
        }
    }

    getForms(): { valid: boolean; forms: AmcsFormGroup[] } {
        const valid = !this.isAnyRowErrored();
        const validForms: AmcsFormGroup[] = [];
        if (valid) {
            this.rows.forEach(row => {
                if (!row.isError) {
                    row.formItems.forEach(formItem => {
                        if (!formItem.formGroup.invalid) {
                            validForms.push(formItem);
                        }
                    });
                }
            });
        }
        return { valid, forms: validForms };
    }

    setDisabledState() {
        if (!this.config.allowEdit) {
            this.rows.forEach(row => {
                row.formItems.forEach(formItem => {
                    formItem.formGroup.disable();
                });
            });
        }
    }

    private isAnyRowErrored(): boolean {
        this.setRowErrors();
        return this.rows.some(row => row.isError);
    }

    private setRowErrors() {
        this.rows.forEach(row => {
            row.isError = row.isSelected && row.formItems.length > 0 && row.formItems.some(form => !form.checkIfValid());
        });
        this.onManualFocusError.next();
    }

    private createBlankForm(rowId: number): AmcsFormGroup {
        return this.createForm(rowId);
    }

    private checkColumnSizesValid(columns: AmcsGridFormColumn[]) {
        if (columns.map(c => c.widthPercentage).reduce((sum, current) => sum + current) !== 100) {
            throw new Error(('Columns sizes do not total 100%'));
        }
    }
}
