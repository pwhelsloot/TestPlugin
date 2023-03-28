import { Component, ElementRef, forwardRef, Input, OnDestroy, OnInit, Optional, Renderer2, TemplateRef, ViewEncapsulation } from '@angular/core';
import { ControlContainer, NG_VALUE_ACCESSOR } from '@angular/forms';
import { BitmaskHelper } from '@core-module/helpers/bitmask-helper';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { ILookupItem } from '@coremodels/lookups/lookup-item.interface';
import { AmcsFormControlBaseComponent } from '@shared-module/forms/amcs-form-control-base';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Subscription } from 'rxjs';


/**
 * Consider using amcs-multi-select-with-search instead
 */
@Component({
  selector: 'app-amcs-multi-select',
  templateUrl: './amcs-multi-select.component.html',
  styleUrls: ['./amcs-multi-select.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsMultiSelectComponent),
      multi: true,
    },
  ],
  encapsulation: ViewEncapsulation.None,
})
export class AmcsMultiSelectComponent extends AmcsFormControlBaseComponent implements OnInit, OnDestroy {
  edited = false;
  FormControlDisplay = FormControlDisplay;
  selectTranslation: string;
  value: any;

  @Input('isDisabled') isDisabled = false;
  @Input('isCheckbox') isCheckbox = false;
  @Input('isSelectAll') isSelectAll = false;
  @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;
  @Input('valueAsBitmask') valueAsBitmask = false;
  @Input('options') options: ILookupItem[];
  @Input() rowTemplate: TemplateRef<any>;

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
      this.selectTranslation = translations['searchselect.select'];
    });
  }

  ngOnDestroy() {
    this.translationSubscription.unsubscribe();
    super.ngOnDestroy();
  }

  /** ControlValueAccessor interface */
  writeValue(value: any) {
    const isNullOrArray: boolean = !isTruthy(value) || Array.isArray(value);
    // value must always be an array for mat-select to work, if the form controls
    // supplied us a bitmask we must convert it to array
    if (isNullOrArray || !this.valueAsBitmask) {
      this.value = value;
    } else if (this.valueAsBitmask) {
      this.value = BitmaskHelper.convertBitmaskToArray(value);
    }
    if (!isTruthy(this.value) || this.value.length === 0) {
      this.writeNullValue();
    } else {
      this.edited = true;
      if (this.valueAsBitmask) {
        this.onChangeCallback(BitmaskHelper.convertArrayToBitmask(this.value));
      } else {
        this.onChangeCallback(this.value);
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
    this.isDisabled = isDisabled;
  }

  /** End ControlValueAccessor interface */

  private writeNullValue() {
    this.edited = false;
    this.onChangeCallback(null);
  }
}
