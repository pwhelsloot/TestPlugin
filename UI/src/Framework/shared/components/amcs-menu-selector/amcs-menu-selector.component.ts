import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { AmcsMenuSelectorItem } from './amcs-menu-selector-item.model';

/**
 * @deprecated Marked for removal, use app-amcs-dropdown instead
 */
@Component({
  selector: 'app-amcs-menu-selector',
  templateUrl: './amcs-menu-selector.component.html',
  styleUrls: ['./amcs-menu-selector.component.scss']
})
export class AmcsMenuSelectorComponent extends AutomationLocatorDirective {

  @Input() items: AmcsMenuSelectorItem[];
  @Input() headerText: string;

  @Output() onItemSelected = new EventEmitter<AmcsMenuSelectorItem>();

  showMenu = false;

  onSelected(itemSelected: AmcsMenuSelectorItem) {
    this.items.forEach(item => {
      item.isActive = false;
    });
    itemSelected.isActive = true;
    this.onItemSelected.emit(itemSelected);
    this.showMenu = false;
  }

}
