import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { AmcsDropdownIconEnum } from '../amcs-dropdown/amcs-dropdown-icon-enum.model';
import { BrowserGridEditorActionButtonEnum } from '../layouts/amcs-browser-grid-editor-layout/browser-grid-editor-action-button.enum';
import { MenuItemClickedEvent } from '../layouts/amcs-browser-grid-layout/menu-item-clicked-event';
import { OpenActionMenuEvent } from '../layouts/amcs-browser-grid-layout/open-action-menu-event';
import { ActionColumnButton } from './amcs-grid-action-button';
import { IConfigurableActionRow } from './configurable-action-row';

@Component({
  selector: 'app-amcs-grid-action-column',
  templateUrl: './amcs-grid-action-column.component.html',
  styleUrls: ['./amcs-grid-action-column.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AmcsGridActionColumnComponent extends AutomationLocatorDirective implements OnInit {
  @Input() isDisabled: boolean;
  @Input() row: IConfigurableActionRow;
  @Input() displayInline = false;
  @Input() loading = false;
  @Input() dropup = false;
  @Input() items: ActionColumnButton[];
  @Input() dropdownIconEnum = AmcsDropdownIconEnum.EllipsisHorizontal;
  @Input() index: number;
  @Output() onMenuItemClicked = new EventEmitter<MenuItemClickedEvent>();
  @Output() onOpenActionMenu = new EventEmitter<OpenActionMenuEvent>();
  AmcsDropdownIconEnum = AmcsDropdownIconEnum;
  filteredItems: ActionColumnButton[];
  BrowserGridEditorActionButtonEnum = BrowserGridEditorActionButtonEnum;

  ngOnInit(): void {
    if (this.row.actionButtonsConfiguration) {
      this.items = [...this.row.actionButtonsConfiguration];
    }
    if (!this.items) {
      return;
    }

    if (this.row.rowTextHighlighted) {
      this.filteredItems = this.items.filter((x) => x.id === BrowserGridEditorActionButtonEnum.Undelete);
    } else {
      this.filteredItems = [...this.items];
      const index = this.items.findIndex((x) => x.id === BrowserGridEditorActionButtonEnum.Undelete);
      if (index > -1) {
        this.filteredItems.splice(index, 1);
      }
    }
  }

  emitClicked(button: ActionColumnButton, row: any) {
    if (!button.isDisabled) {
      this.onMenuItemClicked.emit(Object.assign(new MenuItemClickedEvent(), { action: button, row }));
    }
  }
}
