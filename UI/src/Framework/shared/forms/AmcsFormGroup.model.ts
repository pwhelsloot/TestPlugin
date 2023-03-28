import { FormGroup } from '@angular/forms';
/**
 * @deprecated Use BaseForm
 */
export class AmcsFormGroup {
    constructor(public formGroup: FormGroup) {}

    hasErrors(nameKey: string, errorKey?: string): boolean {
        let hasErrors = false;

        if (errorKey === null || errorKey === undefined) {
            hasErrors = this.formGroup.get(nameKey).invalid && this.formGroup.get(nameKey).touched;
        } else {
            hasErrors =
                this.formGroup.get(nameKey).invalid &&
                this.formGroup.get(nameKey).touched &&
                this.formGroup.get(nameKey).errors != null &&
                this.formGroup.get(nameKey).errors[errorKey];
        }
        return hasErrors;
    }

    hasGroupError(errorKey: string): boolean {
        return this.formGroup.invalid && this.formGroup.touched && this.formGroup.errors != null && this.formGroup.errors[errorKey];
    }

    checkIfValid() {
        this.markAsTouched(this.formGroup);
        return !this.formGroup.invalid;
    }

    markAsTouched(formGroup: FormGroup) {
        if (formGroup != null && formGroup.status !== 'DISABLED') {
            (Object as any).values(formGroup.controls).forEach((control) => {
                control.markAsTouched();
                if (control.controls) {
                    (Object as any).values(control.controls).forEach((c) => {
                        c.markAsTouched();
                        if (c.controls) {
                            this.markAsTouched(c);
                        }
                    });
                }
            });
        }
    }
}
