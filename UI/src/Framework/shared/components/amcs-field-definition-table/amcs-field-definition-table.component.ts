import { Component, ElementRef, Input, OnInit, Renderer2 } from '@angular/core';
import { FormControlTableItem } from '@coremodels/form-control-table-item';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { FieldDefinitionFormGroup } from '@shared-module/forms/FieldDefinitionFormGroup.model';

/**
 * @deprecated Mark to be moved to settings module in PlatformUI
 */
@Component({
  selector: 'app-amcs-field-definition-table',
  templateUrl: './amcs-field-definition-table.component.html',
  styleUrls: ['./amcs-field-definition-table.component.scss']
})
export class AmcsFieldDefinitionTableComponent extends AutomationLocatorDirective implements OnInit {
  @Input() form: FieldDefinitionFormGroup;

  constructor(protected elRef: ElementRef, protected renderer: Renderer2) {
    super(elRef, renderer);
  }

  ngOnInit() {
    this.form.items.forEach((item) => {
      const mandatoryControl = this.form.formGroup.get(FieldDefinitionFormGroup.getMandatoryControlName(item.formControlName));
      const editableControl = this.form.formGroup.get(FieldDefinitionFormGroup.getEditableControlName(item.formControlName));
      const visibleControl = this.form.formGroup.get(FieldDefinitionFormGroup.getVisibleControlName(item.formControlName));
      const newValue: boolean = visibleControl.value;

      if (newValue) {
        // eslint-disable-next-line @typescript-eslint/no-unused-expressions
        item.forcedMandatory ? mandatoryControl.disable() : mandatoryControl.enable();
        editableControl.enable();
      } else {
        if (mandatoryControl.value) {
          mandatoryControl.setValue(false);
        }
        if (editableControl.value) {
          editableControl.setValue(false);
        }

        mandatoryControl.disable();
        editableControl.disable();
      }
    });
  }

  isVisibleChanged(e: any, item: FormControlTableItem) {
    const mandatoryControl = this.form.formGroup.get(FieldDefinitionFormGroup.getMandatoryControlName(item.formControlName));
    const editableControl = this.form.formGroup.get(FieldDefinitionFormGroup.getEditableControlName(item.formControlName));
    const visibleControl = this.form.formGroup.get(FieldDefinitionFormGroup.getVisibleControlName(item.formControlName));
    const newValue: boolean = visibleControl.value;

    if (newValue) {
      // eslint-disable-next-line @typescript-eslint/no-unused-expressions
      item.forcedMandatory ? mandatoryControl.disable() : mandatoryControl.enable();
      editableControl.enable();
    } else {
      if (mandatoryControl.value) {
        mandatoryControl.setValue(false);
      }
      if (editableControl.value) {
        editableControl.setValue(false);
      }

      mandatoryControl.disable();
      editableControl.disable();
    }
  }
}
