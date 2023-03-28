import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-amcs-mobile-header',
  templateUrl: './amcs-mobile-header.component.html',
  styleUrls: ['./amcs-mobile-header.component.scss']
})
export class AmcsMobileHeaderComponent {

  @Input() loading: boolean;
  @Input() title: string;
  @Input() optionText: string;
  @Input() optionIcon: string;
  @Input() optionIconSubText: string;
  @Input() optionUppercase = true;
  @Input() saving = false;
  @Output() onReturn = new EventEmitter();
  @Output() onOption = new EventEmitter();

  onOptionClicked() {
    if (!this.saving) {
      this.onOption.emit();
    }
  }

  onBackClicked() {
    this.onReturn.emit();
  }
}
