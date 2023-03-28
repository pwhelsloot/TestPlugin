import { Component, forwardRef, Input, Output } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { ISelectableItem } from '@core-module/models/iselectable-item.model';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { Subject } from 'rxjs';

/**
 * @deprecated Move to PlatformUI
 */
@Component({
  selector: 'app-amcs-list-select-with-search',
  templateUrl: './amcs-list-select-with-search.component.html',
  styleUrls: ['./amcs-list-select-with-search.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmcsListSelectWithSearchComponent),
      multi: true
    }
  ]
})
export class AmcsListSelectWithSearchComponent extends AutomationLocatorDirective implements ControlValueAccessor {

  value: any;
  edited: boolean;
  FormControlDisplay = FormControlDisplay;
  searchString: string;
  selectAll: boolean;

  @Input('items') items: ISelectableItem[];
  @Input('isDisabled') isDisabled = false;
  @Input('hideSelectAll') hideSelectAll = false;
  @Input('hasError') hasError = false;
  @Input() loading = false;
  @Input() hideSearch = false;
  @Input() disableFormSelection = false;
  @Input() tooltipMessage: string;
  @Input() customSearchClass: string;
  @Input() autoFocus: boolean;
  @Output('rowsSelected') rowsSelected = new Subject<ISelectableItem[]>();

  onTouchedCallback: () => void = () => { };
  onChangeCallback: (_: any) => void = (_: any) => { };

  /** ControlValueAccessor interface */
  writeValue(value: any): void {
    this.value = value;
    if (this.value === null) {
      this.writeNullValue();
    } else {
      this.edited = true;
      this.onChangeCallback(this.value);
    }

    if (!this.disableFormSelection) {
      const valueAsArray: number[] = value as number[];
      this.items.forEach(i => i.isSelected = valueAsArray.includes(i.id));
      this.selectAll = !this.items.some(i => !i.isSelected);
      this.rowsSelected.next(this.items);
    }
  }

  registerOnChange(fn: any): void {
    this.onChangeCallback = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouchedCallback = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    this.isDisabled = isDisabled;
  }

  /** End ControlValueAccessor interface */

  getFilteredItems(): ISelectableItem[] {
    if (isTruthy(this.searchString) && this.searchString.length > 0) {
      const regex = new RegExp(this.searchString, 'i');
      return this.items.filter(x => x.description.match(regex));
    } else {
      return this.items;
    }
  }

  updateSelectAll(selectAll: boolean) {
    this.items.forEach(i => i.isSelected = selectAll);
    this.updateValue();
  }

  updateValue() {
    this.writeValue(this.items.filter(i => i.isSelected).map(i => i.id));
    if (this.disableFormSelection) {
      this.rowsSelected.next(this.items);
    }
  }

  private writeNullValue() {
    this.edited = false;
    this.onChangeCallback(null);
  }
}
