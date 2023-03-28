import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { BaseFormGroup } from '@shared-module/forms/base-form-group.model';
import { BehaviorSubject, Observable } from 'rxjs';

export interface IAmcsTabService {
    form: AmcsFormGroup | BaseFormGroup;
    initialised: BehaviorSubject<boolean>;
    checkValidation(): Observable<boolean>;
    initialise();
}
