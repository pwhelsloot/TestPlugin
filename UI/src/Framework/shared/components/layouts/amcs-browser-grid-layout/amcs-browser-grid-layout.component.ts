import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, TemplateRef } from '@angular/core';
import { ISelectorItem } from '../../../models/iselector-item.model';
import { BrowserGridOptions } from './browser-grid-options-model';
import { ColumnPositionChangedEvent } from './column-position-changed.event';
import { MenuItemClickedEvent } from './menu-item-clicked-event';
import { OpenActionMenuEvent } from './open-action-menu-event';
import { SortChangedEvent } from './sort-changed.event';
import { ValueChangedEvent } from './value-changed.event';

@Component({
  selector: 'app-amcs-browser-grid-layout',
  templateUrl: './amcs-browser-grid-layout.component.html',
  styleUrls: ['./amcs-browser-grid-layout.component.scss'],
})
export class AmcsBrowserGridLayoutComponent implements OnChanges {
  /**
   * Options that will be used for the Browser and Grid
   *
   * @type {BrowserGridOptions}
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Input() options: BrowserGridOptions;

  /**
   * Used by Browser to indicate in loading state or not
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Input() loading = false;

    /**
   * Used by Grid to indicate in loading state or not
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
     @Input() gridLoading = false;

  /**
   * Used by Grid to indicate if row details are loading
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Input() detailsLoading = false;

  /**
   * Template used by the Grid that will define what the details of each row will look like on Desktopn
   *
   * @type {TemplateRef<any>}
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Input() rowDetailTemplate: TemplateRef<any>; // or change template inputs to @ViewChild() as pass into options.. but more work for dev..

  /**
   * Template used by the Grid that will define what the details of each row will look like on Mobile devices
   *
   * @type {TemplateRef<any>}
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Input() mobileRowDetailTemplate: TemplateRef<any>;

  /**
   * Template that defines what a popover looks like
   *
   * @type {TemplateRef<any>}
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Input() popoverTemplate: TemplateRef<any>;

  /**
   * Template that indicates what the menu in the browser tile header will look like
   *
   * @type {TemplateRef<any>}
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Input() menuTemplate: TemplateRef<any>;

  /**
   * Template that indicates what the action in the browser tile header will look like
   *
   * @type {TemplateRef<any>}
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Input() headerActionTemplate: TemplateRef<any>;

  // outputs from browser
  /**
   * Emitted when the Add button is clicked in the browser tile
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Output() onAdd = new EventEmitter();

  /**
   * Emitted when the Edit button is clicked in the browser tile
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Output() onEdit = new EventEmitter();

  /**
   * Emitted when the Delete button is clicked in the browser tile
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Output() onDelete = new EventEmitter();

  /**
   * Emitted when the DeExpand button is clicked in the browser tile
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Output() onDeExpand = new EventEmitter();

  /**
   * Emitted when the Close button is clicked in the browser tile
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Output() onClose = new EventEmitter();

  // outputs from grid

  /**
   * Emitted when a row is selected
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Output() onRowsSelected = new EventEmitter<number[]>();

  /**
   * Emitted when a row is expanded
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Output() onRowExpanded = new EventEmitter<number>();

  /**
   * Emitted when a link on a row has been clicked
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Output() onLinkClicked = new EventEmitter<number>();

  /**
   * Emitted when the value of a amcs-select in a column was changed
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Output() onValueChanged = new EventEmitter<ValueChangedEvent>();

  /**
   * Emitted when the sorting direction of a column was changed
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Output() onSortChanged = new EventEmitter<SortChangedEvent>();

  /**
   * Emitted when the filter was changed
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Output() onFilterChanged = new EventEmitter<string>();

  /**
   * Emitted when a column has been re-ordered
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Output() onColumnPositionsChanged = new EventEmitter<ColumnPositionChangedEvent[]>();

  /**
   * Emitted when the page was changed
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Output() onSelectedPageChanged = new EventEmitter<number>();

  /**
   * Emitted when the action column menu is opened
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Output() onOpenActions = new EventEmitter<OpenActionMenuEvent>();

  /**
   * Emitted when a popover is clicked
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Output() onPopoverClicked = new EventEmitter<ISelectorItem>();

  /**
   * Emitted when a menu item in a row is clicked
   *
   * @memberof AmcsBrowserGridLayoutComponent
   */
  @Output() onMenuItemClicked = new EventEmitter<MenuItemClickedEvent>();

  @Output() showDeletedRecords = new EventEmitter<boolean>();

  @Output() onSave = new EventEmitter();

  @Output() onCancel = new EventEmitter();

  ngOnChanges(changes: SimpleChanges) {
    if (changes['options']) {
      this.options.gridOptions.rowDetailTemplate = this.rowDetailTemplate; // yes this looks weird but makes for easier syntax in layout
      this.options.gridOptions.mobileRowDetailTemplate = this.mobileRowDetailTemplate;
      this.options.gridOptions.popoverTemplate = this.popoverTemplate;
      this.options.browserOptions.menuTemplate = this.menuTemplate;
      this.options.browserOptions.headerActionTemplate = this.headerActionTemplate;
    }
  }
}
