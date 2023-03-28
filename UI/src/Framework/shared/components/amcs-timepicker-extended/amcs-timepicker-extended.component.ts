import { DecimalPipe } from '@angular/common';
import { Component, ElementRef, forwardRef, Input, OnInit, Renderer2, ViewChild } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

/**
 * @deprecated Marked for removal, use app-amcs-timepicker instead
 */
@Component({
  selector: 'app-amcs-timepicker-extended',
  templateUrl: './amcs-timepicker-extended.component.html',
  styleUrls: ['./amcs-timepicker-extended.component.scss'],
  providers: [
    DecimalPipe,
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsTimepickerExtendedComponent),
      multi: true
    }]
})
export class AmcsTimepickerExtendedComponent extends AutomationLocatorDirective implements OnInit, ControlValueAccessor {

  @ViewChild('hourInput') hourInput: ElementRef;
  @ViewChild('minuteInput') minuteInput: ElementRef;

  @Input('use12HourClock') use12HourClock = false;
  @Input('showMerdians') showMerdians = true;
  @Input('showTimeDropdown') showTimeDropdown = true;
  @Input('disabled') disabled = false;
  @Input('initialTime') initialTime: string;
  @Input('minuteStep') minuteStep: number;

  minHour: number;
  maxHour: number;
  hours: number[];
  minutes: number[];

  currentHour = '';
  currentMinute = '';
  currentMerdian = '';

  showHourDropdown = false;
  showMinuteDropdown = false;

  onTouchedCallback: () => void;
  onChangeCallback: (_: any) => void;

  get innerValue(): string {
    return this._innerValue;
  }

