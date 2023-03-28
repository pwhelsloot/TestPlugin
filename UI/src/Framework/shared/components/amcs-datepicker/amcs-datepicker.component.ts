import {
  Component,
  ElementRef,
  forwardRef,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Optional,
  Renderer2,
  SimpleChanges,
  ViewChild
} from '@angular/core';
import { ControlContainer, ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { FormControlDisplay } from '@coremodels/forms/form-control-display.enum';
import { AmcsDatepickerConfig } from '@shared-module/components/amcs-datepicker/amcs-datepicker-config.model';
import { AmcsFormControlBaseComponent } from '@shared-module/forms/amcs-form-control-base';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { defineLocale, enGbLocale, esUsLocale, frLocale, nbLocale, nlLocale } from 'ngx-bootstrap/chronos';
import { BsDatepickerConfig, BsDatepickerDirective, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { Subject, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { DatePickerFormat } from './date-picker-format.enum';

defineLocale('en');
defineLocale('en-gb', enGbLocale);
defineLocale('nb', nbLocale);
defineLocale('es-us', esUsLocale);
defineLocale('nl', nlLocale);
defineLocale('fr-fr', frLocale);

@Component({
  selector: 'app-amcs-datepicker',
  templateUrl: './amcs-datepicker.component.html',
  styleUrls: ['./amcs-datepicker.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsDatepickerComponent),
      multi: true,
    },
  ],
})
export class AmcsDatepickerComponent extends AmcsFormControlBaseComponent implements OnInit, OnChanges, ControlValueAccessor, OnDestroy {
  edited = false;
  value: any;
  ngxConfig: BsDatepickerConfig;
  FormControlDisplay = FormControlDisplay;

  @Input('customClass') customClass: string;
  @Input('config') config = new AmcsDatepickerConfig();
  @Input('placement') placement = 'bottom';
  @Input() isSecondaryColor = false;
  @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;
  @Input('autocomplete') autocomplete = 'off';
  @Input('dateTooltip') dateTooltip: string;
  @Input('disabled') disabled = false;
  // Readonly to prevent manual input
  @Input('readonly') readonly = false;
  @Input() previewChanges: Subject<any>;
  @Input() hasFieldDefs = false;
  @Input() errors: string = null;
  @Input('autoFocus') autoFocus = false;
  @ViewChild('dp') datePickerElement: BsDatepickerDirective;
  @Input() noMargin = false;
  @Input() noPadding = false;

  toggleSubject = new Subject();

  constructor(
    protected elRef: ElementRef,
    protected renderer: Renderer2,
    private translateSettingService: TranslateSettingService,
    private localeService: BsLocaleService,
    @Optional() readonly controlContainer: ControlContainer
  ) {
    super(controlContainer, elRef, renderer);
    this.setLocale();
  }

  private selectedLanguageSubscription: Subscription;
  private toggleSubscription: Subscription;

  onTouchedCallback: () => void = () => {};
  onChangeCallback: (_: any) => void = (_: any) => {};

  ngOnInit() {
    this.refreshConfig();
    this.toggleSubscription = this.toggleSubject.pipe(debounceTime(300)).subscribe(() => {
      this.datePickerElement.toggle();
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['config']) {
      this.refreshConfig();
    }
  }

  ngOnDestroy() {
    if (this.selectedLanguageSubscription) {
      this.selectedLanguageSubscription.unsubscribe();
    }

    if (this.toggleSubscription) {
      this.toggleSubscription.unsubscribe();
    }
    super.ngOnDestroy();
  }

  writeValueFromView(value: Date) {
    event.stopPropagation();
    if (isTruthy(this.previewChanges)) {
      // We need this.datePickerElement.bsValue to keep view inline with our
      // value but sadly this fires the change event again so just need this check
      // to not get caught in loop
      if (this.value !== value) {
        const newValue: Date = isTruthy(value) ? AmcsDate.createFrom(value) : null;
        this.previewChanges.next(newValue);
        if (isTruthy(this.datePickerElement)) {
          this.datePickerElement.bsValue = this.value;
        }
      }
    } else {
      this.writeValue(value);
    }
  }

  /** ControlValueAccessor interface */
  writeValue(value: Date | string) {
    if (!isTruthy(value)) {
      this.writeNullValue();
    } else {
      this.edited = true;
      const validValue: Date = AmcsDate.createFrom(value);
      if (validValue !== null) {
        this.value = this.checkWithinMinMax(validValue);
        this.onChangeCallback(this.value);
        this.onTouchedCallback();
      } else {
        this.writeNullValue();
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
    this.ngxConfig = { ...this.ngxConfig, isDisabled: this.disabled };
  }

  onTab() {
    this.datePickerElement.hide();
  }

  forShiftTab($event: KeyboardEvent) {
    if ($event.shiftKey && $event.key === 'Tab') {
      this.datePickerElement.hide();
    }
  }

  toggle() {
    if (!this.disabled && !this.ngxConfig.isDisabled) {
      this.toggleSubject.next();
    }
  }

  openCal() {
    if (!this.disabled && !this.ngxConfig.isDisabled && !this.datePickerElement.isOpen) {
      this.datePickerElement.show();
    }
  }

  onOpenCalendar(container: any) {
    if (this.config.dateInputFormat && this.config.dateInputFormat === DatePickerFormat.month) {
      container.monthSelectHandler = (event: any): void => {
        container._store.dispatch(container._actions.select(event.date));
      };
      container.setViewMode('month');
    }
  }

  /** END ControlValueAccessor interface */

  private setLocale() {
    this.selectedLanguageSubscription = this.translateSettingService.selectedLanguage.subscribe((language: string) => {
      let locale = 'en';
      switch (language) {
        case 'en-US':
          locale = 'en';
          break;
        case 'en-GB':
          locale = 'en-gb';
          break;
        case 'es-MX':
          // No es-MX local in NGX-Bootstrap/Chronos so using es-US for this component
          locale = 'es-us';
          break;
        case 'nb':
          locale = 'nb';
          break;
        case 'en-AU':
          locale = 'en-gb';
          break;
        case 'nl':
          locale = 'nl';
          break;
        case 'fr-FR':
          locale = 'fr-fr';
          break;
      }
      this.localeService.use(locale);
    });
  }

  private checkWithinMinMax(dateSelected: Date): Date {
    if (this.ngxConfig.minDate && dateSelected < this.ngxConfig.minDate) {
      return this.ngxConfig.minDate;
    }
    if (this.ngxConfig.maxDate && dateSelected > this.ngxConfig.maxDate) {
      return this.ngxConfig.maxDate;
    }
    return dateSelected;
  }

  private refreshConfig() {
    this.ngxConfig = new BsDatepickerConfig();
    this.ngxConfig.minDate = this.config.minDate ? this.config.minDate : this.ngxConfig.minDate;
    this.ngxConfig.maxDate = this.config.maxDate ? this.config.maxDate : this.ngxConfig.maxDate;
    this.ngxConfig.containerClass = this.config.containerClass ? this.config.containerClass : 'amcs-datepicker';
    this.ngxConfig.dateInputFormat = this.config.dateInputFormat ? this.config.dateInputFormat : this.ngxConfig.dateInputFormat;
    this.ngxConfig.daysDisabled = this.config.daysDisabled ? this.config.daysDisabled : this.ngxConfig.daysDisabled;
    this.ngxConfig.selectFromOtherMonth = this.config.selectFromOtherMonth ? this.config.selectFromOtherMonth : true;
  }

  private writeNullValue() {
    this.edited = false;
    this.value = null;
    this.onChangeCallback(null);
    this.onTouchedCallback();
  }
}
