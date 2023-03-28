import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { BaseFormGroup } from '@shared-module/forms/base-form-group.model';
import { BehaviorSubject, Observable } from 'rxjs';

export interface IAmcsStepService {
    form: AmcsFormGroup | BaseFormGroup;
    initialised: BehaviorSubject<boolean>;
    checkValidation(isCheckFromLinkedStep?: boolean): Observable<boolean>;
    initialise();
}
