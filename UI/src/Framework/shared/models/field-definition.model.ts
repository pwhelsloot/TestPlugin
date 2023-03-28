import { ValidatorFn } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';

/**
 * @deprecated Move to PlatformUI
 */
export class FieldDefinition {
    label: string;
    key: string;
    validators: ValidatorFn[];
    mandatoryValue: boolean;
    editableValue: boolean;
    visibleValue: boolean;
    forceMandatory: boolean;
    forceVisible: boolean;

    constructor(label: string, key: string, validators?: ValidatorFn[], mandatoryValue?: boolean, editableValue?: boolean, visibleValue?: boolean,
        forceMandatory?: boolean, forceVisible?: boolean) {
        this.label = label;
        this.key = key;
        this.validators = isTruthy(validators) ? validators : null;
        this.mandatoryValue = isTruthy(mandatoryValue) ? mandatoryValue : false;
        this.editableValue = isTruthy(editableValue) ? editableValue : true;
        this.visibleValue = isTruthy(visibleValue) ? visibleValue : true;
        this.forceMandatory = isTruthy(forceMandatory) ? forceMandatory : false;
        this.forceVisible = isTruthy(forceVisible) ? forceVisible  : false;
    }
}
