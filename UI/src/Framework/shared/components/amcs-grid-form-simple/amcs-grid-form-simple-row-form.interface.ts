import { FormGroup, FormControl } from '@angular/forms';

export interface IGridFormSimpleRowForm {
    htmlFormGroup: FormGroup;
    hasErrors(control: FormControl, errorKey?: string): boolean;
    hasGroupError(errorKey: string): boolean;
    checkIfValid();
}
