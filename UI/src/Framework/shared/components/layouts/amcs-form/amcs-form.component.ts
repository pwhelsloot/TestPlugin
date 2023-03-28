import { Component, EventEmitter, forwardRef, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { ControlContainer } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { FormDisplayModeEnum } from '@core-module/models/forms/form-display-model.enum';
import { AmcsFormDirective, createFormGroupDirective } from './amcs-form.directive';
import { FormOptions } from './form-options.model';
@Component({
  selector: 'app-amcs-form',
  templateUrl: './amcs-form.component.html',
  styleUrls: ['./amcs-form.component.scss'],
  // RDM - Help for this from here https://stackoverflow.com/questions/53561484/access-formgroupdirective-of-container-component-inside-an-embeddedview
  // We want to have our 'formControlName' controls in a different component from the [formGroup] tag, only possible via providing a controlContainer.
  providers: [
    { provide: AmcsFormDirective, useExisting: forwardRef(() => AmcsFormComponent) },
    {
      provide: ControlContainer,
      useFactory: createFormGroupDirective,
      deps: [AmcsFormDirective]
    }
  ]
})
export class AmcsFormComponent extends AmcsFormDirective implements OnChanges {
  loadingContainerSize = 120;
  enableFormActions = false;
  displayMode = FormDisplayModeEnum.Standard;
  FormDisplayMode = FormDisplayModeEnum;

  /**
 * Options that will be used for the Form
 *
 * @type {FormOptions}
 * @memberof AmcsFormComponent
 */
  @Input() options: FormOptions;

  /**
 * AmcsFormActionsComponent outputs
 */
  @Output() onSave = new EventEmitter();
  @Output() onContinue = new EventEmitter();
  @Output() onCancel = new EventEmitter();
  @Output() onBack = new EventEmitter();

  ngOnChanges(changes: SimpleChanges): void {
    super.ngOnChanges(changes);
    if (changes['options'] && isTruthy(this.options)) {
      this.loadingContainerSize = this.options.loadingContainerSize;
      this.enableFormActions = this.options.enableFormActions;
      this.displayMode = this.options.displayMode;
    }
  }
}
