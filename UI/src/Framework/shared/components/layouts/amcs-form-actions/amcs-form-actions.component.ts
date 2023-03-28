import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormControl } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { BaseFormGroup } from '@shared-module/forms/base-form-group.model';
import { FormActionOptions } from './form-action-options.model';

@Component({
  selector: 'app-amcs-form-actions',
  templateUrl: './amcs-form-actions.component.html',
  styleUrls: ['./amcs-form-actions.component.scss'],
})
export class AmcsFormActionsComponent implements OnInit, OnChanges {
  /**
   * The Form
   *
   * @type { BaseFormGroup }
   * @memberof AmcsFormActionsComponent
   */
  @Input() form: BaseFormGroup = null;

  /**
   * Options that will be used for the Form Actions
   *
   * @type {FormActionOptions}
   * @memberof AmcsFormActionsComponent
   */
  @Input() options: FormActionOptions;

  /**
   * Emitted when the Save button is clicked and form is valid
   *
   * @memberof AmcsFormActionsComponent
   */
  @Output() onSave = new EventEmitter();

  /**
   * Emitted when the Continue button is clicked and form is valid
   *
   * @memberof AmcsFormActionsComponent
   */
  @Output() onContinue = new EventEmitter();

  /**
   * Emitted when the Cancel button is clicked
   *
   * @memberof AmcsFormActionsComponent
   */
  @Output() onCancel = new EventEmitter();

  /**
   * Emitted when the Back button is clicked
   *
   * @memberof AmcsFormActionsComponent
   */
  @Output() onBack = new EventEmitter();

  hasButtons = false;
  isEditMode = false;
  backCustomClass = 'btn-default pull-left';
  continueCustomClass = 'amcs-green pull-right continue-button';
  handelSaveorDeleteCustomClass = 'pull-right';

  ngOnInit(): void {
    this.setUpEditMode();
    this.setUpButtons();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['form']) {
      this.setUpEditMode();
    }
    if (changes['options']) {
      this.setUpButtons();
    }
  }

  doSave(): void {
    if (this.checkIfSaveDisabled()) {
      return;
    } else if (this.form.checkIfValid()) {
      this.onSave.emit();
    }
  }

  doContinue(): void {
    if (this.form.checkIfValid()) {
      this.onContinue.emit();
    }
  }

  doCancel(): void {
    this.onCancel.emit();
  }

  doBack(): void {
    this.onBack.emit();
  }

  private checkIfSaveDisabled(): boolean {
    return (this.isEditMode && this.options.checkPristine && this.form.htmlFormGroup.pristine) || this.options.disableSave;
  }

  private setUpButtons(): void {
    if (!isTruthy(this.options)) {
      this.hasButtons = false;
      return;
    }
    this.hasButtons = this.options.enableSave || this.options.enableContinue || this.options.enableCancel || this.options.enableBack;
    if (this.options.enableBack && this.options.enableContinue) {
      this.backCustomClass += ' margin-right-only';
    }
    if (this.options.enableContinue && this.options.enableSave) {
      this.continueCustomClass += ' margin-right';
    }

    if (this.options.enableDelete) {
      this.handelSaveorDeleteCustomClass += ' amcs-green-confirm';
    } else if (!this.options.enableDelete && this.options.enableSave) {
      this.handelSaveorDeleteCustomClass += ' amcs-green';
    }
  }

  private setUpEditMode(): void {
    if (!isTruthy(this.form)) {
      this.isEditMode = false;
      return;
    }
    const primaryKey: FormControl = this.form.getPrimaryKeyControl();
    this.isEditMode = isTruthy(primaryKey.value);
  }
}
