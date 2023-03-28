import { FormControl, FormGroup } from '@angular/forms';

/**
 * @deprecated Move to PlatformUI
 */
export class AmcsFieldDefinitionFormHelper {

    static buildFormFromFieldDefinitions(...fieldDefinitions: { key: string; isMandatory: boolean; isEditable: boolean; isVisible: boolean }[]): FormGroup {
        const formGroup = new FormGroup({
            empty: new FormControl(false)
        });

        fieldDefinitions.forEach(data => {
            this.addFormControl(formGroup, data);
        });

        formGroup.removeControl('empty');

        return formGroup;
    }

    private static addFormControl(formGroup: FormGroup, element: { key: string; isMandatory: boolean; isEditable: boolean; isVisible: boolean }) {
        const mandatoryValue = element.isMandatory;
        const editableValue = element.isEditable;
        const visibleValue = element.isVisible;

        const mandatoryControl: FormControl = new FormControl(mandatoryValue);
        mandatoryControl.setValue(mandatoryValue);
        mandatoryControl.disable();

        const editableControl: FormControl = new FormControl(editableValue);
        editableControl.setValue(editableValue);
        editableControl.disable();

        const visibleControl: FormControl = new FormControl(visibleValue);
        visibleControl.setValue(visibleValue);
        visibleControl.disable();

        formGroup.addControl(`is${element.key}Mandatory`, mandatoryControl);
        formGroup.addControl(`is${element.key}Editable`, editableControl);
        formGroup.addControl(`is${element.key}Visible`, visibleControl);
    }
}
