import { EventEmitter, Injectable } from '@angular/core';
import { AmcsDataInputGridColumn } from '@shared-module/components/amcs-data-input-grid/amcs-data-input-grid-column';
import { AmcsDataInputGridConfig } from '@shared-module/components/amcs-data-input-grid/amcs-data-input-grid-config.model';
import { AmcsDataInputGridRow } from '@shared-module/components/amcs-data-input-grid/amcs-data-input-grid-row.model';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';

@Injectable()
export class AmcsDataInputGridService {
    rows: AmcsDataInputGridRow[];
    config: AmcsDataInputGridConfig;
    onManualFocusError: EventEmitter<any>;
    createForm: (rowId: number) => AmcsFormGroup;

    // We always have the following layout
    // Checkbox @ checkBoxSizePercent%, @ inputted %. Form @ Max % available.
    setColumnSizes(columns: AmcsDataInputGridColumn[], checkBoxSizePercent: string) {
        this.checkColumnSizesValid(columns);

        const formAvailablePercentage: number = 100 - Number(checkBoxSizePercent);
        columns.forEach(element => {
            element.sizePercentage = element.size.toString() + '%';
            // Although the column might want 50%, thats not 50% of the entire row, it's 50% of the formAvailablePercentage.
            element.relativeSizePercentage = ((formAvailablePercentage / 100) * element.size).toString() + '%';
        });
    }

    rowSelected(row: AmcsDataInputGridRow, selected: boolean) {
        if (selected) {
            row.isSelected = true;
            row.formItems = this.createBlankForm(row.id);
        } else {
            row.isSelected = false;
            setTimeout(() => this.setRowErrors(), 0);
        }
    }

    // Prevents checkbox selection if rows not all valid
    checkCanSelectRow(row: AmcsDataInputGridRow, event: any) {
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
                    if (!row.formItems.formGroup.invalid) {
                        validForms.push(row.formItems);
                    }
                }
            });
        }
        return { valid, forms: validForms };
    }

    setDisabledState() {
        if (!this.config.allowEdit) {
            this.rows.forEach(row => {
                row.formItems.formGroup.disable();
            });
        }
    }

    private createBlankForm(rowId: number): AmcsFormGroup {
        return this.createForm(rowId);
    }

    private isAnyRowErrored(): boolean {
        this.setRowErrors();
        return this.rows.some(row => row.isError);
    }

    private setRowErrors() {
        this.rows.forEach(row => {
            row.isError = row.isSelected && !row.formItems.checkIfValid();
        });
        this.onManualFocusError.next();
    }
    private checkColumnSizesValid(columns: AmcsDataInputGridColumn[]) {
        if (columns.map(c => c.size).reduce((sum, current) => sum + current) !== 100) {
            throw new Error(('Columns sizes do not total 100%'));
        }
    }
}
