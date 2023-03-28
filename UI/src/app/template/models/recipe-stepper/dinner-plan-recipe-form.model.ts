import { BaseForm } from '@shared-module/forms/base-form.model';
import { FormControl, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { DinnerPlanRecipe } from './dinner-plan-recipe.model';
import { AmcsValidators } from '@shared-module/validators/AmcsValidators.model';

export class DinnerPlanRecipeForm extends BaseForm<DinnerPlanRecipe, DinnerPlanRecipeForm> {
  dinnerPlan: FormControl;

  course: FormControl;

  recipe: FormControl;

  preparationDate: FormControl;

  previewPreparationDateChangeSubject = new Subject<Date>();

  buildForm(dataModel: DinnerPlanRecipe, extraParams: any[]): DinnerPlanRecipeForm {
    const form: DinnerPlanRecipeForm = new DinnerPlanRecipeForm();
    const minDate: Date = extraParams[0];
    form.dinnerPlan = new FormControl(dataModel.dinnerPlanId);
    form.course = new FormControl(dataModel.courseId);
    form.recipe = new FormControl(dataModel.recipeId, Validators.required);
    form.preparationDate = new FormControl(dataModel.preparationDate, AmcsValidators.dateMin(minDate));
    return form;
  }

  parseForm(typedForm: DinnerPlanRecipeForm): DinnerPlanRecipe {
    const dataModel: DinnerPlanRecipe = new DinnerPlanRecipe();
    dataModel.dinnerPlanId = typedForm.dinnerPlan.value;
    dataModel.courseId = typedForm.course.value;
    dataModel.recipeId = typedForm.recipe.value;
    dataModel.preparationDate = typedForm.preparationDate.value;
    return dataModel;
  }
}
