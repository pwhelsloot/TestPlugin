import { Component, ElementRef, EventEmitter, forwardRef, Input, OnInit, Output, Renderer2 } from '@angular/core';
import { ControlContainer, ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { ModalGridSelectorServiceAdapter } from '@core-module/services/forms/modal-grid-selector.adapter';
import { GridColumnConfig } from '@shared-module/components/amcs-grid/grid-column-config';
import { AmcsFormControlBaseComponent } from '@shared-module/forms/amcs-form-control-base';
import { filter, take } from 'rxjs/operators';
import { IFilter } from '../../../core/models/api/filters/iFilter';
import { AmcsModalService } from '../amcs-modal/amcs-modal.service';
import { ModalSelectorModeEnum } from './modal-selector-mode.enum';
import { ModalDetailsComponent } from './modal-selector/modal-details.component';

@Component({
  selector: 'app-amcs-modal-selector',
  templateUrl: './amcs-modal-selector.component.html',
  styleUrls: ['./amcs-modal-selector.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsModalSelectorComponent),
      multi: true
    }
  ]
})
export class AmcsModalSelectorComponent extends AmcsFormControlBaseComponent implements OnInit, ControlValueAccessor {
  FormControlDisplay = FormControlDisplay;

  @Output() itemsSelected = new EventEmitter<ILookupItem | ILookupItem[]>();

  @Input() customClass: string;
  @Input() customWrapperClass: string;
  @Input() isDisabled: boolean;
  @Input() placeholder: string;
  @Input() description = '';
  @Input() modalTitle: string;
  @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;
  @Input() columns: GridColumnConfig[];
  @Input() filters: IFilter[];
  @Input() modalSelectorMode: ModalSelectorModeEnum = ModalSelectorModeEnum.Single;
  @Input() autoClose = true;
  edited = false;
  value: any;
  constructor(
    readonly controlContainer: ControlContainer,
    protected elRef: ElementRef,
    protected renderer: Renderer2,
    private modalService: AmcsModalService,
    public adapter: ModalGridSelectorServiceAdapter
  ) {
    super(controlContainer, elRef, renderer);
  }
  ngOnInit(): void {
    if (this.autoClose && this.modalSelectorMode === ModalSelectorModeEnum.Multi) {
      this.autoClose = false;
   }
  }
  onTouchedCallback: () => void = () => {};
  onChangeCallback: (_: any) => void = (_: any) => {};

  onControlClick() {
    this.modalService
      .createModal({
        type: 'confirmation',
        title: this.modalTitle,
        template: ModalDetailsComponent,
        largeSize: true,
        hideButtons: true,
        extraData: [this.passValueAsArr(this.value), this.columns, this.adapter, this.filters, this.checkIfMulti(), this.autoClose]
      })
      .afterClosed()
      .pipe(
        take(1),
        filter((x) => !!x && x.length > 0)
      )
      .subscribe((selectedItems: ILookupItem[]) => {
        if (this.modalSelectorMode === ModalSelectorModeEnum.Single) {
          this.itemsSelected.emit(selectedItems[0]);
          this.writeValue(selectedItems[0].id);
        } else {
          this.itemsSelected.emit(selectedItems);
          this.writeValue(selectedItems.map((item) => item.id));
        }
      });
  }

  writeValue(value: any) {
    this.value = value;
    if (!isTruthy(this.value)) {
      this.writeNullValue();
    } else {
      this.edited = true;
      this.onChangeCallback(this.value);
    }
  }

  reset() {
    this.writeValue(null);
    this.itemsSelected.emit(null);
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

  private checkIfMulti(): boolean {
    return this.modalSelectorMode === ModalSelectorModeEnum.Multi;
  }

  private writeNullValue() {
    this.edited = false;
    this.value = null;
    this.description = '';
    this.onChangeCallback(null);
  }

  private passValueAsArr(value): any[] {
    if (Array.isArray(value)) {
      return value;
    }
    if (!!value) {
      return [value];
    }
    return [];
  }
}
