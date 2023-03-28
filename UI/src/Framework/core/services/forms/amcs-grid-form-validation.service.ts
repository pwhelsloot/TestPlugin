import { Injectable } from '@angular/core';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { Observable, Subject } from 'rxjs';

@Injectable()
export class AmcsGridFormValidationService {
    validForms$: Observable<{ valid: boolean; forms: AmcsFormGroup[] }>;
    readySubject = new Subject();
}
