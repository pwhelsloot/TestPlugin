import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, ViewEncapsulation } from '@angular/core';
import { IColumnLinkClickedEvent } from '@shared-module/components/amcs-grid-wrapper/column-link-clicked-event.interface';
import { ISelectorItem } from '@shared-module/models/iselector-item.model';
import { ColumnPositionChangedEvent } from '../layouts/amcs-browser-grid-layout/column-position-changed.event';
import { GridOptions } from '../layouts/amcs-browser-grid-layout/grid-options-model';
import { MenuItemClickedEvent } from '../layouts/amcs-browser-grid-layout/menu-item-clicked-event';
import { OpenActionMenuEvent } from '../layouts/amcs-browser-grid-layout/open-action-menu-event';
import { SortChangedEvent } from '../layouts/amcs-browser-grid-layout/sort-changed.event';
import { ValueChangedEvent } from '../layouts/amcs-browser-grid-layout/value-changed.event';

@Component({
  selector: 'app-amcs-grid-wrapper',
  templateUrl: './amcs-grid-wrapper.component.html',
  styleUrls: ['./amcs-grid-wrapper.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AmcsGridWrapperComponent {
  @Input() options: GridOptions;
  @Input() detailsLoading = false;
  @Input() loading = false;

  @Output() rowsSelected = new EventEmitter<number[]>();
  @Output() rowExpanded = new EventEmitter<number>();
  @Output() linkClicked = new EventEmitter<number>();
  @Output() linkClickedExtended = new EventEmitter<IColumnLinkClickedEvent>();
  @Output() valueChanged = new EventEmitter<ValueChangedEvent>();
  @Output() sortChanged = new EventEmitter<SortChangedEvent>();
  @Output() filterChanged = new EventEmitter<string>();
  @Output() columnPositionsChanged = new EventEmitter<ColumnPositionChangedEvent[]>();
  @Output() selectedPageChanged = new EventEmitter<number>();
  @Output() popoverClicked = new EventEmitter<ISelectorItem>();
  @Output() menuItemClicked = new EventEmitter<MenuItemClickedEvent>();
  @Output() openActions = new EventEmitter<OpenActionMenuEvent>();
  @Output() showDeletedRecords = new EventEmitter<boolean>();
}
