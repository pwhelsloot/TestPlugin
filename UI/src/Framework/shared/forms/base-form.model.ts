
import { IBaseForm } from './base-form.interface';
import { BaseFormGroup } from './base-form-group.model';

export abstract class BaseForm<D, F> extends BaseFormGroup implements IBaseForm<D, F> {
    abstract buildForm(dataModel: D, ...extraParams: any[]): F;
    abstract parseForm(typedForm: F, ...extraParams: any[]): D;
    afterFormBuilt?(): void;
}
