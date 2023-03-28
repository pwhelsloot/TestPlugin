import { FormControl, FormGroup } from '@angular/forms';
import { FormControlTableItem } from '@coremodels/form-control-table-item';
import { FieldDefinitionFormGroup } from '@shared-module/forms/FieldDefinitionFormGroup.model';
import { IFieldDefinitionGroup } from '@shared-module/models/field-definition-group.interface';
import { FieldDefinition } from '@shared-module/models/field-definition.model';

/**
 * @deprecated Move to PlatformUI
 */
export class FieldDefinitionFormGroupBuilder {

    constructor(private fieldDefGroup: IFieldDefinitionGroup) {
    }

    private formItems: FormControlTableItem[] = [];
    private existingFieldDefinitions: { key: string; isMandatory: boolean; isEditable: boolean; isVisible: boolean }[];

    build(showVisible?: boolean): FieldDefinitionFormGroup {
        if (showVisible === null || showVisible === undefined) {
            showVisible = true;
        }

        const formGroup = new FormGroup({
            empty: new FormControl(false)
        });
        this.fieldDefGroup.definitions.forEach(element => {
            this.addFormControl(formGroup, element);
        });
        // sadly needed just to instiantiate the formgroup
        formGroup.removeControl('empty');
        return new FieldDefinitionFormGroup(formGroup, this.formItems, showVisible);
    }

    addExistingDefinitions(existingFieldDefinitions: { key: string; isMandatory: boolean; isEditable: boolean; isVisible: boolean }[]) {
        this.existingFieldDefinitions = existingFieldDefinitions;
        return this;
    }

    private addFormControl(formGroup: FormGroup, element: FieldDefinition) {
        let mandatoryValue = element.mandatoryValue;
        let editableValue = element.editableValue;
        let visibleValue = element.visibleValue;

        // if there is an existing definition then set mandatoy/editable based on it.
        if (this.existingFieldDefinitions) {
            const existingValues: { key: string; isMandatory: boolean; isEditable: boolean; isVisible: boolean } = this.existingFieldDefinitions.find(x => x.key === element.key);
            if (existingValues != null) {
                mandatoryValue = existingValues.isMandatory;
                editableValue = existingValues.isEditable;
                visibleValue = existingValues.isVisible;
            }
        }
        const mandatoryControl: FormControl = new FormControl(mandatoryValue);
        if (element.forceMandatory) {
            mandatoryControl.setValue(true);
            mandatoryControl.disable();
        }
        const editableControl: FormControl = new FormControl(editableValue);
        const visibleControl: FormControl = new FormControl(visibleValue);
        if (element.forceMandatory || element.forceVisible) {
            visibleControl.setValue(true);
            visibleControl.disable();
        }

        formGroup.addControl(FieldDefinitionFormGroup.getMandatoryControlName(element.key), mandatoryControl);
        formGroup.addControl(FieldDefinitionFormGroup.getEditableControlName(element.key), editableControl);
        formGroup.addControl(FieldDefinitionFormGroup.getVisibleControlName(element.key), visibleControl);
        const formItem: FormControlTableItem = new FormControlTableItem();
        formItem.label = element.label;
        formItem.formControlName = element.key;
        formItem.forcedMandatory = element.forceMandatory;
        formItem.validators = element.validators != null ? element.validators : [];
        this.formItems.push(formItem);
    }
}
