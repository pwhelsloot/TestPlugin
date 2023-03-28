import {
  AfterViewInit,
  Component,
  ElementRef,
  forwardRef,
  Input,
  OnChanges,
  Optional,
  Renderer2,
  SimpleChanges,
  ViewChild
} from '@angular/core';
import { ControlContainer, ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { AmcsFormControlBaseComponent } from '@shared-module/forms/amcs-form-control-base';
import { GlossaryPipe } from '@shared-module/pipes/glossary.pipe';

@Component({
  selector: 'app-amcs-text-area',
  templateUrl: './amcs-text-area.component.html',
  styleUrls: ['./amcs-text-area.component.scss'],
  providers: [
    GlossaryPipe,
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsTextAreaComponent),
      multi: true,
    },
  ],
})
export class AmcsTextAreaComponent extends AmcsFormControlBaseComponent implements OnChanges, AfterViewInit, ControlValueAccessor {
  edited = false;
  value: any;
  FormControlDisplay = FormControlDisplay;

  @Input('customClass') customClass: string;
  @Input('minheight') minheight: string;
  @Input('height') height: string;
  @Input('inputTooltip') inputTooltip: string;
  @Input('isDisabled') isDisabled: boolean;
  @Input('autocomplete') autocomplete = 'on';
  @Input('autoFocus') autoFocus = false;
  @Input('horizontalResizeEnabled') horizontalResizeEnabled = false;
  @ViewChild('input') inputElement: ElementRef;
  @Input('maxLength') maxLength: number = null;
  @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;

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
    if (changes['isDisabled']) {
      this.transformValueUsingGlossary();
    }
  }

  ngAfterViewInit() {
    if (this.customClass != null) {
      const classes = this.customClass.split(' ');
      classes.forEach((element) => {
        this.renderer.addClass(this.inputElement.nativeElement, element);
      });
    }
    this.setResizeState(this.horizontalResizeEnabled);
  }

  /** ControlValueAccessor interface */
  writeValue(value: string) {
    if (!isTruthy(value)) {
      this.writeNullValue();
    } else {
      this.edited = true;
      this.value = value;
      this.preGlossaryTransformValue = value;
      this.transformValueUsingGlossary();
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

  private transformValueUsingGlossary() {
    if (this.isDisabled) {
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

  private setResizeState(value: boolean) {
    if (value) {
      this.inputElement.nativeElement.style.resize = 'both';
    } else {
      this.inputElement.nativeElement.style.resize = 'vertical';
    }
  }
}
