import { FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { FormControlTableItem } from '@coremodels/form-control-table-item';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { FieldDefinition } from '@shared-module/models/field-definition.model';
import { FieldDefinitionDataModel } from '../models/field-definition-data.model';
import { BaseFormGroup } from './base-form-group.model';

/**
 * @deprecated Move to PlatformUI
 */
export class FieldDefinitionFormGroup {

    constructor(public formGroup: FormGroup, public items: FormControlTableItem[], public showVisible: boolean) {

    }

    hasErrors(nameKey: string, errorKey?: string): boolean {
        if (errorKey == null) {
            return !this.formGroup.get(nameKey).valid
                && this.formGroup.get(nameKey).touched;
        } else {
            return !this.formGroup.get(nameKey).valid
                && this.formGroup.get(nameKey).touched
                && this.formGroup.get(nameKey).errors != null
                && this.formGroup.get(nameKey).errors[errorKey];
        }
    }

    setValidators(parentForm: AmcsFormGroup | BaseFormGroup) {
        this.items.forEach(element => {
            let isMandatory: boolean = this.formGroup.get(FieldDefinitionFormGroup.getMandatoryControlName(element.formControlName)).value;
            const isEditable: boolean = this.formGroup.get(FieldDefinitionFormGroup.getEditableControlName(element.formControlName)).value;
            let isVisible: boolean = this.formGroup.get(FieldDefinitionFormGroup.getVisibleControlName(element.formControlName)).value;

            // Angular doesnt return values from disabled controls but if disabled we know its mandatory/visible
            isMandatory = isMandatory !== undefined && isMandatory !== null ? isMandatory : true;
            isVisible = isVisible !== undefined && isVisible !== null ? isVisible : true;

            const formGroup: FormGroup = this.getFormGroup(parentForm);

            if (isMandatory && !isEditable) {
                // Some controls might already have validators and angular only allows you to set them so we need
                // to take a slice of its standard ones and push on required
                const validators = element.validators.slice();
                validators.push(Validators.required);
                formGroup.get(element.formControlName).setValidators(validators);
            } else {
                formGroup.get(element.formControlName).clearValidators();
                if (element.validators.length > 0) {
                    formGroup.get(element.formControlName).setValidators(element.validators);
                }
            }
            formGroup.get(element.formControlName).updateValueAndValidity({ emitEvent: false });
        });
    }

    mapFormGroupToFieldDefinitions(formGroupValue: any): { key: string; isMandatory: boolean; isEditable: boolean; isVisible: boolean }[] {
        const fieldDefs = [];

        this.items.forEach(element => {
            const mandatoryControlName = FieldDefinitionFormGroup.getMandatoryControlName(element.formControlName);
            const editableControlName = FieldDefinitionFormGroup.getEditableControlName(element.formControlName);
            const visibleControlName = FieldDefinitionFormGroup.getVisibleControlName(element.formControlName);

            let isMandatory: boolean;
            let isEditable: boolean;
            let isVisible: boolean;

            if (element.forcedMandatory) {
                // If forced mandatory, set mandatory/visible to true
                isMandatory = true;
                isEditable = formGroupValue.hasOwnProperty(editableControlName) ? formGroupValue[editableControlName] : true;
                isVisible = true;
            } else {
                isVisible = formGroupValue.hasOwnProperty(visibleControlName) ? formGroupValue[visibleControlName] : true;

                if (!isVisible) {
                    // If not visible, set mandatory/editable to false
                    isMandatory = false;
                    isEditable = false;
                } else {
                    isMandatory = formGroupValue.hasOwnProperty(mandatoryControlName) ? formGroupValue[mandatoryControlName] : true;
                    isEditable = formGroupValue.hasOwnProperty(editableControlName) ? formGroupValue[editableControlName] : true;
                }
            }

            fieldDefs.push({ key: element.formControlName, isMandatory, isEditable, isVisible });
        });

        return fieldDefs;
    }

    getTypedFieldDefinitions(): FieldDefinitionDataModel[] {
        return this.mapFormGroupToFieldDefinitions(this.formGroup.value).map(x => this.mapToTypedFieldDefinitionDataModel(x));
    }

    private mapToTypedFieldDefinitionDataModel(definition: { key: string; isMandatory: boolean; isEditable: boolean; isVisible: boolean }) {
        const fieldDef = new FieldDefinitionDataModel();
        fieldDef.key = definition.key;
        fieldDef.isEditable = definition.isEditable;
        fieldDef.isVisible = definition.isVisible;
        fieldDef.isMandatory = definition.isMandatory;
        return fieldDef;
    }

    static addControl(
        value: any,
        key: string,
        fieldDefinitions: { key: string; isMandatory: boolean; isEditable: boolean }[],
        existingValidators?: ValidatorFn[],
        updateOn?: 'change' | 'blur' | 'submit'): FormControl {

        if (updateOn === null) {
            updateOn = 'blur';
        }

        const field = fieldDefinitions.find(t => t.key === key);
        const validators: ValidatorFn[] = [];
        if (field && field.isMandatory) {
            validators.push(Validators.required);
        }
        if (validators.length < 1) {
            validators.push(Validators.nullValidator);
        }
        if (isTruthy(existingValidators)) {
            validators.push(...existingValidators);
        }

        // 'field' could be 'undefined' when using an old template which, for example, doesn't have the 'local authority' field
        const control = new FormControl({ value, disabled: field ? !field.isEditable : false }, { validators, updateOn });
        if (field && field.isMandatory && !field.isEditable && !isTruthy(control.value)) {
            control.setErrors({ required: true });
        }
        return control;
    }

    static parseDefinition(key: string, fieldDefinitions: { key: string; isMandatory: boolean }[]): ValidatorFn[] {
        const fields = fieldDefinitions.find(t => t.key === key);
        const validators: ValidatorFn[] = [];

        if (fields && fields.isMandatory) {
            validators.push(Validators.required);
        }
        // At a minimum, we need a null validator ( a validator that doesn't do anything ), otherwise this blows up
        if (validators.length < 1) {
            validators.push(Validators.nullValidator);
        }

        return validators;
    }

    static getMandatoryControlName(formControlName: string) {
        return 'is' + formControlName + 'Mandatory';
    }

    static getEditableControlName(formControlName: string) {
        return 'is' + formControlName + 'Editable';
    }

    static getVisibleControlName(formControlName: string) {
        return 'is' + formControlName + 'Visible';
    }

    // This 'upgrades' a set of given fieldDefinitions using defaults
    static upgradeUsingDefaults(defaultDefinitions: FieldDefinition[],
        fieldDefinitions: { key: string; isMandatory: boolean; isEditable: boolean; isVisible: boolean }[]) {
        defaultDefinitions.forEach(defaultDef => {
            if (!fieldDefinitions.some(y => y.key === defaultDef.key)) {
                // If field definition does not exist, create it
                fieldDefinitions.push({ key: defaultDef.key, isEditable: defaultDef.editableValue, isMandatory: defaultDef.mandatoryValue, isVisible: defaultDef.visibleValue });
            } else {
                // If force mandatory, set isMandatory, isVisible flags to true
                if (defaultDef.forceMandatory) {
                    const fieldDef = fieldDefinitions.filter(y => y.key === defaultDef.key)[0];
                    fieldDef.isMandatory = true;
                    fieldDef.isVisible = true;
                }
            }
        });
    }

    private getFormGroup(form: AmcsFormGroup | BaseFormGroup): FormGroup {
        if (form instanceof AmcsFormGroup) {
            return form.formGroup;
        } else if (form instanceof BaseFormGroup) {
            return form.htmlFormGroup;
        } else {
            throw new Error(('Error, form group type not recognised'));
        }
    }
}
