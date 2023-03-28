import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  forwardRef,
  Input,
  OnDestroy,
  OnInit,
  Optional,
  Output,
  Renderer2,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import { ControlContainer, NG_VALUE_ACCESSOR } from '@angular/forms';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { isTruthy } from '@corehelpers/is-truthy.function';
import { NgSelectComponent } from '@ng-select/ng-select';
import { AmcsFormControlBaseComponent } from '@shared-module/forms/amcs-form-control-base';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Observable, Subject, Subscription } from 'rxjs';

// TODO: Whats the difference with app-amcs-select-compact-typeahead?
@Component({
  selector: 'app-amcs-select-typeahead',
  templateUrl: './amcs-select-typeahead.component.html',
  styleUrls: ['./amcs-select-typeahead.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsSelectTypeaheadComponent),
      multi: true,
    },
  ],
  encapsulation: ViewEncapsulation.None,
})
export class AmcsSelectTypeaheadComponent extends AmcsFormControlBaseComponent implements OnInit, AfterViewInit, OnDestroy {
  edited = false;
  value: any;
  typeToSearchText: string;
  notFoundText: string;
  loadingText: string;
  FormControlDisplay = FormControlDisplay;

  @Input() items$: Observable<any[]>;
  @Input() items: any[];
  @Input() bindLabel: string;
  @Input() bindDropdownLabel: string;
  @Input() bindValue: string = null;
  @Input() inputSubject: Subject<string>;
  @Input() groupBy: string;
  @Input() isDisabled = false;
  @Input() selectableGroup = false;
  @Input() appendToBody = true;
  @Input() loading = false;
  @Input() autoFocus = false;
  @Input() hasWarning = false;
  @Input() selectOnTab: boolean;
  @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;
  @Input() appendTo = 'body';
  // eslint-disable-next-line @angular-eslint/no-output-native
  @Output() change = new EventEmitter<any>();
  // eslint-disable-next-line @angular-eslint/no-output-native
  @Output() blur = new EventEmitter<any>();
  @ViewChild('select') selector: NgSelectComponent;
  @Input() clearOnSelect = false;

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
  // Should maybe write call changeCallbacks like this, the problem is if this
  // control isn't being used in a form or [(ngModel)] the callback never gets assigned! This
  // means we never get a 'change' event so this code fires one manually
  onChangeCallback: (_: any) => void = (_: any) => {
    this.change.emit(_);
  };

  ngOnInit() {
    this.translationSubscription = this.appTranslationsService.translations.subscribe((translations: string[]) => {
      this.typeToSearchText = translations['selectTypeahead.typeToSearch'];
      this.notFoundText = translations['selectTypeahead.noResultsFound'];
      this.loadingText = translations['selectTypeahead.loadingText'];
    });
  }

  ngAfterViewInit() {
    setTimeout(() => {
      this.selector.typeahead = this.inputSubject;
      if (this.autoFocus) {
        this.selector.focus();
      }
      this.selector.setDisabledState(this.isDisabled);
    }, 0);
  }

  ngOnDestroy() {
    this.translationSubscription.unsubscribe();
    super.ngOnDestroy();
  }

  onTabPressed() {
    if (isTruthy(this.selectOnTab)) {
      if (this.selector.isOpen) {
        if (this.selector.itemsList.markedItem) {
          this.selector.toggleItem(this.selector.itemsList.markedItem);
        } else if (this.selector.addTag) {
          this.selector.selectTag();
        }
      }
    }
  }

  onBlur($event) {
    this.onTouchedCallback();
    this.blur.emit($event);
  }

  /** ControlValueAccessor interface */
  writeValue(value: any) {
    this.value = value;
    if (!this.value || this.value.length === 0) {
      this.edited = false;
      this.value = null;
    } else {
      this.edited = true;
    }
    this.onChangeCallback(this.value);
    if (this.clearOnSelect && this.edited) {
      this.selector.handleClearClick();
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
}
