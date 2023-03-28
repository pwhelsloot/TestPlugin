import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { AmcsDataInputGridColumn } from './amcs-data-input-grid-column';

export class IAmcsDataInputGrid {
    form: AmcsFormGroup;
    columns: AmcsDataInputGridColumn[];
}
