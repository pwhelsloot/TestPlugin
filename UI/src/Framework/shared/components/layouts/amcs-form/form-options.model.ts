import { FormDisplayModeEnum } from '@core-module/models/forms/form-display-model.enum';
import { FormActionOptions } from '../amcs-form-actions/form-action-options.model';

export class FormOptions {
    actionOptions: FormActionOptions = new FormActionOptions();
    // For loading animation
    loadingContainerSize = 120;
    enableFormActions = true;
    displayMode = FormDisplayModeEnum.Standard;
}
