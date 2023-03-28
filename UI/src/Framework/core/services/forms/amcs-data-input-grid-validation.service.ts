import { Injectable } from '@angular/core';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { Observable, Subject } from 'rxjs';

@Injectable()
export class AmcsDataInputGridValidationService {
    validForms$: Observable<{ valid: boolean; forms: AmcsFormGroup[] }>;
    readySubject = new Subject();
}
