import { BaseForm } from '@shared-module/forms/base-form.model';
import { Ingredient } from './template-ingredient.model';
import { FormControl, Validators } from '@angular/forms';

export class TemplateIngredientForm extends BaseForm<Ingredient, TemplateIngredientForm> {
  name: FormControl;

  measurementId: FormControl;

  measurement: FormControl;

  typeId: FormControl;

  type: FormControl;

  optional: FormControl;

  id: number;

  buildForm(dataModel: Ingredient): TemplateIngredientForm {
    const form = new TemplateIngredientForm();
    form.id = dataModel.ingredientId;
    form.name = new FormControl(dataModel.name, { validators: [Validators.required, Validators.maxLength(50)], updateOn: 'blur' });
    form.measurementId = new FormControl(dataModel.measurementId);
    form.measurement = new FormControl(dataModel.measurement);
    form.typeId = new FormControl(dataModel.typeId);
    form.type = new FormControl(dataModel.type);
    form.optional = new FormControl(dataModel.optional);
    return form;
  }

  parseForm(typedForm: TemplateIngredientForm): Ingredient {
    const dataModel = new Ingredient();
    dataModel.ingredientId = typedForm.id;
    dataModel.name = typedForm.name.value;
    dataModel.measurementId = typedForm.measurementId.value;
    dataModel.measurement = typedForm.measurement.value;
    dataModel.typeId = typedForm.typeId.value;
    dataModel.type = typedForm.type.value;
    dataModel.optional = typedForm.optional.value;
    return dataModel;
  }
}
