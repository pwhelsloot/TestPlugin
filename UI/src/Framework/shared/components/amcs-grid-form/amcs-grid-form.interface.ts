import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { AmcsGridFormColumn } from './amcs-grid-form-column';

export class IAmcsGridForm {
    form: AmcsFormGroup;
    columns: AmcsGridFormColumn[];
}
