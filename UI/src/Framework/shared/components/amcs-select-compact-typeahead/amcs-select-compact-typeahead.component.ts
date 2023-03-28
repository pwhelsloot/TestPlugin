import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  forwardRef,
  Input,
  OnDestroy,
  OnInit,
  Output,
  Renderer2,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { isTruthy } from '@corehelpers/is-truthy.function';
import { NgSelectComponent } from '@ng-select/ng-select';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { BehaviorSubject, Observable, Subject, Subscription } from 'rxjs';
import { filter, take } from 'rxjs/operators';

/**
 * @todo Move AllowCreation logic to amcs-select-typeahead and deprecate this one
 */
@Component({
  selector: 'app-amcs-select-compact-typeahead',
  templateUrl: './amcs-select-compact-typeahead.component.html',
  styleUrls: ['./amcs-select-compact-typeahead.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsSelectCompactTypeaheadComponent),
      multi: true
    }
  ],
  encapsulation: ViewEncapsulation.None
})
export class AmcsSelectCompactTypeaheadComponent extends AutomationLocatorDirective implements OnInit, AfterViewInit, OnDestroy {
  edited = false;
  value: any;
  typeToSearchText: string;
  notFoundText: string;
  loadingText: string;
  FormControlDisplay = FormControlDisplay;

  @Input() items$: Observable<any[]>;
  @Input() items: any[];
  @Input() bindLabel: string;
  @Input() bindValue: string = null;
  @Input() inputSubject: Subject<string>;
  @Input() groupBy: string;
  @Input() isDisabled = false;
  @Input() appendToBody = true;
  @Input() hasError = false;
  @Input() loading = false;
  @Input() autoFocus = false;
  @Input() allowCreation = false;
  @Input() selectOnTab: boolean;
  @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;
  @Input() appendTo = 'body';
  @Input() editableSearchTerm = false;
  @Input() customNoResultsFoundMessage: string;
  // eslint-disable-next-line @angular-eslint/no-output-native
  @Output() change = new EventEmitter<any>();
  // eslint-disable-next-line @angular-eslint/no-output-native
  @Output() blur = new EventEmitter<any>();
  @Output() onClose = new EventEmitter<any>();

  @ViewChild('select') selector: NgSelectComponent;

  constructor(protected elRef: ElementRef, protected renderer: Renderer2, private appTranslationsService: SharedTranslationsService) {
    super(elRef, renderer);
  }

  private translationSubscription: Subscription;
  private selectorReady = new BehaviorSubject<boolean>(false);
  private closeSubscription: Subscription;
  private typedTextSubscription: Subscription;
  private changeSubscription: Subscription;
  private clearSubscription: Subscription;
  private itemSelected = false;
  private typedText: string;

  onTouchedCallback: () => void = () => { };
  // Should maybe write call changeCallbacks like this, the problem is if this
  // control isn't being used in a form or [(ngModel)] the callback never gets assigned! This
  // means we never get a 'change' event so this code fires one manually
  onChangeCallback: (_: any) => void = (_: any) => {
    this.change.emit(_);
  };

  ngOnInit() {
    this.translationSubscription = this.appTranslationsService.translations.subscribe((translations: string[]) => {
      this.typeToSearchText = translations['selectTypeahead.typeToSearch'];
      this.notFoundText = this.allowCreation
        ? translations['selectTypeahead.itemToCreate']
        : (isTruthy(this.customNoResultsFoundMessage) ? this.customNoResultsFoundMessage : translations['selectTypeahead.noResultsFound']);
      this.loadingText = translations['selectTypeahead.loadingText'];
    });
  }

  ngAfterViewInit() {
    setTimeout(() => {
      this.selector.typeahead = this.inputSubject;
      if (this.autoFocus) {
        this.selector.focus();
      }
      if (this.allowCreation) {
        this.setupCreationMode();
      }
      this.selectorReady.next(true);
    }, 0);
  }

  ngOnDestroy() {
    this.translationSubscription.unsubscribe();
    if (this.allowCreation) {
      if (isTruthy(this.closeSubscription)) {
        this.closeSubscription.unsubscribe();
      }
      if (isTruthy(this.changeSubscription)) {
        this.changeSubscription.unsubscribe();
      }
      if (isTruthy(this.clearSubscription)) {
        this.clearSubscription.unsubscribe();
      }
      if (isTruthy(this.typedTextSubscription)) {
        this.typedTextSubscription.unsubscribe();
      }
    }
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

  onDropDownClose($event) {
    this.onClose.emit($event);
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
  }

  registerOnChange(fn: any) {
    this.onChangeCallback = fn;
  }

  registerOnTouched(fn: any) {
    this.onTouchedCallback = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.isDisabled = isDisabled;
    this.selectorReady
      .pipe(
        filter((x) => x),
        take(1)
      )
      .subscribe(() => {
        this.selector.setDisabledState(this.isDisabled);
      });
  }

  private setupCreationMode() {
    // Sadly ng-select clears our 'inputSubject' before 'closeEvent' fires so we must track
    // what the user has typed
    this.typedTextSubscription = this.inputSubject.subscribe((typedText: string) => {
      if (isTruthy(typedText)) {
        this.typedText = typedText;
      }
      if (this.editableSearchTerm && typedText == null) {
        this.selector.handleClearClick();
      }
    });

    // On close if we've not selected an item then create and and select it manually
    this.closeSubscription = this.selector.closeEvent.subscribe(() => {
      if (!this.itemSelected && isTruthy(this.typedText)) {
        const newItem = {};
        newItem[isTruthy(this.bindValue) ? this.bindValue : 'id'] = 0;
        newItem[this.bindLabel] = this.typedText;
        this.writeValue(newItem);
      }
      this.itemSelected = false;
      this.typedText = null;
    });

    // On clear we must wipe the typed text
    this.clearSubscription = this.selector.clearEvent.subscribe(() => {
      this.typedText = null;
      this.itemSelected = false;
    });

    // Listen for an actual item selection
    this.changeSubscription = this.selector.changeEvent.subscribe(() => {
      this.itemSelected = true;
    });
  }

  /** End ControlValueAccessor interface */
}
