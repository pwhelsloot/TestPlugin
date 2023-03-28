import { FormControl, Validators } from '@angular/forms';
import { BaseForm } from '@shared-module/forms/base-form.model';
import { PersonalAccessToken } from './personal-access-token.model';

export class PersonalAccessTokenForm extends BaseForm<PersonalAccessToken, PersonalAccessTokenForm> {

    description: FormControl;

    sysUserId: FormControl;

    expires: FormControl;

    buildForm(): PersonalAccessTokenForm {
        const form = new PersonalAccessTokenForm();
        form.description = new FormControl(null, [Validators.required, Validators.maxLength(60)]);
        form.sysUserId = new FormControl(null, [Validators.required]);
        form.expires = new FormControl(null, [Validators.required]);
        return form;
    }

    parseForm(typedForm: PersonalAccessTokenForm): PersonalAccessToken {
        const token = new PersonalAccessToken();
        token.description = typedForm.description.value;
        token.sysUserId = typedForm.sysUserId.value;
        token.expires = typedForm.expires.value;
        return token;
    }
}
