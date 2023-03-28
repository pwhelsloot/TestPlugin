import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { Observable, Subject } from 'rxjs';

export class AmcsDataInputGridRow {

    isSelectedSubject = new Subject<boolean>();
    isSelected$: Observable<boolean>;

    isError = false;
    isCheckboxDisabled = false;

    get isSelected() {
      return this._isSelected;
    }

    set isSelected(value: boolean) {
      this._isSelected = value;
      this.isSelectedSubject.next(value);
    }

    constructor(public id: number, public item: any, public formItems?: AmcsFormGroup) {
      if (!isTruthy(this.formItems)) {
        this.formItems = null;
      }
      this.isSelected$ = this.isSelectedSubject.asObservable();
    }

    private _isSelected: boolean;
}
