import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { AmcsMenuItem } from '@shared-module/models/amcs-menu-item.model';

/**
 * @deprecated Marked for removal, use app-amcs-dropdown instead
 */
@Component({
  selector: 'app-amcs-dropdown-menu',
  templateUrl: './amcs-dropdown-menu.component.html',
  styleUrls: ['./amcs-dropdown-menu.component.scss']
})
export class AmcsDropdownMenuComponent extends AutomationLocatorDirective {

  @Input() backdropClass: string;
  @Input() buttonClass: string;
  @Input() items: AmcsMenuItem[] = [];
  @Input() xPosition = 'before';
  @Input() yPosition = 'below';
  @Output() itemClicked = new EventEmitter<AmcsMenuItem>();

  onItemClick(sender: AmcsMenuItem) {
    this.itemClicked.emit(sender);
  }
}
