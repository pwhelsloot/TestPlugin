import { AfterViewInit, Component, ElementRef, EventEmitter, forwardRef, Input, OnChanges, OnDestroy, Optional, Output, Renderer2, SimpleChanges } from '@angular/core';
import { ControlContainer, NG_VALUE_ACCESSOR } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { getNumberSeparators } from '@core-module/helpers/locale-helper';
import { FormControlDisplay } from '@coremodels/forms/form-control-display.enum';
import { AmcsFormControlBaseComponent } from '@shared-module/forms/amcs-form-control-base';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { CurrencyMaskInputMode } from 'amcs-ngx-currency';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-amcs-numerical-input',
  templateUrl: './amcs-numerical-input.component.html',
  styleUrls: ['./amcs-numerical-input.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsNumericalInputComponent),
      multi: true
    }
  ]
})
export class AmcsNumericalInputComponent extends AmcsFormControlBaseComponent implements OnChanges, AfterViewInit, OnDestroy {
  value: any;
  edited: boolean;
  FormControlDisplay = FormControlDisplay;
  inputMode = CurrencyMaskInputMode.NATURAL;

  inputValue: any;
  inputPrefix: string;
  inputSuffix: string;
  inputHasFocus = false;

  @Input() prefix = '';
  @Input() autoFocus = false;
  @Input() isDisabled = false;
  @Input() loading = false;
  @Input() customClass: string;
  @Input() decimalsDeliminator = '';
  @Input() thousandsDeliminator = '';
  @Input() customWrapperClass: string;
  @Input() alignment = 'right';
  @Input() precision = 2;
  @Input('maxLength') maxLength: number = null;
  @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;
  @Input() noMargin = false;
  @Input() noPadding = false;
  @Input() allowNegative = true;
  @Input() suffix = '';
  @Input() minusToParentheses = false;
  @Input() isMobile = false;
  @Input() hasWarning = false;
  // eslint-disable-next-line @angular-eslint/no-output-native
  @Output('blur') blur = new EventEmitter<any>();

  constructor(
    protected elRef: ElementRef,
    protected renderer: Renderer2,
    private translateSettingService: TranslateSettingService,
    @Optional() readonly controlContainer: ControlContainer
  ) {
    super(controlContainer, elRef, renderer);
    this.selectedLanguageSubscription = this.translateSettingService.selectedLanguage
      .subscribe((language: string) => {
        const numberSeparators = getNumberSeparators(language);
        this.thousandsDeliminator = this.thousandsDeliminator === ''
          ? numberSeparators.group
          : this.thousandsDeliminator;
        this.decimalsDeliminator = this.decimalsDeliminator === ''
          ? numberSeparators.decimal
          : this.decimalsDeliminator;
      });
  }
  private selectedLanguageSubscription: Subscription;


  ngAfterViewInit(): void {
    // Init the input values
    setTimeout(() => this.updateInputValues(), 0);
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['prefix'] || changes['suffix'] || changes['minusToParentheses']) {
      // Re-init the input values
      this.updateInputValues();
    }
  }

  ngOnDestroy(): void {
    this.selectedLanguageSubscription?.unsubscribe();
    super.ngOnDestroy();
  }

  onTouchedCallback: () => void = () => { };
  onChangeCallback: (_: any) => void = (_: any) => { };

  /** ControlValueAccessor interface */
  writeValue(value: any) {
    if (isTruthy(value)) {
      this.value = value;
      this.inputValue = value;
      this.edited = true;
      this.onChangeCallback(this.value);
    } else {
      this.writeNullValue();
    }

    if (!this.inputHasFocus) {
      this.updateInputValues();
    }
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
    this.inputHasFocus = true;
    e.target.select();
    this.updateInputValues();
  }

  onBlur(e: any) {
    this.inputHasFocus = false;
    this.updateInputValues();

    this.blur.next(e);
    this.onTouchedCallback();
  }

  private writeNullValue() {
    this.value = null;
    this.inputValue = null;
    this.edited = false;
    this.onChangeCallback(null);
  }

  private updateInputValues() {
    if (this.inputHasFocus) {
      this.updateParenthesesToMinus();
    } else {
      this.updateMinusToParentheses();
    }
  }

  private updateParenthesesToMinus() {
    // Pass values to input values
    if (this.inputValue !== this.value) {
      this.inputValue = this.value;
    }
    if (this.inputPrefix !== this.prefix) {
      this.inputPrefix = this.prefix;
    }
    if (this.inputSuffix !== this.suffix) {
      this.inputSuffix = this.suffix;
    }
  }

  private updateMinusToParentheses() {
    let newInputValue: number;
    let newInputPrefix: string;
    let newInputSuffix: string;

    if (isTruthy(this.value) && this.minusToParentheses && this.value < 0) {
      // If using parentheses for negative numbers, input value is set to positive and surrounded by parentheses.
      newInputValue = Math.abs(this.value);
      newInputPrefix = `(${this.prefix}`;
      newInputSuffix = `)${this.suffix}`;
    } else {
      // Otherwise, pass values to input values
      newInputValue = this.value;
      newInputPrefix = this.prefix;
      newInputSuffix = this.suffix;
    }

    if (this.inputValue !== newInputValue) {
      this.inputValue = newInputValue;
    }
    if (this.inputPrefix !== newInputPrefix) {
      this.inputPrefix = newInputPrefix;
    }
    if (this.inputSuffix !== newInputSuffix) {
      this.inputSuffix = newInputSuffix;
    }
  }
}
