import { FormControl, Validators } from '@angular/forms';
import { BaseForm } from '@shared-module/forms/base-form.model';
import { ComponentFilter } from './component-filter.model';

export class ComponentFilterForm extends BaseForm<ComponentFilter, ComponentFilterForm> {

  property: FormControl;

  filterType: FormControl;

  filterTextValue: FormControl;

  filterNumberValue: FormControl;

  filterDateValue: FormControl;

  filterBooleanValue: FormControl;

  filterEnumValue: FormControl;

  buildForm(dataModel: ComponentFilter): ComponentFilterForm {
    const form = new ComponentFilterForm();
    form.property = new FormControl(dataModel.propertyId, { validators: Validators.required, updateOn: 'blur' });
    form.filterType = new FormControl(dataModel.filterTypeId, { validators: Validators.required, updateOn: 'blur' });
    form.filterTextValue = new FormControl(dataModel.filterTextValue);
    form.filterNumberValue = new FormControl(dataModel.filterNumberValue);
    form.filterDateValue = new FormControl(dataModel.filterDateValue);
    form.filterBooleanValue = new FormControl(dataModel.filterBooleanValue);
    form.filterEnumValue = new FormControl(dataModel.filterEnumValue);
    return form;
  }

  parseForm(typedForm: ComponentFilterForm): ComponentFilter {
    const dataModel = new ComponentFilter();
    dataModel.propertyId = typedForm.property.value;
    dataModel.filterTypeId = typedForm.filterType.value;
    dataModel.filterTextValue = typedForm.filterTextValue.value;
    dataModel.filterNumberValue = typedForm.filterNumberValue.value;
    dataModel.filterDateValue = typedForm.filterDateValue.value;
    dataModel.filterBooleanValue = typedForm.filterBooleanValue.value;
    dataModel.filterEnumValue = typedForm.filterEnumValue.value;
    return dataModel;
  }

}
