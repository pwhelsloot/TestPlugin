import { FormGroup } from '@angular/forms';
import { BaseService } from '@coreservices/base.service';

export abstract class AmcsFormService<T> extends BaseService {
    protected abstract buildForm();

    protected abstract populateForm(data: T, translations?: string[]);

    protected abstract save();

    protected abstract return();

    protected markFormGroupTouched(formGroup: FormGroup) {
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

    protected cancel() {
        this.return();
    }
}
