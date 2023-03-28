import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AmcsMobileSummaryItemEnum } from './amcs-mobile-summary-item-type.enum';
import { AmcsMobileSummaryItem } from './amcs-mobile-summary-item.model';

@Component({
  selector: 'app-amcs-mobile-summary',
  templateUrl: './amcs-mobile-summary.component.html',
  styleUrls: ['./amcs-mobile-summary.component.scss']
})
export class AmcsMobileSummaryComponent {

  @Input() items: AmcsMobileSummaryItem[] = [];
  @Output() onItemClick = new EventEmitter<number>();
  @Input('customClass') customClass: string;

  AmcsMobileSummaryItemEnum = AmcsMobileSummaryItemEnum;

  itemClicked(item: AmcsMobileSummaryItem) {
    this.onItemClick.emit(item.id);
  }
}
