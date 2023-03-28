import { FormControl, Validators } from '@angular/forms';
import { BaseForm } from '@shared-module/forms/base-form.model';
import { DinnerPlan } from './dinner-plan.model';

export class DinnerPlanForm extends BaseForm<DinnerPlan, DinnerPlanForm> {
  id: FormControl;

  name: FormControl;

  difficultyLevel: FormControl;

  estimatedTime: FormControl;

  courses: FormControl;

  buildForm(dataModel: DinnerPlan): DinnerPlanForm {
    const form: DinnerPlanForm = new DinnerPlanForm();
    form.id = new FormControl(dataModel.id);
    form.name = new FormControl(dataModel.name, Validators.required);
    form.difficultyLevel = new FormControl(dataModel.difficultyLevelId, Validators.required);
    form.estimatedTime = new FormControl(dataModel.estimatedTime);
    form.courses = new FormControl(dataModel.courseIds, [Validators.required, Validators.maxLength(3)]);
    return form;
  }

  parseForm(typedForm: DinnerPlanForm): DinnerPlan {
    const dataModel: DinnerPlan = new DinnerPlan();
    dataModel.id = typedForm.id.value;
    dataModel.name = typedForm.name.value;
    dataModel.estimatedTime = typedForm.estimatedTime.value;
    dataModel.difficultyLevelId = typedForm.difficultyLevel.value;
    dataModel.courseIds = typedForm.courses.value;
    return dataModel;
  }
}
