import { FormControl, Validators } from '@angular/forms';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { BaseForm } from '@shared-module/forms/base-form.model';
import { StorybookAmcsFormModel } from './storybook-amcs-form-data.model';
const ALPHANUMERIC_PATTERN = /^[A-Za-z0-9 _\-.,:;()]*$/;

export class StoryBookAmcsForm extends BaseForm<StorybookAmcsFormModel, StoryBookAmcsForm> {
  id: FormControl;

  dropdownId: FormControl;

  numInput: FormControl;

  date: FormControl;

  someText: FormControl;

  table: FormControl;

  testId: FormControl;

  testName: FormControl;

  testIdTwo: FormControl;

  buildForm(dataModel: StorybookAmcsFormModel): StoryBookAmcsForm {
    const form = new StoryBookAmcsForm();
    form.id = new FormControl(dataModel.id);
    form.dropdownId = new FormControl(dataModel.dropdownId,Validators.required);
    form.numInput = new FormControl(dataModel.numInput, Validators.required);
    form.date = new FormControl(AmcsDate.create(), Validators.required);
    form.someText = new FormControl(dataModel.someText, [Validators.pattern(ALPHANUMERIC_PATTERN), Validators.required]);
    form.table = new FormControl(dataModel.table);
    form.testId = new FormControl(dataModel.testId);
    form.testName = new FormControl(dataModel.testName);
    form.testIdTwo = new FormControl(dataModel.testIdTwo);
    return form;
  }

  parseForm(typedForm: StoryBookAmcsForm): StorybookAmcsFormModel {
    const dataModel = new StorybookAmcsFormModel();
    dataModel.id = typedForm.id.value;
    dataModel.dropdownId = typedForm.dropdownId.value;
    dataModel.numInput = typedForm.numInput.value;
    dataModel.date = typedForm.date.value;
    dataModel.someText = typedForm.someText.value;
    dataModel.table = typedForm.table.value;
    dataModel.testId = typedForm.testId.value;
    dataModel.testName = typedForm.testName.value;
    dataModel.testIdTwo = typedForm.testIdTwo.value;

    return dataModel;
  }
}
