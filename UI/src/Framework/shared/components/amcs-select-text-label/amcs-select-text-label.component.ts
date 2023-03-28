import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';

/**
 * @todo Marked to be a private component
 * Move to amcs-multi-select-with-search folder
 */
@Component({
  selector: 'app-amcs-select-text-label',
  templateUrl: './amcs-select-text-label.component.html',
  styleUrls: ['./amcs-select-text-label.component.scss']
})
export class AmcsSelectTextLabelComponent implements OnChanges {

  @Input() allItems = [];
  @Input() items = [];
  @Input() toolTipPlacement = 'auto';
  @Input() isGroupBy = false;

  text: string;

  ngOnChanges(changes: SimpleChanges) {

    const allItemsAreSelected = (this.allItems.length === this.items.length);
    const selectAllGroupIsSelected = (this.items.length === 1 && this.items[0].selectAll && this.items[0].id === undefined);

    if (!this.isGroupBy && (allItemsAreSelected || selectAllGroupIsSelected)) {
      this.text = this.items[0].selectAll;
    } else {
      this.text = this.items.map(x => x.description).join(', ');
    }
  }
}
