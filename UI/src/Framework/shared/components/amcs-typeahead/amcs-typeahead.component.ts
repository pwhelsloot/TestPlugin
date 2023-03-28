import { AfterViewInit, Component, ElementRef, EventEmitter, forwardRef, Input, OnChanges, OnInit, Output, Renderer2, SimpleChanges, ViewChild } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsTypeaheadConfig } from '@shared-module/components/amcs-typeahead/amcs-typeahead-config.model';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { TypeaheadDirective } from 'ngx-bootstrap/typeahead';

/**
 * @deprecated Marked for removal, use app-amcs-select-typeahead
 */
@Component({
  selector: 'app-amcs-typeahead',
  templateUrl: './amcs-typeahead.component.html',
  styleUrls: ['./amcs-typeahead.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsTypeaheadComponent),
      multi: true
    }
  ]
})
export class AmcsTypeaheadComponent extends AutomationLocatorDirective implements OnInit, OnChanges, AfterViewInit, ControlValueAccessor {
  edited: boolean;
  // activeItemIndex allows us to style the active item correctly see html .active class.
  activeItemIndex;

  get innerValue(): any {
    return this._innerValue;
  }

  set innerValue(value: any) {
    this.writeValue(value);
  }

  @Input('config') config: AmcsTypeaheadConfig;
  @Input('customClass') customClass: string;
  @Input('inputTooltip') inputTooltip: string;
  @Input('isDisabled') isDisabled: boolean;
  @Input('hasError') hasError: boolean;
  @Input('autocomplete') autocomplete: string;
  @Input('placeholder') placeholder: string;
  @Input('autoFocus') autoFocus = false;
  @ViewChild('input') inputElement: ElementRef;
  @ViewChild(TypeaheadDirective) typeahead: TypeaheadDirective;

  @Output('itemSelected') itemSelected: EventEmitter<any> = new EventEmitter<any>();
  @Output('itemHighlighted') itemHighlighted: EventEmitter<any> = new EventEmitter<any>();
  @Output('textChanged') textChanged: EventEmitter<string> = new EventEmitter<string>();

  onTouchedCallback: () => void;
  onChangeCallback: (_: any) => void;

  constructor(protected elRef: ElementRef, protected renderer: Renderer2) {
    super(elRef, renderer);
    this._innerValue = null;
  }

  private _innerValue: string;

  ngOnInit() {
    this.edited = false;
    if (this.hasError == null) {
      this.hasError = false;
    }
    if (this.onTouchedCallback == null) {
      this.onTouchedCallback = () => { };
    }
    if (this.onChangeCallback == null) {
      this.onChangeCallback = (_: any) => { };
    }
    this.refreshConfig();
  }

  ngAfterViewInit() {
    if (this.customClass != null) {
      const classes = this.customClass.split(' ');
      classes.forEach(element => {
        this.renderer.addClass(this.inputElement.nativeElement, element);
      });
    }
  }

  // Needs to be on key-up as key-down is used by typeahead to actually
  // move active item.
  keyUp(e: any) {
    // up
    if (e.keyCode === 38) {
      e.preventDefault();
      this.decrementActiveItem();
      return;
    }
    // down
    if (e.keyCode === 40) {
      e.preventDefault();
      this.incrementActiveItem();
      return;
    }
  }

  // Stops text cursor jumping to start/end
  keyDown(e: any) {
    // up
    if (e.keyCode === 38) {
      e.preventDefault();
      return;
    }
    // down
    if (e.keyCode === 40) {
      e.preventDefault();
      return;
    }
  }

  // manual click
  selectMatch(match) {
    this.typeahead._container.selectMatch(match);
    this.onTouchedCallback();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['config']) {
      this.refreshConfig();
    }
  }

  /** ControlValueAccessor interface */
  writeValue(value: string) {
    if (value == null || (this._innerValue !== null && value === '')) {
      this.resetState();
    } else if (value !== undefined && value != null && !this.valueIsEqual(value)) {
      this._innerValue = value;
      this.edited = true;
      this.activeItemIndex = null;
      if (!this.valueInMatches()) {
        this.textChanged.emit(this._innerValue);
      }
      this.onChangeCallback(this._innerValue);
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

  private resetState() {
    this._innerValue = null;
    this.activeItemIndex = null;
    this.edited = false;
    this.onChangeCallback(this._innerValue);
  }

  private incrementActiveItem() {
    if (this.typeahead._container != null) {
      // The typeahead control always selects the top item as active, this a hack saying
      // on the first time user hits arrow-down we actually need to go back one as we'll now
      // be on item two
      if (this.activeItemIndex == null) {
        this.typeahead._container.prevActiveMatch();
      }
      this.activeItemIndex = this.typeahead.matches.indexOf(this.typeahead._container.active);
      this.itemHighlighted.next(this.typeahead._container.active.item);
    }
  }

  private decrementActiveItem() {
    if (this.typeahead._container != null) {
      this.activeItemIndex = this.typeahead.matches.indexOf(this.typeahead._container.active);
      this.itemHighlighted.next(this.typeahead._container.active.item);
    }
  }

  // Checks if inner value is in matches array
  private valueInMatches(): boolean {
    if (isTruthy(this.typeahead)) {
      if (this.typeahead.matches == null) {
        return false;
      } else {
        return this.typeahead.matches.some(x => x.value === this._innerValue);
      }
    } else {
      return false;
    }
  }

  private valueIsEqual(value: any) {
    if (value != null && this._innerValue != null) {
      return value === this._innerValue;
    } else {
      return value === this._innerValue;
    }
  }

  private refreshConfig() {
    if (this.config == null) {
      this.config = new AmcsTypeaheadConfig();
    }
    // Sets some defaults but any config properties will take presentence
    this.config = {
      typeaheadAsync: false,
      typeaheadMinLength: 1,
      typeaheadOptionsLimit: 7,
      ...this.config
    };
  }
}
