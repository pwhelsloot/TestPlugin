import { Component, ElementRef, EventEmitter, forwardRef, Input, Optional, Output, Renderer2 } from '@angular/core';
import { ControlContainer, ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { FormControlDisplay } from '@coremodels/forms/form-control-display.enum';
import { AmcsFormControlBaseComponent } from '@shared-module/forms/amcs-form-control-base';

@Component({
  selector: 'app-amcs-quantity-selector',
  templateUrl: './amcs-quantity-selector.component.html',
  styleUrls: ['./amcs-quantity-selector.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsQuantitySelectorComponent),
      multi: true
    }
  ]
})
export class AmcsQuantitySelectorComponent extends AmcsFormControlBaseComponent implements ControlValueAccessor {
  value = 0;
  edited = false;
  FormControlDisplay = FormControlDisplay;
  maxLength: number;

  @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;
  @Input() isDisabled = false;
  @Input() autoFocus = false;
  @Input() minValue: number;
  @Input() maxValue: number;
  @Input() noMargin = false;
  @Input() customClass: string = null;
  // eslint-disable-next-line @angular-eslint/no-output-native
  @Output() change = new EventEmitter<any>();
  // eslint-disable-next-line @angular-eslint/no-output-native
  @Output() blur = new EventEmitter<any>();
  // eslint-disable-next-line @angular-eslint/no-output-native
  @Output() focus = new EventEmitter<any>();

  constructor(protected elRef: ElementRef, protected renderer: Renderer2, @Optional() readonly controlContainer: ControlContainer) {
    super(controlContainer, elRef, renderer);
  }

  onTouchedCallback: () => void = () => {};
  onChangeCallback: (_: any) => void = (_: any) => {};

  /** ControlValueAccessor interface */
  writeValue(value: any) {
    this.value = this.validateValue(Number(value));
    this.onChangeCallback(this.value);
  }

  validateValue(numericValue: number): number {
    this.edited = isTruthy(numericValue) && !isNaN(numericValue);
    if (this.edited) {
      numericValue = this.clampValue(numericValue);
      return numericValue;
    }
    return 0;
  }

  clampValue(numericValue: number): number {
    if (isTruthy(this.minValue) && numericValue < this.minValue) {
      this.maxLength = Math.abs(this.minValue).toString().length;
      return this.minValue;
    }
    if (isTruthy(this.maxValue) && numericValue > this.maxValue) {
      this.maxLength = Math.abs(this.maxValue).toString().length;
      return this.maxValue;
    }
    this.maxLength = null;
    return numericValue;
  }

  numericalInputChanged(value: any) {
    if (value === this.value) {
      return;
    }
    this.writeValue(value);
  }

  registerOnChange(fn: any) {
    this.onChangeCallback = fn;
  }

  registerOnTouched(fn: any) {
    this.onTouchedCallback = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.isDisabled = isDisabled;
  }

  /** End ControlValueAccessor interface */

  onFocus(e: any) {
    this.focus.next(e);
  }

  onBlur(e: any) {
    this.blur.next(e);
    this.onTouchedCallback();
  }

  onChange(e: any) {
    this.change.emit(e);
  }

  decreaseQty(e: any) {
    if (!isTruthy(this.minValue) || this.value > this.minValue) {
      this.writeValue(--this.value);
    }
  }

  increaseQty(e: any) {
    if (!isTruthy(this.maxValue) || this.value < this.maxValue) {
      this.writeValue(++this.value);
    }
  }
}
