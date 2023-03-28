import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { FormBuilder } from '@angular/forms';
import { BaseForm } from './base-form.model';

export class AmcsFormBuilder {
    static buildForm<D, F extends BaseForm<D, F>>(formBuilder: FormBuilder, dataModel: D, type: (new () => BaseForm<D, F>), ...extraParams: any[]): F {
        const instance: BaseForm<D, F> = new type();
        const form = instance.buildForm(dataModel, extraParams);
        form.htmlFormGroup = formBuilder.group(form);
        if (isTruthy(form.afterFormBuilt)) {
            form.afterFormBuilt();
        }
        return form;
    }

    static parseForm<D, F extends BaseForm<D, F>>(typedForm: F, type: (new () => BaseForm<D, F>), ...extraParams: any[]): D {
        const form: BaseForm<D, F> = new type();
        return form.parseForm(typedForm, extraParams);
    }
}
