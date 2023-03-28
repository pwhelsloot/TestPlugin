import { FormControl, FormGroup } from '@angular/forms';
import { Subject } from 'rxjs';
import { isTruthy } from '../../core/helpers/is-truthy.function';
import { AmcsFormValidationHelper } from './amcs-form-validation-helper';

export abstract class BaseFormGroup {
    htmlFormGroup: FormGroup;
    isValid = true;
    manualFocusOnErrorSubject = new Subject();

    checkIfValid() {
        this.isValid = true;
        this.markAsTouched(this.htmlFormGroup);
        this.manualFocusOnErrorSubject.next();
        return !this.htmlFormGroup.invalid && this.isValid;
    }

    hasErrors(control: FormControl, errorKey?: string): boolean {
        let hasErrors = false;

        if (errorKey === null || errorKey === undefined) {
            hasErrors = AmcsFormValidationHelper.formControlHasError(control);
        } else {
            hasErrors = control.invalid && control.touched && control.errors != null && control.errors[errorKey];
        }
        return hasErrors;
    }

    hasGroupError(errorKey: string): boolean {
        return (
            this.htmlFormGroup.invalid &&
            this.htmlFormGroup.touched &&
            this.htmlFormGroup.errors != null &&
            this.htmlFormGroup.errors[errorKey]
        );
    }

    getPrimaryKeyControl(): FormControl {
        if (isTruthy(this.htmlFormGroup.controls.id)) {
            return this.htmlFormGroup.controls.id as FormControl;
        }
        throw new Error('No PrimaryKeyControl found, please set one for ' + this.constructor.name);
    }

    private markAsTouched(formGroup: FormGroup) {
        if (formGroup != null && formGroup.status !== 'DISABLED') {
            (Object as any).values(formGroup.controls).forEach((control) => {
                control.markAsTouched();
                this.disabledControlStatus(control);
                if (control.controls) {
                    (Object as any).values(control.controls).forEach((c) => {
                        c.markAsTouched();
                        this.disabledControlStatus(control);
                        if (c.controls) {
                            this.markAsTouched(c);
                            this.disabledControlStatus(control);
                        }
                    });
                }
            });
        }
    }

    private disabledControlStatus(control: any) {
        if (control.status === 'DISABLED' && !control.value && control.touched && isTruthy(control.errors) && control.errors.required) {
            this.isValid = false;
        }
    }
}
