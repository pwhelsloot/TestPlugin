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
    ViewChild
} from '@angular/core';
import { ControlContainer, NG_VALUE_ACCESSOR } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { FormControlDisplay } from '@coremodels/forms/form-control-display.enum';
import { IGroupedLookupItem } from '@coremodels/lookups/grouped-lookup-item.interface';
import { ILookupItem } from '@coremodels/lookups/lookup-item.interface';
import { AmcsFormControlBaseComponent } from '@shared-module/forms/amcs-form-control-base';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Subject, Subscription } from 'rxjs';
import { AmcsSelectHelper } from '../helpers/amcs-select-helper';

@Component({
  selector: 'app-amcs-select',
  templateUrl: './amcs-select.component.html',
  styleUrls: ['./amcs-select.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsSelectComponent),
      multi: true
    }
  ]
})
export class AmcsSelectComponent extends AmcsFormControlBaseComponent implements OnInit, AfterViewInit, OnDestroy, OnChanges {
  edited = false;
  selectTranslation: string;
  value: any;
  FormControlDisplay = FormControlDisplay;

  typeCheck = AmcsSelectHelper.areOptionsGrouped;

  @Input('isDisabled') isDisabled: boolean;
  @Input('isSecondaryUi') isSecondaryUi = false;
  @Input('customClass') customClass: string;
  @Input('customPlaceholder') customPlaceholder: string;
  @Input() isLargeSelect = false;
  @Input() loading = false;
  @Input('isOptional') isOptional: boolean;
  @Input('options') options: ILookupItem[] | IGroupedLookupItem[];
  @Input('selectTooltip') selectTooltip: string;
  @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;
  @Input() previewChanges: Subject<any>;
  @Input('keepOriginalOrder') keepOriginalOrder = false;
  @Input('useNumericValues') useNumericValues = true;
  @Input('isMobile') isMobile = false;
  @ViewChild('select') selectElement: ElementRef;
  @Input('autoFocus') autoFocus = false;
  @Input() bindLabel = 'description';
  @Input() bindValue = 'id';
  // eslint-disable-next-line @angular-eslint/no-output-native
  @Output('change') change = new EventEmitter<any>();
  // eslint-disable-next-line @angular-eslint/no-output-native
  @Output('blur') blur = new EventEmitter<any>();
  // eslint-disable-next-line @angular-eslint/no-output-native
  @Output('focus') focus = new EventEmitter<any>();
  @Input() selectTooltipIconMessage: string;
  @Input() selectTooltipIconClass: string;

  constructor(
    protected elRef: ElementRef,
    protected renderer: Renderer2,
    private appTranslationsService: SharedTranslationsService,
    @Optional() readonly controlContainer: ControlContainer
  ) {
    super(controlContainer, elRef, renderer);
  }

  private translationSubscription: Subscription;

  onTouchedCallback: () => void = () => {};
  onChangeCallback: (_: any) => void = (_: any) => {};

  ngOnInit() {
    this.translationSubscription = this.appTranslationsService.translations.subscribe((translations: string[]) => {
      this.selectTranslation = isTruthy(this.customPlaceholder) ? this.customPlaceholder : translations['searchselect.select'];
    });
  }

  ngOnChanges(change: SimpleChanges) {
    if (change && change['options'] && !this.keepOriginalOrder) {
      AmcsSelectHelper.doSort(this.options);
      this.checkSelectionValid();
    }
  }

  ngAfterViewInit() {
    if (this.customClass != null) {
      const classes = this.customClass.split(' ');
      classes.forEach((element) => {
        this.renderer.addClass(this.selectElement.nativeElement, element);
      });
    }
  }

  ngOnDestroy() {
    this.translationSubscription?.unsubscribe();
    super.ngOnDestroy();
  }

  writeValueFromView(value: any) {
    if (!isTruthy(value) || value === 'null') {
      value = null;
    }

    if (isTruthy(this.previewChanges)) {
      if (value !== null && this.useNumericValues) {
        this.previewChanges.next(+value);
      } else {
        this.previewChanges.next(value);
      }

      if (isTruthy(this.selectElement)) {
        this.selectElement.nativeElement.value = this.value;
      }
    } else {
      this.writeValue(value);
    }
  }

  /** ControlValueAccessor interface */
  writeValue(value: any) {
    this.value = value;
    if (!isTruthy(this.value)) {
      this.writeNullValue();
    } else {
      this.edited = true;

      if (this.useNumericValues) {
        this.onChangeCallback(+this.value);
      } else {
        this.onChangeCallback(this.value);
      }
    }
  }

  checkSelectionValid() {
    if (
      this.value !== null &&
      this.value !== undefined &&
      this.options !== null &&
      this.options !== undefined &&
      !AmcsSelectHelper.areOptionsGrouped(this.options)
    ) {
      let found = false;

      if (this.useNumericValues) {
        found = this.options.some((x) => x[this.bindValue] === +this.value);
      } else {
        found = this.options.some((x) => x[this.bindValue] === this.value);
      }

      if (!found) {
        // This might be affecting the form it's linked to so put onto different thread
        setTimeout(() => {
          this.writeValue(null);
        }, 0);
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

  private writeNullValue() {
    this.edited = false;
    this.onChangeCallback(null);
  }
}
