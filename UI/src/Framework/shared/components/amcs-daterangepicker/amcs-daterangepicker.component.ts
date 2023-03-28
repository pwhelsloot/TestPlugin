
import { Component, ElementRef, forwardRef, Input, OnChanges, OnDestroy, OnInit, Renderer2, SimpleChanges } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { AmcsDatepickerConfig } from '@shared-module/components/amcs-datepicker/amcs-datepicker-config.model';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { defineLocale, enGbLocale, esUsLocale, frLocale, nbLocale, nlLocale } from 'ngx-bootstrap/chronos';
import { BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { Subscription } from 'rxjs';

defineLocale('en');
defineLocale('en-gb', enGbLocale);
defineLocale('nb', nbLocale);
defineLocale('es-us', esUsLocale);
defineLocale('nl', nlLocale);
defineLocale('fr-fr', frLocale);

/**
 * @deprecated Marked for removal, use the @class AmcsDatePickerComponent instead
 */
@Component({
  selector: 'app-amcs-daterangepicker',
  templateUrl: './amcs-daterangepicker.component.html',
  styleUrls: ['./amcs-daterangepicker.component.scss'],

  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsDaterangepickerComponent),
      multi: true
    }
  ]
})
export class AmcsDaterangepickerComponent extends AutomationLocatorDirective implements OnInit, OnChanges, OnDestroy, ControlValueAccessor {
  ngxConfig: BsDaterangepickerConfig;
  edited = false;
  value: any;

  @Input('config') config: AmcsDatepickerConfig;
  @Input('hasError') hasError = false;
  @Input('dateTooltip') dateTooltip: string;
  @Input() disabled = false;
  @Input() placeholder: string;

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    private translateSettingService: TranslateSettingService, private localeService: BsLocaleService) {
    super(elRef, renderer);
    this.selectedLanguageSubscription = this.translateSettingService.selectedLanguage
      .subscribe((language: string) => {
        let locale = 'en';
        switch (language) {
          case 'en-US':
            locale = 'en';
            break;
          case 'en-GB':
            locale = 'en-GB';
            break;
          case 'nb':
            locale = 'nb';
            break;
          case 'es-MX':
            // No es-MX local in NGX-Bootstrap/Chronos so using es-US for this component
            locale = 'es-us';
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

  private selectedLanguageSubscription: Subscription;

  onTouchedCallback: () => void = () => { };
  onChangeCallback: (_: any) => void = (_: any) => { };

  ngOnInit() {
    this.refreshConfig();
  }

  ngOnChanges(changes: SimpleChanges) {
    this.refreshConfig();
  }

  ngOnDestroy() {
    this.selectedLanguageSubscription.unsubscribe();
  }

  /** ControlValueAccessor interface */

  writeValue(value: Date[] | string) {
    if (!isTruthy(value) || value.length !== 2 && !isTruthy(value[0]) || !isTruthy(value[1])) {
      this.writeNullValue();
    } else {
      this.edited = true;
      const startDate = AmcsDate.createFrom(value[0]);
      const endDate = AmcsDate.createFrom(value[1]);
      if (startDate !== null && endDate !== null) {
        this.value = new Array<Date>(startDate, endDate);
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

  /** End ControlValueAccessor interface */

  private refreshConfig() {
    if (this.ngxConfig == null) {
      this.ngxConfig = new BsDaterangepickerConfig();
    }
    // Sets some defaults but any config properties will take presentence
    this.ngxConfig = {
      ...this.ngxConfig,
      displayMonths: 2
      // , ...this.config
    };
  }

  private writeNullValue() {
    this.edited = false;
    this.value = null;
    this.onChangeCallback(null);
  }

}
