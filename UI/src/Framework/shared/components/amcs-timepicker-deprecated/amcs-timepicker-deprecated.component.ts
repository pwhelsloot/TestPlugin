import { AfterViewInit, Component, ElementRef, forwardRef, Input, OnChanges, OnDestroy, OnInit, Renderer2, SimpleChanges, ViewChild, ViewEncapsulation } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { AmcsTimepickerConfig } from '@shared-module/components/amcs-timepicker/amcs-timepicker-config.model';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { TimepickerComponent, TimepickerConfig } from 'ngx-bootstrap/timepicker';

/**
 * @deprecated Marked for removal, use the app-amcs-timepicker instead
 */
@Component({
  selector: 'app-amcs-timepicker-deprecated',
  templateUrl: './amcs-timepicker-deprecated.component.html',
  styleUrls: ['./amcs-timepicker-deprecated.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsTimepickerDeprecatedComponent),
      multi: true
    }],
  encapsulation: ViewEncapsulation.None
})

export class AmcsTimepickerDeprecatedComponent extends AutomationLocatorDirective implements OnInit, OnChanges, OnDestroy, AfterViewInit, ControlValueAccessor {
  ngxConfig: TimepickerConfig;
  edited = false;

  get innerValue(): Date {
    return this._innerValue;
  }

  set innerValue(value: Date) {
    this.writeValue(value);
  }

  @Input('config') config: AmcsTimepickerConfig;
  @Input() label: string;
  @Input() hasError = false;
  @Input('smallMode') smallMode = false;
  @Input('timeTooltip') timeTooltip: string;
  @Input() disabled = false;

  @ViewChild('timePicker') timePicker: TimepickerComponent;

  onTouchedCallback: () => void;
  onChangeCallback: (_: any) => void;

  constructor(protected elRef: ElementRef, protected renderer: Renderer2) {
    super(elRef, renderer);
  }

  private _innerValue: Date;
  private showMeridian: boolean;
  private meridians: string[] = ['AM', 'PM'];

  private hourEventListener: () => void;
  private minuteEventListener: () => void;

  ngOnInit() {
    if (this.onTouchedCallback == null) {
      this.onTouchedCallback = () => { };
    }
    if (this.onChangeCallback == null) {
      this.onChangeCallback = (_: any) => { };
    }

    this.showMeridian = this.canShowMeridian();
    if (this.showMeridian) {
      this.meridians = this.getMerdians();
    }
    this.refreshConfig();
  }

  ngOnChanges(changes: SimpleChanges) {
    this.refreshConfig();
  }

  ngAfterViewInit() {
    /*
        A bit brute force, but this implementaiton allows us to react to each indiviual hour
        & minute input fields in order to prevent non-numeric characters AND invalid hour/minute
        values. Can't rely on the internal values the ngx-timepicker component stores, because
        they don't update until the given input field loses focus. - JTW
    */
    const hourInput = this.elRef.nativeElement.getElementsByTagName('input')[0];
    const minuteInput = this.elRef.nativeElement.getElementsByTagName('input')[1];

    this.hourEventListener = this.renderer.listen(hourInput, 'keydown', ($event) => {
      const keycode = this.convertKeyCodeToValue($event.which, $event.shiftKey);

      if (keycode === null) {
        return true;
      }

      const selectionStart = hourInput.selectionStart;
      const selectionEnd = hourInput.selectionEnd;
      const combinedValue = this.combineCurrentAndNextVal(hourInput.value, keycode, selectionStart, selectionEnd);
      if (this.isNonNumericCharacter(keycode) || parseInt(combinedValue, 10) > 12) {
        return false;
      }

      if (!minuteInput.value) {
        this.timePicker.updateMinutes('00');
      }

      return true;
    });

    this.minuteEventListener = this.renderer.listen(minuteInput, 'keydown', ($event) => {
      const keycode = this.convertKeyCodeToValue($event.which, $event.shiftKey);

      if (keycode === null) {
        return true;
      }

      const selectionStart = minuteInput.selectionStart;
      const selectionEnd = minuteInput.selectionEnd;
      const combinedValue = this.combineCurrentAndNextVal(minuteInput.value, keycode, selectionStart, selectionEnd);
      if (this.isNonNumericCharacter(keycode) || parseInt(combinedValue, 10) > 59) {
        return false;
      }

      if (!hourInput.value) {
        this.timePicker.updateHours('01');
      }

      return true;
    });
  }

  ngOnDestroy() {
    this.hourEventListener();
    this.minuteEventListener();
  }

  /** ControlValueAccessor interface */
  writeValue(value: Date) {
    if (value !== null && value !== undefined && !this.valueIsEqual(value)) {
      this._innerValue = value;
      this.edited = true;
      this.onChangeCallback(this._innerValue);
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
    this.disabled = isDisabled;
    this.ngxConfig = { ...this.ngxConfig, readonlyInput: this.disabled };
  }

  private isNonNumericCharacter(val: string): boolean {
    const nonNumericRegex = /[^0-9]+$/g;
    return val === null ? false : nonNumericRegex.test(val);
  }

  private convertKeyCodeToValue(val: number, shiftKey: boolean): string {
    // Make sure we catch shift key usage, e.g., shift+1 = !, not 1 as the keycode would incorrectly indicate
    if (shiftKey && val >= 48 && val <= 57) {
      return String.fromCharCode(65);
    }

    return val >= 8 && val <= 46 && val !== 32 ? null : String.fromCharCode(val >= 96 && val <= 105 ? val - 48 : val);
  }

  /** End ControlValueAccessor interface */

  private refreshConfig() {
    if (this.ngxConfig == null) {
      this.ngxConfig = new TimepickerConfig();
    }
    // Sets some defaults but any config properties will take presentence
    this.ngxConfig = {
      ...this.ngxConfig,
      showMeridian: this.showMeridian,
      meridians: this.meridians,
      showSpinners: false,
      arrowkeys: true,
      ...this.config
    };
  }

  private canShowMeridian(): boolean {
    return true;
  }

  private getMerdians(): string[] {
    return ['AM', 'PM'];
  }

  private valueIsEqual(value: any) {
    if (value != null && this._innerValue != null) {
      return value.getTime() === this._innerValue.getTime();
    } else {
      return value === this._innerValue;
    }
  }

  private combineCurrentAndNextVal(currentVal: string, nextVal, start: number, end: number): string {
    let combinedValue: string;

    if (start === 0 && end === 2) {
      combinedValue = nextVal;
    } else if (start === 0 && end === 1) {
      combinedValue = `${nextVal}${currentVal[1]}`;
    } else if (start === 1 && end === 2) {
      combinedValue = `${currentVal[0]}${nextVal}`;
    } else {
      combinedValue = `${currentVal}${nextVal}`;
    }

    return combinedValue;
  }
}
