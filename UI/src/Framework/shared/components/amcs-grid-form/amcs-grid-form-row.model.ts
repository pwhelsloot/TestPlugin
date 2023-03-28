import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';

export class AmcsGridFormRow {
    isSelected = false;
    isError = false;
    constructor(public id: number, public description: string, public formItems?: AmcsFormGroup[]) {
        if (!isTruthy(this.formItems)) {
            this.formItems = [];
        } else if (this.formItems.length > 0) {
            this.isSelected = true;
        }
    }
}
