import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  forwardRef,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Optional,
  Output,
  Renderer2,
  SimpleChanges,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { ControlContainer, ControlValueAccessor, FormControl, NG_VALIDATORS, NG_VALUE_ACCESSOR, Validator } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { FormControlDisplay } from '@coremodels/forms/form-control-display.enum';
import { AmcsTimepickerConfig } from '@shared-module/components/amcs-timepicker/amcs-timepicker-config.model';
import { AmcsFormControlBaseComponent } from '@shared-module/forms/amcs-form-control-base';
import { PopoverDirective } from 'ngx-bootstrap/popover';
import { TimepickerComponent, TimepickerConfig } from 'ngx-bootstrap/timepicker';

@Component({
  selector: 'app-amcs-timepicker',
  templateUrl: './amcs-timepicker.component.html',
  styleUrls: ['./amcs-timepicker.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsTimepickerComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => AmcsTimepickerComponent),
      multi: true,
    },
  ],
  encapsulation: ViewEncapsulation.None,
})
export class AmcsTimepickerComponent
  extends AmcsFormControlBaseComponent
  implements OnInit, OnChanges, AfterViewInit, OnDestroy, ControlValueAccessor, Validator
{
  @Output() editedChanged = new EventEmitter<boolean>();

  ngxConfig: TimepickerConfig;
  edited = false;
  FormControlDisplay = FormControlDisplay;

  get innerValue(): Date {
    return this._innerValue;
  }

  set innerValue(value: Date) {
    this.writeValue(value);
  }

  get stringValue(): string {
    return this._stringValue;
  }

  set stringValue(value: string) {
    this.writeValue(value);
  }

  @Input('config') config: AmcsTimepickerConfig;
  @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;
  @Input('timeTooltip') timeTooltip: string;
  @Input() disabled = false;
  @Input() interval = 1;
  // if displayseconds = true we currently don't support the popover
  @Input() displayseconds = false;
  @Input() addToolTipIcon = false;

  @ViewChild('popover') popoverDirective: PopoverDirective;
  @ViewChild('timepicker') timepickerComponent: TimepickerComponent;
  @ViewChild('timepickerDiv') timepickerDiv: ElementRef;
  @ViewChild('timeInput') timeInput: any;

  onTouchedCallback: () => void;
  onChangeCallback: (_: any) => void;

  constructor(protected elRef: ElementRef, protected renderer: Renderer2, @Optional() readonly controlContainer: ControlContainer) {
    super(controlContainer, elRef, renderer);
    this.hourEventListener = () => true;
    this.minuteEventListener = () => true;
  }

  private _innerValue: Date;
  private _stringValue: string;

  private showMeridian: boolean;
  private meridians: string[] = ['AM', 'PM'];

  private hourEventListener: () => void;
  private minuteEventListener: () => void;

  ngOnInit() {
    if (this.onTouchedCallback == null) {
      this.onTouchedCallback = () => {};
    }
    if (this.onChangeCallback == null) {
      this.onChangeCallback = (_: any) => {};
    }

    this.showMeridian = this.canShowMeridian();
    if (this.showMeridian) {
      this.meridians = this.getMerdians();
    }
    this.refreshConfig();
  }

  ngAfterViewInit() {
    this.popoverDirective.onShown.subscribe(() => {
      // When popover is shown, configure events on timepicker inputs
      // NOTE: Need a delay to allow popover to be added to DOM.
      setTimeout(() => {
        this.configureTimePicker();
      }, 150);
    });

    this.popoverDirective.onHidden.subscribe(() => {
      // When popover is hidden, remove events on timepicker inputs
      this.hourEventListener = () => true;
      this.minuteEventListener = () => true;
    });
    if (this.displayseconds && this.timeInput) {
      this.renderer.setAttribute(this.timeInput.nativeElement, 'step', '1');
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    this.refreshConfig();
  }

  configureTimePicker() {
    /*
        A bit brute force, but this implementaiton allows us to react to each indiviual hour
        & minute input fields in order to prevent non-numeric characters AND invalid hour/minute
        values. Can't rely on the internal values the ngx-timepicker component stores, because
        they don't update until the given input field loses focus. - JTW
    */
    const hourInput = this.timepickerDiv.nativeElement.getElementsByTagName('input')[0];
    const minuteInput = this.timepickerDiv.nativeElement.getElementsByTagName('input')[1];

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
        this.timepickerComponent.updateMinutes('00');
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
        this.timepickerComponent.updateHours('01');
      }

      return true;
    });
  }

  ngOnDestroy() {
    this.hourEventListener();
    this.minuteEventListener();

    this.popoverDirective.onShown.unsubscribe();
    this.popoverDirective.onHidden.unsubscribe();

    super.ngOnDestroy();
  }

  /** ControlValueAccessor interface */
  writeValue(value: any) {
    if (value !== null && value !== undefined) {
      let dateValue: Date;
      let stringValue: string;

      if (typeof value === 'string') {
        dateValue = this.parseTime(value);
        stringValue = value;
      } else {
        dateValue = value;
        dateValue.setDate(1);
        dateValue.setFullYear(2000);
        dateValue.setMonth(1);
        if (isTruthy(this.displayseconds) && this.displayseconds) {
          stringValue =
            this.formatTwoDigits(dateValue.getHours()) +
            ':' +
            this.formatTwoDigits(dateValue.getMinutes()) +
            ':' +
            this.formatTwoDigits(dateValue.getSeconds());
        } else {
          stringValue = this.formatTwoDigits(dateValue.getHours()) + ':' + this.formatTwoDigits(dateValue.getMinutes());
        }
      }

      if (!this.valueIsEqual(dateValue)) {
        this._innerValue = dateValue;
        this._stringValue = stringValue;
        this.edited = true;
        this.editedChanged.emit(this.edited);
        this.onChangeCallback(this._innerValue);
        this.onTouchedCallback();
      }
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
  /** End ControlValueAccessor interface */

  /** Validate interface */
  validate(c: FormControl) {
    return isTruthy(this.timeInput) && (this.timeInput.nativeElement.validity.badInput || !this.timeInput.nativeElement.validity.valid)
      ? {
          invalidInputError: {
            valid: false,
          },
        }
      : null;
  }
  /** End Validate interface */

  toggle() {
    if (!this.displayseconds && !this.disabled && !this.ngxConfig.readonlyInput) {
      this.popoverDirective.toggle();
    }
  }

  timeInputKeyUp(e: any) {
    // Update the form
    this.onTouchedCallback();
    this.onChangeCallback(this._innerValue);
  }

  timeInputClick(e: any) {
    this.onTouchedCallback();
    this.onChangeCallback(this._innerValue);
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

  private refreshConfig() {
    if (this.ngxConfig == null) {
      this.ngxConfig = new TimepickerConfig();
    }
    // Sets some defaults but any config properties will take presentence
    this.ngxConfig = {
      ...this.ngxConfig,
      showMeridian: this.showMeridian,
      meridians: this.meridians,
      showSpinners: true,
      arrowkeys: true,
      minuteStep: this.interval,
      ...this.config,
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

  private parseTime(timeString: string): Date {
    if (!isTruthy(timeString)) {
      return null;
    }
    const splitMax = this.displayseconds ? 3 : 2;
    const splTimeString = timeString.split(':', splitMax);
    if (splTimeString.length !== splitMax) {
      return null;
    }

    const hours = +splTimeString[0];
    const minutes = +splTimeString[1];
    const date = AmcsDate.createFor(2000, 1, 1);
    date.setHours(hours);
    date.setMinutes(minutes);
    if (this.displayseconds) {
      const seconds = +splTimeString[2];
      date.setSeconds(seconds);
    }
    return date;
  }

  private formatTwoDigits(value: number): string {
    if (value === undefined || value === null) {
      return null;
    }

    return ('0' + value).slice(-2);
  }
}
