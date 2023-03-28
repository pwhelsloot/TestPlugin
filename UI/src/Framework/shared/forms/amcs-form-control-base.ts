import { BooleanInput, coerceBooleanProperty } from '@angular/cdk/coercion';
import { AfterContentInit, Component, ElementRef, Input, OnDestroy, Renderer2 } from '@angular/core';
import { AbstractControl, ControlContainer, FormControl } from '@angular/forms';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { Subscription } from 'rxjs';
import { AmcsFormValidationHelper } from './amcs-form-validation-helper';

@Component({
  template: ''
})
export abstract class AmcsFormControlBaseComponent extends AutomationLocatorDirective implements OnDestroy, AfterContentInit {
  /**
   * Determines if this control is using the default form label or a custom one using projection
   * Gets set to True if a Label or Translate is provided
   */
  isDefaultLabel = false;

  useDefaultErrorState = true;

  protected required: BooleanInput;

  @Input() formControl: FormControl;

  get hasError() {
    return this.useDefaultErrorState ? AmcsFormValidationHelper.formControlHasError(this.formControl) : this._hasError;
  }

  @Input() set hasError(value) {
    this._hasError = value;
    if (value !== undefined) {
      this.useDefaultErrorState = false;
    }
  }

  get label(): string {
    return this._label;
  }

  @Input() set label(value: string) {
    this._label = value;
    this.initLabel();
  }

  get isRequired(): BooleanInput {
    return this.required;
  }

  @Input() set isRequired(value: BooleanInput) {
    this.required = coerceBooleanProperty(value);
  }

  constructor(readonly controlContainer: ControlContainer, protected elRef: ElementRef, protected renderer: Renderer2) {
    super(elRef, renderer);
  }

  private _label: string;
  private _hasError = false;
  private statusSubscription: Subscription;

  /**
   * Due to how we handle our FormGroups(HtmlFormGroup) ControlContainer is only populated afterContentInit, during ctor this is still empty
   */
  ngAfterContentInit() {
    this.setFormControl();
    this.initLabel();
  }

  initLabel() {
    this.setUsingDefaultLabel();
    this.setIsRequired();
  }

  setIsRequired() {
    // only set if not set before checking for !this.isRequired will not work since it will catch false
    if (this.isDefaultLabel && this.isRequired === undefined && this.formControl !== undefined && this.statusSubscription === undefined) {
      this.isRequired = this.hasRequiredValidator();
      this.statusSubscription = this.formControl.statusChanges.subscribe(() => {
        this.isRequired = this.hasRequiredValidator();
      });
    }
  }

  /**
   * Check if the Label is provided, if so enable default label
   */
  setUsingDefaultLabel() {
    if (this.label) {
      this.isDefaultLabel = true;
    }
  }

  /**
   * Check the current form control if a Required Validator is attached
   * @returns True if Control has the Required validator
   */
  hasRequiredValidator() {
    const control = this.formControl;
    const validatorFn = control?.validator;
    if (validatorFn && validatorFn({} as AbstractControl)?.required) {
      return true;
    }
    return false;
  }

  /**
   * Try and get the Control from the injected ControlContainer if a formControlName is provided
   * @returns Control from ControlContainer if provided
   */
  getControlFromControlContainer(): FormControl {
    if (this.formControlName) {
      return this.controlContainer?.control?.get(this.formControlName) as FormControl;
    } else {
      return undefined;
    }
  }

  setFormControl() {
    if (this.formControl === undefined) {
      this.formControl = this.getControlFromControlContainer();
    }
  }

  ngOnDestroy(): void {
    if (this.statusSubscription) {
      this.statusSubscription.unsubscribe();
    }
  }
}
