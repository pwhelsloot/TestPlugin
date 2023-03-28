import { Component, ElementRef, forwardRef, Input, OnInit, Renderer2, ViewChild } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

/**
 * @deprecated Marked for removal, Please use amcs-text-area instead
 */
@Component({
  selector: 'app-amcs-comment',
  templateUrl: './amcs-comment.component.html',
  styleUrls: ['./amcs-comment.component.scss'],
  providers: [
    {
        provide: NG_VALUE_ACCESSOR,
        useExisting: forwardRef(() => AmcsCommentComponent),
        multi: true
    }
  ]
})
export class AmcsCommentComponent extends AutomationLocatorDirective implements ControlValueAccessor, OnInit {
  value: any;
  FormControlDisplay = FormControlDisplay;

  @Input('customClass') customClass: string;
  @Input('minheight') minheight: string;
  @Input('isDisabled') isDisabled: boolean;
  @Input('hasError') hasError = false;
  @ViewChild('input') inputElement: ElementRef;
  @Input('maxLength') maxLength: number = null;
  @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;

  constructor(protected elRef: ElementRef, protected renderer: Renderer2) {
    super(elRef, renderer);
  }

  ngOnInit() {
    this.customClass = 'comment-mobile';
  }

  onTouchedCallback: () => void = () => { };
  onChangeCallback: (_: any) => void = (_: any) => { };

  writeValue(value: string) {
    if (!isTruthy(value)) {
      this.writeNullValue();
    } else {
      this.value = value;
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

  private writeNullValue() {
    this.value = null;
    this.onChangeCallback(null);
  }
}
