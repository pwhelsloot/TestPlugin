import { Component, ElementRef, EventEmitter, forwardRef, Injector, Input, OnDestroy, OnInit, Output, Renderer2 } from '@angular/core';
import { ControlValueAccessor, NgControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { ISelectableItem } from '@core-module/models/iselectable-item.model';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { AmcsRadioControlRegistryDirective } from '../amcs-radio-input/amcs-radio-control-registry.directive';
import { IAmcsRadioControl } from '../amcs-radio-input/amcs-radio-control.interface';

@Component({
  selector: 'app-amcs-radio-tile',
  templateUrl: './amcs-radio-tile.component.html',
  styleUrls: ['./amcs-radio-tile.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsRadioTileComponent),
      multi: true
    }]
})
export class AmcsRadioTileComponent extends AutomationLocatorDirective implements OnInit, OnDestroy, ControlValueAccessor, IAmcsRadioControl {
  state: boolean;
  @Input() label: string;
  @Input() value: any;
  @Input() radioGroupName: string;
  @Input() imagePadding = '6px';
  @Input() base64Image: string = null;
  @Input() height: number = null;
  @Input() iconClass: string = null;
  @Input() extraOptions: ISelectableItem[] = [];
  @Input('hasError') hasError = false;
  @Input('isDisabled') isDisabled: boolean;
  @Input() selectedExtraOptionId: number;
  @Input('imgClass') imgClass: string;
  @Input('customClass') customClass: string;
  @Input() customRadioTileClass: string;
  @Input() iconImgSrc: string = null;
  @Output() selectedExtraOptionIdChange: EventEmitter<number> = new EventEmitter<number>();

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    private registry: AmcsRadioControlRegistryDirective, private injector: Injector) {
    super(elRef, renderer);
  }

  private control: NgControl;

  onChange = () => { };
  onTouched = () => { };

  ngOnInit() {
    if (isTruthy(this.radioGroupName)) {
      this.formControlName = this.radioGroupName;
    }
    this.control = this.injector.get(NgControl);
    this.registry.add(this.control, this);
    if (isTruthy(this.selectedExtraOptionId)) {
      const item: ISelectableItem = this.extraOptions.find(x => x.id === this.selectedExtraOptionId);
      if (isTruthy(item)) {
        item.isSelected = true;
      }
    }
  }

  ngOnDestroy() { this.registry.remove(this); }

  writeValue(value: any): void {
    this.state = value === this.value;
    if (!this.state && this.extraOptions && this.extraOptions.length > 0) {
      this.clearOptions();
    }
  }

  registerOnChange(fn: (_: any) => {}): void {
    this.onChange = () => {
      fn(this.value);
      this.registry.select(this);
      this.onTouched();
    };
  }

  fireCheck(value: any): void { this.writeValue(value); }

  registerOnTouched(fn: () => {}): void { this.onTouched = fn; }

  setDisabledState(isDisabled: boolean): void {
    this.isDisabled = isDisabled;
  }

  selectTile() {
    if (!this.isDisabled) {
      this.onChange();
    }
  }

  selectOption(option: ISelectableItem) {
    this.extraOptions.forEach(element => {
      element.isSelected = false;
    });
    option.isSelected = true;
    this.selectedExtraOptionId = option.id;
    this.selectedExtraOptionIdChange.emit(this.selectedExtraOptionId);
  }

  afterSelection() {
    if (this.state && this.extraOptions && this.extraOptions.length > 0 && this.extraOptions.filter(x => x.isSelected).length <= 0) {
      this.selectOption(this.extraOptions[0]);
    }
  }

  private clearOptions() {
    if (this.extraOptions.find(x => x.isSelected)) {
      this.extraOptions.forEach(element => {
        element.isSelected = false;
      });
    }
    this.selectedExtraOptionId = null;
    this.selectedExtraOptionIdChange.emit(this.selectedExtraOptionId);
  }
}
