import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  forwardRef,
  Input,
  OnChanges,
  Optional,
  Output,
  Renderer2,
  SimpleChanges,
  TemplateRef,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { ControlContainer, ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { FormControlDisplay } from '@coremodels/forms/form-control-display.enum';
import { AmcsFormControlBaseComponent } from '@shared-module/forms/amcs-form-control-base';
import { GlossaryPipe } from '@shared-module/pipes/glossary.pipe';

@Component({
  selector: 'app-amcs-input',
  templateUrl: './amcs-input.component.html',
  styleUrls: ['./amcs-input.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    GlossaryPipe,
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsInputComponent),
      multi: true,
    },
  ],
})
export class AmcsInputComponent extends AmcsFormControlBaseComponent implements OnChanges, AfterViewInit, ControlValueAccessor {
  edited = false;
  value: any;
  FormControlDisplay = FormControlDisplay;

  /**
   * Don't use for number, use app-amcs-numerical-input instead
   */
  @Input('type') type = 'text';
  @Input('min') min;
  @Input('step') step: string;
  @Input('customClass') customClass: string;
  @Input() labelClass: string;
  @Input('customWrapperClass') customWrapperClass: string;
  @Input('inputTooltip') inputTooltip: string | TemplateRef<any>;
  @Input('isDisabled') isDisabled: boolean;
  @Input('isReadOnly') isReadOnly = false;
  @Input('autocomplete') autocomplete = 'on';
  @Input('autoFocus') autoFocus = false;
  @Input('noPadding') noPadding = false;
  @Input() noMargin = false;
  @Input('icon') icon: string;
  @Input('placeholder') placeholder: string;
  @Input('maxLength') maxLength: number = null;
  @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;
  @Input() precision: number;
  @Input() enableClear = false;
  @Input() isMobile = false;
  @Input() selectAllOnFocus = false;
  @Input() addToolTipIcon = false;

  // eslint-disable-next-line @angular-eslint/no-output-native
  @Output('change') change = new EventEmitter<any>();
  // eslint-disable-next-line @angular-eslint/no-output-native
  @Output('blur') blur = new EventEmitter<any>();
  // eslint-disable-next-line @angular-eslint/no-output-native
  @Output('focus') focus = new EventEmitter<any>();
  @ViewChild('defaultInput') defaultInput: ElementRef;
  @ViewChild('checkboxInput') checkboxInput: ElementRef;
  @ViewChild('wrapper') wrapperElement: ElementRef;

  constructor(
    protected readonly elRef: ElementRef,
    protected readonly renderer: Renderer2,
    private readonly glossaryPipe: GlossaryPipe,
    @Optional() readonly controlContainer: ControlContainer
  ) {
    super(controlContainer, elRef, renderer);
  }

  private preGlossaryTransformValue: string;

  onTouchedCallback: () => void = () => {};
  onChangeCallback: (_: any) => void = (_: any) => {};

  ngOnChanges(changes: SimpleChanges) {
    if (changes['isReadOnly'] || changes['isDisabled']) {
      this.transformValueUsingGlossary();
    }
  }

  ngAfterViewInit() {
    if (this.customClass != null) {
      const classes = this.customClass.split(' ');
      classes.forEach((element) => {
        if (this.type === 'checkbox') {
          this.renderer.addClass(this.checkboxInput.nativeElement, element);
        } else {
          this.renderer.addClass(this.defaultInput.nativeElement, element);
        }
      });
    }
    if (this.customWrapperClass != null) {
      const classes = this.customWrapperClass.split(' ');
      classes.forEach((element) => {
        this.renderer.addClass(this.wrapperElement.nativeElement, element);
      });
    }
  }

  /** ControlValueAccessor interface */
  writeValue(value: any) {
    if (!isTruthy(value)) {
      this.writeNullValue();
    } else {
      this.edited = true;
      if (this.type === 'number' || this.type === 'currency') {
        value = Number(value);
      }
      this.value = value;
      if (this.type === 'text') {
        this.preGlossaryTransformValue = value;
        this.transformValueUsingGlossary();
      }
      this.onChangeCallback(this.value);
      this.onTouchedCallback();
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
    this.transformValueUsingGlossary();
  }

  /** End ControlValueAccessor interface */

  onChange(e: any) {
    this.change.next(e);
  }

  onFocus(e: any) {
    if (this.selectAllOnFocus) {
      e.target.select();
    }
    this.focus.next(e);
  }

  onBlur(e: any) {
    // If number, round to precision
    if ((this.type === 'number' || this.type === 'currency') && isTruthy(this.precision) && isTruthy(this.value)) {
      const factor = Math.pow(10, this.precision);
      const roundedValue = Math.round(Number(this.value) * factor) / factor;
      if (roundedValue !== this.value) {
        this.writeValue(roundedValue);
      }
    }
    this.blur.next(e);
    this.onTouchedCallback();
  }

  keyPressEvent(e: any) {
    if (isTruthy(this.min) && this.min >= 0 && e.keyCode === 45) {
      e.preventDefault();
      return;
    } else if (isTruthy(this.precision) && this.precision === 0 && e.key === '.') {
      e.preventDefault();
      return;
    }
  }

  private transformValueUsingGlossary() {
    if (this.isReadOnly || this.isDisabled) {
      this.value = this.glossaryPipe.transform(this.value);
    } else if (this.value !== this.preGlossaryTransformValue) {
      this.value = this.preGlossaryTransformValue;
    }
  }

  private writeNullValue() {
    this.edited = false;
    this.value = null;
    this.preGlossaryTransformValue = null;
    this.onChangeCallback(null);
  }
}
