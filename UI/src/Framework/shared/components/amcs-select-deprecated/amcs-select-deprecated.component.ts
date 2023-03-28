import { AfterContentInit, AfterViewInit, Component, ElementRef, forwardRef, Input, OnDestroy, Renderer2, ViewChild } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { GlossaryService } from '@core-module/services/glossary/glossary.service';
import { FormControlDisplay } from '@coremodels/forms/form-control-display.enum';
import { AmcsSelectHelper } from '@shared-module/components/helpers/amcs-select-helper';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Subscription } from 'rxjs';

/**
 * @deprecated Use app-amcs-select instead
 */
@Component({
  selector: 'app-amcs-select-deprecated',
  templateUrl: './amcs-select.component.html',
  styleUrls: ['./amcs-select.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsSelectDeprecatedComponent),
      multi: true
    }
  ]
})
export class AmcsSelectDeprecatedComponent extends AutomationLocatorDirective implements AfterContentInit, AfterViewInit, OnDestroy {
  edited = false;
  value: any;
  selectTranslation: string;
  FormControlDisplay = FormControlDisplay;

  @Input('isDisabled') isDisabled: boolean;
  @Input('isSortable') isSortable = true;
  @Input('isSecondaryUi') isSecondaryUi = false;
  @Input('customClass') customClass: string;
  @Input('hasError') hasError = false;
  @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;
  @Input('isOptional') isOptional: boolean;
  @Input('selectTooltip') selectTooltip: string;
  @Input('isNumber') isNumber = false;
  @Input() autoFocus = false;
  @ViewChild('select') selectElement: ElementRef;

  constructor(protected elRef: ElementRef, protected renderer: Renderer2, private readonly appTranslationsService: SharedTranslationsService, private readonly glossaryService: GlossaryService) {
    super(elRef, renderer);
  }

  private translationSubscription: Subscription;

  onTouchedCallback: () => void = () => { };
  onChangeCallback: (_: any) => void = (_: any) => { };

  ngAfterViewInit() {
    if (this.customClass != null) {
      const classes = this.customClass.split(' ');
      classes.forEach(element => {
        this.renderer.addClass(this.selectElement.nativeElement, element);
      });
    }
    if (this.isSortable) {
      AmcsSelectHelper.doDeprecatedSort(this.selectElement.nativeElement);
    }
    setTimeout(() => {
      this.translationSubscription = this.appTranslationsService.translations.subscribe((translations: string[]) => {
        this.selectTranslation = translations['searchselect.select'];
      });
    }, 0);
  }

  ngAfterContentInit() {
     AmcsSelectHelper.applyGlossaryTranslations(this.elRef, this.glossaryService);
  }

  ngOnDestroy() {
    if (this.translationSubscription) {
      this.translationSubscription.unsubscribe();
    }
  }

  /** ControlValueAccessor interface */
  writeValue(value: any) {
    this.value = value;
    if (this.value === null) {
      this.writeNullValue();
    } else {
      this.edited = true;
      if (this.isNumber) {
        this.onChangeCallback(+this.value);
      } else {
        this.onChangeCallback(this.value);
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
