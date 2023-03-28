import { FormControl, Validators } from '@angular/forms';
import { BaseForm } from '@shared-module/forms/base-form.model';
import { SBExampleFormModal } from './storybook-example-form-data.model';

export class SBExampleModalForm extends BaseForm<SBExampleFormModal, SBExampleModalForm> {

    id: FormControl;

    testId: FormControl;

    testIdTwo: FormControl;

    testName: FormControl;

    buildForm(dataModel: SBExampleFormModal): SBExampleModalForm {
        const form = new SBExampleModalForm();
        form.id = new FormControl(dataModel.id);
        form.testName = new FormControl(dataModel.testName, Validators.required);
        form.testId = new FormControl(dataModel.testId, Validators.required);
        form.testIdTwo = new FormControl(dataModel.testIdTwo);
        return form;
    }

    parseForm(typedForm: SBExampleModalForm): SBExampleFormModal {
        const dataModel = new SBExampleFormModal();
        dataModel.id = typedForm.id.value;
        dataModel.testName = typedForm.testName.value;
        dataModel.testId = typedForm.testId.value;
        dataModel.testIdTwo = typedForm.testIdTwo.value;
        return dataModel;
    }

}
