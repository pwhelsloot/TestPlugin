import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  forwardRef,
  Injector,
  Input,
  OnDestroy,
  OnInit,
  Optional,
  Output,
  Renderer2,
  ViewChild,
} from '@angular/core';
import { ControlContainer, ControlValueAccessor, NgControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { ISelectableItem } from '@coremodels/iselectable-item.model';
import { AmcsRadioControlRegistryDirective } from '@shared-module/components/amcs-radio-input/amcs-radio-control-registry.directive';
import { AmcsFormControlBaseComponent } from '@shared-module/forms/amcs-form-control-base';
import { IAmcsRadioControl } from './amcs-radio-control.interface';

@Component({
  selector: 'app-amcs-radio-input',
  templateUrl: './amcs-radio-input.component.html',
  styleUrls: ['./amcs-radio-input.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsRadioInputComponent),
      multi: true,
    },
  ],
})
export class AmcsRadioInputComponent
  extends AmcsFormControlBaseComponent
  implements OnInit, OnDestroy, AfterViewInit, ControlValueAccessor, IAmcsRadioControl
{
  state: boolean;
  @Input() value: any;
  @Input() isSecondaryColor = false;
  @Input() isBlueRadio = false;
  @Input() extraOptions: ISelectableItem[];
  @Input('bigRadio') bigRadio = true;
  @Input('isDisabled') isDisabled: boolean;
  @Input() noMargin = false;
  @Input() customClass: string = null;
  @Output() selectedExtraOptionsChange: EventEmitter<ISelectableItem[]> = new EventEmitter<ISelectableItem[]>();
  @ViewChild('input') inputElement: ElementRef;

  constructor(
    protected elRef: ElementRef,
    protected renderer: Renderer2,
    private registry: AmcsRadioControlRegistryDirective,
    private injector: Injector,
    @Optional() readonly controlContainer: ControlContainer
  ) {
    super(controlContainer, elRef, renderer);
  }

  private control: NgControl;

  onChange = () => {};
  onTouched = () => {};

  ngOnInit() {
    this.control = this.injector.get(NgControl);
    this.registry.add(this.control, this);
  }

  ngAfterViewInit() {
    this.renderer.setProperty(this.inputElement.nativeElement, 'checked', this.state);
  }

  ngOnDestroy() {
    this.registry.remove(this);
  }

  writeValue(value: any): void {
    this.state = value === this.value;
    if (this.inputElement) {
      this.renderer.setProperty(this.inputElement.nativeElement, 'checked', this.state);
    }
    if (!this.state && this.extraOptions && this.extraOptions.find((x) => x.isSelected)) {
      this.extraOptions.forEach((element) => {
        element.isSelected = false;
      });
      this.selectedExtraOptionsChange.emit([]);
    }
  }

  registerOnChange(fn: (_: any) => {}): void {
    this.onChange = () => {
      fn(this.value);
      this.registry.select(this);
      this.onTouched();
    };
  }

  fireCheck(value: any): void {
    this.writeValue(value);
  }

  registerOnTouched(fn: () => {}): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.isDisabled = isDisabled;
  }

  afterSelection() {
    // RDM Not needed for basic radio control, used in radio-tile
  }

  selectedExtrasChange() {
    this.onChange();
    this.onTouched();
    this.selectedExtraOptionsChange.emit(this.extraOptions.filter((x) => x.isSelected));
  }
}