  set innerValue(value: string) {
    this.writeValue(value);
  }

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    private numberPipe: DecimalPipe) {
    super(elRef, renderer);
  }

  private _innerValue: string;
  private radix = 10;

  ngOnInit() {
    if (this.onTouchedCallback == null) {
      this.onTouchedCallback = () => { };
    }
    if (this.onChangeCallback == null) {
      this.onChangeCallback = (_: any) => { };
    }

    this.initialTimeSetup();
  }

  /** $event listeners */
  prevDefault($event: any) {
    $event.preventDefault();
  }

  // Note: We have this wrapped in a setTimeout to just ever so slightly delay the dropdown from being hidden
  // so we can register the user click
  onInputFocus(timeType: string, focusType: string) {
    setTimeout(() => {
      if (timeType === 'hour') {
        this.showHourDropdown = focusType === 'in';
      } else {
        this.showMinuteDropdown = focusType === 'in';
      }
    }, 150);
  }

  onInputWheel(type: string, $event: any) {
    $event.preventDefault();

    if (this.disabled) {
      return;
    }

    if ($event.deltaY < 0) {
      if (type === 'hour') {
        this.increaseHour(this.currentHour, this.currentMinute);
      } else {
        this.increaseMinute(this.currentHour, this.currentMinute);
      }
    } else {
      if (type === 'hour') {
        this.decreaseHour(this.currentHour, this.currentMinute);
      } else {
        this.decreaseMinute(this.currentHour, this.currentMinute);
      }
    }
  }

  onInputChange(type: string, $event: any) {
    if (type === 'hour') {
      this.updateHour($event.target.value);
    } else {
      this.updateMinute($event.target.value);
    }
  }

  onHourClick(val: string) {
    this.updateHour(val);
  }

  onMinuteClick(val: string) {
    this.updateMinute(val);
  }

  // This is where the validation for determining whether a user can enter a given value into the appropiate input field takes place
  onInputKeyDown(type: string, $event: any) {
    const val: string = $event.target.value;
    if (this.isNonNumericCharacter(this.convertKeyCodeToValue($event.which, $event.shiftKey))) {
      $event.preventDefault();
    }

    // Note: We only want to run this validation if the user is entering a number, otherwise hitting the tab
    //       or enter key, for example, would be prevented.
    const numericRegex = /[0-9]+$/g;
    if (this.isGreaterThanMaxLength(val, 2) && numericRegex.test(this.convertKeyCodeToValue($event.which, $event.shiftKey))) {
      $event.preventDefault();
    }

    if (type === 'hour' && this.isInvalidHourInput(val, this.convertKeyCodeToValue($event.which, $event.shiftKey))) {
      $event.preventDefault();
    }

    if (type === 'minute' && this.isInvalidMinuteInput(val, this.convertKeyCodeToValue($event.which, $event.shiftKey))) {
      $event.preventDefault();
    }
  }

  onArrowUp(type: string, $event: any) {
    $event.preventDefault();

    if (this.disabled) {
      return;
    }

    if (type === 'hour') {
      this.increaseHour(this.currentHour, this.currentMinute);
    } else {
      this.increaseMinute(this.currentHour, this.currentMinute);
    }
  }

  onArrowDown(type: string, $event: any) {
    $event.preventDefault();

    if (this.disabled) {
      return;
    }

    if (type === 'hour') {
      this.decreaseHour(this.currentHour, this.currentMinute);
    } else {
      this.decreaseMinute(this.currentHour, this.currentMinute);
    }
  }

  toggleMerdian() {
    if (!this.disabled) {
      this.currentMerdian = this.currentMerdian === 'AM' ? 'PM' : 'AM';
      this.innerValue = this.compileTime(this.currentHour, this.currentMinute, this.currentMerdian);
    }
  }
  /** End $event listeners */

  /** ControlValueAccessor interface */
  writeValue(value: string) {
    if (value !== undefined && !this.valueIsEqual(value)) {
      this._innerValue = value;
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
  }
  /** End ControlValueAccessor interface */

  private updateHour(val: string) {
    this.currentHour = val === null || val === '' ? '' : val;
    this.innerValue = this.compileTime(this.currentHour, this.currentMinute, this.currentMerdian);
  }

  private updateMinute(val: string) {
    this.currentMinute = val === null || val === '' ? '' : val;
    this.innerValue = this.compileTime(this.currentHour, this.currentMinute, this.currentMerdian);
  }

  private compileTime(hour: string, minutes: string, merdian: string): string {
    let hourVal = isNaN(parseInt(hour, this.radix)) ? 0 : parseInt(hour, this.radix);
    const minuteVal = isNaN(parseInt(minutes, this.radix)) ? 0 : parseInt(minutes, this.radix);

    if (merdian === 'PM' && hourVal < 12) {
      hourVal = hourVal + 12;
    } else if (merdian === 'AM' && hourVal === 12) {
      hourVal = 0;
    }

    return `${hourVal}:${this.numberPipe.transform(minuteVal, '2.0-0')}`;
  }

  private decompileTime(time: string, use12HourClock: boolean = false): [number, number, string] {
    const minuteVal = parseInt(time.split(':')[1], this.radix);
    let hourVal = parseInt(time.split(':')[0], this.radix);
    let merdian;

    if (use12HourClock && hourVal >= 13) {
      hourVal = hourVal - 12;
      merdian = 'PM';
    } else if (use12HourClock) {
      merdian = 'AM';
    }

    return [hourVal, minuteVal, merdian];
  }

  private initialTimeSetup() {
    this.minHour = this.use12HourClock ? 1 : 0;
    this.maxHour = this.use12HourClock ? 12 : 23;

    // If we're using a 12hr clock, set default merdian to 'PM', otherwise leave blank
    this.currentMerdian = this.use12HourClock ? 'PM' : '';

    this.hours = Array.from(Array(this.maxHour).keys()).map(t => t + 1);
    this.minutes = Array.from(Array(59).keys()).map(t => t + 1);
    this.minutes.unshift(0);

    if (this.minuteStep) {
      this.minutes = this.minutes.filter(t => t % this.minuteStep === 0);
    }

    if (!this.use12HourClock) {
      this.hours.unshift(0);
    }

    const validTimeRegex = /^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$/;
    if (this.initialTime && validTimeRegex.test(this.initialTime)) {
      this._innerValue = this.initialTime;
      const decompiledTime = this.decompileTime(this.initialTime, this.use12HourClock);
      this.updateHour(decompiledTime[0].toString());
      this.updateMinute(decompiledTime[1].toString());
      this.currentMerdian = decompiledTime[2];
    }
  }

  private increaseHour(hour: string, minute: string) {
    let hourVal = parseInt(hour, this.radix);
    hourVal = isNaN(hourVal) ? this.hours[0] : hourVal;

    let minuteVal = parseInt(minute, this.radix);
    minuteVal = isNaN(minuteVal) ? this.minutes[0] : minuteVal;

    let nextValue = this.hours.find(t => t > hourVal);
    nextValue = nextValue === undefined ? this.hours[0] : nextValue;

    this.updateHour(`${nextValue}`);
    this.updateMinute(`${minuteVal}`);

    if (this.use12HourClock && nextValue === 12) {
      this.toggleMerdian();
    }
  }

  private decreaseHour(hour: string, minute: string) {
    let hourVal = parseInt(hour, this.radix);
    hourVal = isNaN(hourVal) ? this.hours[this.hours.length - 1] : hourVal;

    let minuteVal = parseInt(minute, this.radix);
    minuteVal = isNaN(minuteVal) ? this.minutes[0] : minuteVal;

    let nextValue = this.hours.slice().reverse().find(t => t < hourVal);
    nextValue = nextValue === undefined ? this.hours[this.hours.length - 1] : nextValue;

    this.updateHour(`${nextValue}`);
    this.updateMinute(`${minuteVal}`);

    if (this.use12HourClock && hourVal === 12) {
      this.toggleMerdian();
    }
  }

  private increaseMinute(hour: string, minute: string) {
    const minuteVal = parseInt(minute, this.radix);
    let nextValue = this.minutes.find(t => t > minuteVal);
    nextValue = nextValue === undefined ? this.minutes[0] : nextValue;

    if (isNaN(minuteVal) || minuteVal >= this.minutes[this.minutes.length - 1]) {
      this.increaseHour(hour, `${this.minutes[0]}`);
    } else {
      this.updateMinute(`${nextValue}`);
    }
  }

  private decreaseMinute(hour: string, minute: string) {
    const minuteVal = parseInt(minute, this.radix);
    let nextValue = this.minutes.slice().reverse().find(t => t < minuteVal);
    nextValue = nextValue === undefined ? this.minutes[this.minutes.length - 1] : nextValue;

    if (isNaN(minuteVal) || minuteVal === 0) {
      this.decreaseHour(hour, `${this.minutes[this.minutes.length - 1]}`);
    } else {
      this.updateMinute(`${nextValue}`);
    }
  }

  private valueIsEqual(value: string) {
    return value != null && this._innerValue != null ? value === this._innerValue : false;
  }

  private isNonNumericCharacter(val: string): boolean {
    const nonNumericRegex = /[^0-9]+$/g;
    return val === null ? false : nonNumericRegex.test(val);
  }

  private isGreaterThanMaxLength(val: string, maxLength: number): boolean {
    // Value shouldn't be more than two characters, regardless if using 12 or 24 hour clock
    if (val.trim().length >= maxLength) {
      return false;
    } else {
      return true;
    }
  }

  private isInvalidHourInput(currentVal: string, nextVal: string) {
    const selectionStart = this.hourInput.nativeElement.selectionStart;
    const selectionEnd = this.hourInput.nativeElement.selectionEnd;
    const combinedValue = this.combineCurrentAndNextVal(currentVal, nextVal, selectionStart, selectionEnd);
    return this.use12HourClock ? parseInt(combinedValue, this.radix) >= 13 : parseInt(combinedValue, this.radix) >= 24;
  }

  private isInvalidMinuteInput(currentVal: string, nextVal: string) {
    const selectionStart = this.minuteInput.nativeElement.selectionStart;
    const selectionEnd = this.minuteInput.nativeElement.selectionEnd;
    const combinedValue = this.combineCurrentAndNextVal(currentVal, nextVal, selectionStart, selectionEnd);
    return parseInt(combinedValue, this.radix) >= 60;
  }

  // See key code list : https://css-tricks.com/snippets/javascript/javascript-keycodes/
  // We only want to process printable characters. Anything like a 'tab' or 'arrow up' key should be returned as null
  // so the user isn't blocked from using them
  // Note: subtracting 48 from the keycode event if the given condition is met allows use of the numpad
  private convertKeyCodeToValue(val: number, shiftKey: boolean): string {

    // Make sure we catch shift key usage, e.g., shift+1 = !, not 1 as the keycode would incorrectly indicate
    if (shiftKey && val >= 48 && val <= 57) {
      return String.fromCharCode(65);
    }

    return val >= 8 && val <= 46 && val !== 32 ? null : String.fromCharCode(val >= 96 && val <= 105 ? val - 48 : val);
  }

  private combineCurrentAndNextVal(currentVal: string, nextVal, start: number, end: number): string {
    let combinedValue;

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
