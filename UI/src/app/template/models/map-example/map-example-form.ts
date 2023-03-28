import { FormControl, Validators } from '@angular/forms';
import { BaseForm } from '@shared-module/forms/base-form.model';
import { MapExample } from './map-example.model';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsValidators } from '@shared-module/validators/AmcsValidators.model';

export class MapExampleForm extends BaseForm<MapExample, MapExampleForm> {
  mapExampleId: FormControl;
  description: FormControl;
  latitude: FormControl;
  longitude: FormControl;

  buildForm(dataModel: MapExample): MapExampleForm {
    const form = new MapExampleForm();
    form.description = new FormControl(dataModel.description, {
      validators: [Validators.required],
    });

    form.latitude = new FormControl(isTruthy(dataModel.latitude) ? dataModel.latitude.toString() : dataModel.latitude, {
      validators: [Validators.maxLength(22), AmcsValidators.validCoordinate('latitude'), Validators.required],
      updateOn: 'blur',
    });

    form.longitude = new FormControl(isTruthy(dataModel.longitude) ? dataModel.longitude.toString() : dataModel.longitude, {
      validators: [Validators.maxLength(22), AmcsValidators.validCoordinate('longitude'), Validators.required],
      updateOn: 'blur',
    });

    form.mapExampleId = new FormControl(dataModel.mapExampleId);
    return form;
  }

  parseForm(typedForm: MapExampleForm): MapExample {
    const dataModel = new MapExample();
    dataModel.mapExampleId = typedForm.mapExampleId.value;
    dataModel.description = typedForm.description.value;
    dataModel.latitude = +typedForm.latitude.value;
    dataModel.longitude = +typedForm.longitude.value;
    return dataModel;
  }
}
