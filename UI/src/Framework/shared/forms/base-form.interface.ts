import { FormGroup } from '@angular/forms';

export interface IBaseForm<D, F> {
    htmlFormGroup: FormGroup;
    buildForm(dataModel: D, ...extraParams: any[]): F;
    parseForm(typedForm: F, ...extraParams: any[]): D;
}
