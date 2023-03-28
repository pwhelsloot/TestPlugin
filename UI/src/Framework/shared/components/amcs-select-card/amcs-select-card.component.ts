import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { ISelectCardItem } from '@core-module/models/select-card-item.interface';
import { AmcsSelectHelper } from '../helpers/amcs-select-helper';

/**
 * @deprecated Move to PlatformUI
 */
@Component({
  selector: 'app-amcs-select-card',
  templateUrl: './amcs-select-card.component.html',
  styleUrls: ['./amcs-select-card.component.scss']
})
export class AmcsSelectCardComponent implements OnInit, OnChanges {

  @Input() items: ISelectCardItem[] = [];
  @Input() outerDivClass = 'col-xs-6';
  @Input() cardHeight = '60px';
  @Input() keepOriginalOrder = false;
  @Input() isMultiSelect = false;
  @Input() highlightIconClass: string;
  @Output() itemSelected = new EventEmitter<ISelectCardItem>();

  ngOnInit() {
    if (!this.isMultiSelect && this.items.filter(item => item.isSelected).length > 1) {
      this.items
        .forEach(item => item.isSelected = false);
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['items'] && this.items && !this.keepOriginalOrder) {
      AmcsSelectHelper.doSort(this.items);
    }
  }

  onItemClicked(selectedItem: ISelectCardItem) {
    selectedItem.isSelected = !selectedItem.isSelected;
    if (!this.isMultiSelect) {
      this.items
        .filter(item => item.id !== selectedItem.id)
        .forEach(item => item.isSelected = false);
    }
    this.itemSelected.emit(selectedItem);
  }
}
