import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AmcsMobileSummaryItem } from '../amcs-mobile-summary/amcs-mobile-summary-item.model';

@Component({
  selector: 'app-amcs-mobile-layout',
  templateUrl: './amcs-mobile-layout.component.html',
  styleUrls: ['./amcs-mobile-layout.component.scss']
})
export class AmcsMobileLayoutComponent {

  @Input() loading: boolean;

  @Input() title: boolean;
  @Input() optionText: string;
  @Input() optionIcon: string;
  @Input() optionIconSubText: string;
  @Input() optionUppercase = true;
  @Output() onReturn = new EventEmitter();
  @Output() onOption = new EventEmitter();

  @Input() showSaveCancel = false;
  @Input() cancelText: string;
  @Input() cancelSecondaryColor = false;
  @Input() cancelPrimaryColor = false;
  @Input() hideCancelButton = false;
  @Input() saveText: string;
  @Input() enableSave = true;
  @Input() cancelEmitsReturnEvent = true;
  @Input() saving = false;
  @Output() onSave = new EventEmitter();
  @Output() onCancel = new EventEmitter();

  @Input() showSummary = false;
  @Input() summaryItems: AmcsMobileSummaryItem[] = [];
  @Input() summaryCustomClass: string;
  @Output() onItemClick = new EventEmitter<number>();

  return() {
    this.onReturn.emit();
  }

  option() {
    this.onOption.emit();
  }

  save() {
    this.onSave.emit();
  }

  cancel() {
    if (this.cancelEmitsReturnEvent) {
      this.return();
    } else {
      this.onCancel.emit();
    }
  }

  itemClicked(id: number) {
    this.onItemClick.emit(id);
  }
}
