import { FormGroup } from '@angular/forms';

export class FormHelper {
    static markFormGroupTouched(formGroup) {
        (Object as any).values(formGroup.controls).forEach((group: FormGroup) => {
            group.markAsTouched();

            if (group.controls) {
                (Object as any).values(group.controls).forEach((innerGroup: FormGroup) => {
                    innerGroup.markAsTouched();
                    if (innerGroup.controls) {
                        this.markFormGroupTouched(innerGroup);
                    }
                });
            }
        });
    }
}
