import { ValidatorFn } from '@angular/forms';

/**
 * @deprecated Move to PlatformUI
 */
export class FormControlTableItem {
    label: string;
    formControlName: string;
    forcedMandatory: boolean;
    validators: ValidatorFn[];
}
