import { animate, style, transition, trigger } from '@angular/animations';
import { CurrencyPipe, DecimalPipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  QueryList,
  Renderer2,
  SimpleChanges,
  TemplateRef,
  ViewChildren
} from '@angular/core';
import { MediaChange, MediaObserver } from '@angular/flex-layout';
import { isNullOrUndefined, isTruthy } from '@core-module/helpers/is-truthy.function';
import { ResizeableColumnHelper } from '@core-module/helpers/resizeable-column.helper';
import { GridColumnAdvancedConfig } from '@core-module/models/column-customiser/grid-column-advanced-config.model';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { InnerTileServiceUI } from '@core-module/services/ui/inner-tile.service.ui';
import { compareDates } from '@coremodels/date/amcs-date.model';
import { GridActionColumnHeaderOptions } from '@shared-module/components/amcs-grid-action-column/amcs-grid-action-column-header/amcs-grid-action-column-header-options';
import { IColumnLinkClickedEvent } from '@shared-module/components/amcs-grid-wrapper/column-link-clicked-event.interface';
import { GridSortDirection } from '@shared-module/components/amcs-grid/grid-sort-direction.enum';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { GridCellColorEnum } from '@shared-module/models/grid-cell-color-enum';
import { GridRowColorEnum } from '@shared-module/models/grid-row-color-enum';
import { ISelectorItem } from '@shared-module/models/iselector-item.model';
import { ITableItem } from '@shared-module/models/itable-item.model';
import { MinusToParenthesis } from '@shared-module/pipes/minus-to-parenthesis.pipe';
import { LocalizedDatePipe } from '@translate/localized-datePipe';
import { Subject, Subscription } from 'rxjs';
import { GridColumnAlignment } from './grid-column-alignment.enum';
import { GridColumnConfig } from './grid-column-config';
import { GridColumnType } from './grid-column-type.enum';
import { GridSortDefinition } from './grid-sort-definition';
import { GridSuperscriptColour } from './grid-superscript-colour.enum';
import { GridTotalsFooterConfig } from './grid-totals-footer-config.model';
import { GridTotalsHeaderConfig } from './grid-totals-header-config.model';
import { GridViewport } from './grid-viewport.enum';

const getStringValue = (value: any): string => (isNullOrUndefined(value) ? '' : value.toString());

@Component({
  selector: 'app-amcs-grid',
  templateUrl: './amcs-grid.component.html',
  styleUrls: ['./amcs-grid.component.scss'],
  providers: [DecimalPipe],
  animations: [
    trigger('expandAndCollapse', [
      transition(':enter', [style({ height: 0, overflow: 'hidden' }), animate('200ms', style({ height: '*' }))]),
      transition(':leave', [style({ height: '*', overflow: 'hidden' }), animate('200ms', style({ height: 0 }))])
    ])
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AmcsGridComponent extends AutomationLocatorDirective implements OnInit, OnChanges, OnDestroy {
  @Input('data') data: ITableItem[];
  @Input('columns') columns: GridColumnConfig[];
  @Input('rowTemplate') rowTemplate: TemplateRef<any>;
  @Input('rowHeaderTemplate') rowHeaderTemplate: TemplateRef<any>;
  @Input('rowDetailTemplate') rowDetailTemplate: TemplateRef<any>;
  @Input('mobileRowDetailTemplate') mobileRowDetailTemplate: TemplateRef<any>;
  @Input('showFilter') showFilter = false;
  @Input('showDeleted') showDeleted = false;
  @Input() filterPlaceholder: string;
  @Input() rightAlignFilter = false;
  @Input('detailsLoading') detailsLoading = false;
  @Input('loading') loading = false;
  @Input() disableSelection = false;
  @Input('allowMultiSelect') allowMultiSelect = false;
  @Input('expandOnSelection') expandOnSelection = false; // this flag allows a click anywhere on the row to toggle expansion, which makes sense where selection is not being used.
  @Input('includePaging') includePaging = false;
  @Input() allowHorizontalScroll = false;
  @Input() allowVerticalScroll = false;
  @Input('totalItemCount') totalItemCount: number = null;
  @Input('pageSize') pageSize = 50;
  @Input('pageIndex') pageIndex = 0;
  @Input('useDefaultSort') useDefaultSort = true; // let the grid handle sorting. the alternative is to listen for and handle the sortChanged event.
  @Input('useDefaultFilter') useDefaultFilter = true;
  @Input() removeCellBorders = false;
  @Input('alignHeaderWithCell') alignHeaderWithCell = false;
  @Input('customFooterClass') customFooterClass = '';
  @Input('applyStickyTooltipClass') applyStickyTooltipClass = false;
  @Input('truncateText') truncateText = true;
  @Input() gridHeight = 'auto';
  @Input() gridViewHeight: 'auto';
  @Input() gridMaxHeight: any = undefined;
  @Input('centerRowVertically') centerRowVertically = false;
  @Input('reducedMarginTop') reducedMarginTop = false;
  @Input() isSelectAllDisabled = false;
  @Input() hasSelectionError = false;

  // let the grid handle filtering via the filter function on each table item.
  @Input() totalsFooterConfig: GridTotalsFooterConfig;
  @Input() totalsHeaderConfig: GridTotalsHeaderConfig;
  @Input() headerSummaryEntity: any;
  @Input() footerSummaryEntity: any;

  // header and detail template overrides not implemented in view yet
  @Input('headerTemplate') headerTemplate: TemplateRef<any>;
  @Input('detailTemplate') detailTemplate: TemplateRef<any>;

  @Input('additionalOptionsTemplate') additionalOptionsTemplate: TemplateRef<any>;

  // action column configuration
  @Input('actionColumnTemplate') actionColumnTemplate: TemplateRef<any>;
  @Input() actionColumnHeaderOptions: GridActionColumnHeaderOptions;
  @Input() actionColumnWidth = 48;

  @Input('popoverTemplate') popoverTemplate: TemplateRef<any>;

  @Input('noDataTemplate') noDataTemplate: TemplateRef<any>;
  @Input() desktopRowHeight: string;
  @Input() mobileRowHeight: string;
  @Input() scrollToSelectedRow = false;
  @Input('allowColumnRebind') allowColumnRebind = false;
  @Input('hideHeaders') hideHeaders = false;

  @Input() selectedCheckboxColumnWidth = 40;
  @Input() showSelectedCheckbox = false;
  @Input() disableMinusToParenthesis = false;
  @Input('filterCustomClass') filterCustomClass = '';

  @Output('rowsSelected') rowsSelected = new Subject<number[]>();
  @Output('rowExpanded') rowExpanded = new Subject<any>();
  @Output('linkClicked') linkClicked = new Subject<number>();
  @Output('linkClickedExtended') linkClickedExtended = new Subject<IColumnLinkClickedEvent>();
  @Output('valueChanged') valueChanged = new Subject<{ rowId: number; key: string }>();
  @Output('sortChanged') sortChanged = new Subject<{ key: string; direction: GridSortDirection }>();
  @Output('filterChanged') filterChanged = new Subject<string>();
  @Output('columnPositionsChanged') columnPositionsChanged = new Subject<{ key: string; position: number }[]>();
  @Output('selectedPageChanged') selectedPageChanged = new Subject<number>();
  @Output('popoverClicked') popoverClicked = new Subject<ISelectorItem>();
  @Output('selectedRowData') selectedRowData = new Subject<any>();
  @Output('showDeletedRecords') showDeletedRecords = new Subject<boolean>();

  @ViewChildren('rowDiv') rowDivs: QueryList<ElementRef>;

  // enums
  Viewport = GridViewport;
  ColumnType = GridColumnType;
  displayMode = FormControlDisplay.Grid;
  GridSuperscriptColour = GridSuperscriptColour;
  GridColumnAlignment = GridColumnAlignment;
  GridCellColorEnum = GridCellColorEnum;
  GridRowColorEnum = GridRowColorEnum;

  currentViewport: GridViewport = null;
  visibleColumns: GridColumnConfig[];
  selectedRowIds: number[] = [];
  expandedRowId: number;
  filterText: string;
  filteredData: ITableItem[];
  draggedColumn: GridColumnConfig;
  columnTotals: { key: string; total: number }[] = [];
  topColumnTotals: { key: string; total: number }[] = [];
  resizeHelper: ResizeableColumnHelper = null;
  isAllSelected = false;
  radioButtonOverride = false;

  constructor(
    protected elRef: ElementRef,
    protected renderer: Renderer2,
    private datePipe: LocalizedDatePipe,
    private currencyPipe: CurrencyPipe,
    private minusToParenthesisPipe: MinusToParenthesis,
    private media: MediaObserver,
    public tileUiService: InnerTileServiceUI,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    super(elRef, renderer);
    this.isEdge = navigator.userAgent.indexOf('Edge') > -1;
  }

  private mediaChangeSubscription: Subscription;
  private columnPositionsInitialised: boolean;
  private currentSort: GridSortDefinition = null;
  private actionClicked = false;
  private isEdge = false;

  ngOnInit() {
    if (this.media.isActive('xs') || this.media.isActive('sm')) {
      this.currentViewport = GridViewport.mobile;
    } else {
      this.currentViewport = GridViewport.desktop;
    }

    this.initiateSelectAll();
    this.changeViewport();

    this.mediaChangeSubscription = this.media.media$.subscribe((change: MediaChange) => {
      // for now, the grid supports only 2 different viewports - desktop and mobile.
      // here we map the flex-layout viewports to these two.
      let viewport: GridViewport = null;

      switch (change.mqAlias) {
        case 'xs':
          viewport = GridViewport.mobile;
          break;
        case 'sm':
          viewport = GridViewport.mobile;
          break;
        case 'md':
          viewport = GridViewport.desktop;
          break;
        case 'lg':
          viewport = GridViewport.desktop;
          break;
        case 'xl':
          viewport = GridViewport.desktop;
          break;
      }

      if (viewport !== this.currentViewport) {
        this.currentViewport = viewport;
        this.changeViewport();
      }
    });

    this.calculateTotals();
    this.radioButtonOverride = !this.disableSelection && this.showSelectedCheckbox && !this.allowMultiSelect;
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['columns'] && this.columns && (!this.columnPositionsInitialised || this.allowColumnRebind)) {
      this.initialiseColumnPositions();
      this.columnPositionsInitialised = true;

      const sortedColumn = this.columns.find((x) => x.sortedBy || x.sortedByDesc);
      if (sortedColumn) {
        const direction = sortedColumn.sortedBy ? GridSortDirection.ascending : GridSortDirection.descending;
        this.currentSort = new GridSortDefinition(
          sortedColumn.sortedByAnotherPropertyKey != null ? sortedColumn.sortedByAnotherPropertyKey : sortedColumn.key,
          direction
        );
      }

      this.calculateTotals();
      this.changeViewport();
    }

    if (changes['data'] && this.data) {
      this.expandedRowId = null; // Reset the expanded row so that this.expand always works
      this.selectedRowIds = this.data.filter((x) => x.isSelected).map((x) => x.id);
      this.isAllSelected = false;
      this.filteredData = this.data.slice();
      if (this.expandOnSelection && this.selectedRowIds[0] != null) {
        this.expand(this.selectedRowIds[0]);
      }

      if (this.scrollToSelectedRow) {
        this.scrollToFirstSelectedRow();
      }

      this.calculateTotals();
      this.formatTruncatableDates();

      if (this.currentSort) {
        this.applyDefaultSort(this.currentSort);
        this.sortChanged.next(this.currentSort);
      }

      // This is to solve performance issue on truncated column
      setTimeout(() => {
        if (!this.changeDetectorRef['destroyed']) {
          this.changeDetectorRef.detectChanges();
        }
      }, 200);
    }

    this.initiateSelectAll();
  }

  ngOnDestroy() {
    if (this.mediaChangeSubscription) {
      this.mediaChangeSubscription.unsubscribe();
    }
    if (isTruthy(this.resizeHelper)) {
      this.resizeHelper.destroy();
    }
  }

  onFilterTextChanged(filterText: string) {
    this.filterText = filterText;
    let searchTerm: string = this.filterText;
    if (isTruthy(searchTerm)) {
      searchTerm = searchTerm.toLowerCase();
    } else {
      searchTerm = '';
    }
    if (this.useDefaultFilter) {
      this.applyDefaultFilter(searchTerm);
    }
    this.filterChanged.next(searchTerm);
  }

  showDeletedData(e: any) {
    this.showDeletedRecords.next(e.target.checked);
  }

  onRowClicked(row: ITableItem) {
    if (this.disableSelection || (row.isSelectDisabled ?? false)) {
      return;
    }

    if (this.actionClicked) {
      this.actionClicked = false;
    } else {
      row.isSelected = !row.isSelected;
      this.onIsSelectedChanged(row);
    }
  }

  onIsSelectedChanged(row: ITableItem) {
    if (this.disableSelection) {
      return;
    }

    if (this.actionClicked) {
      this.actionClicked = false;
    } else {
      // If selected and single-select, deselect all other rows
      if (row.isSelected && !this.allowMultiSelect) {
        this.data.filter(x => x.isSelected && x.id !== row.id).forEach(x => x.isSelected = false);
      }

      this.selectedRowIds = this.data.filter(x => x.isSelected).map(x => x.id);
      this.initiateSelectAll();
      this.rowsSelected.next(this.selectedRowIds);

      if (row.isSelected && this.expandOnSelection) {
        this.expand(row.id);
      }
    }
  }

  onPopoverClicked(row: any) {
    this.actionClicked = true;
    this.popoverClicked.next(row);
  }

  onActionClicked() {
    this.actionClicked = true;
  }

  onExpandRowClicked(row: ITableItem) {
    if (this.disableSelection || (row.isSelectDisabled ?? false)) {
      return;
    }

    this.expand(row.id);
    event.stopPropagation();
  }

  onLinkClicked(key: string, item: ITableItem) {
    this.linkClicked.next(item.id);
    this.linkClickedExtended.next({ key, item });
  }

  onValueChanged(rowId: number, key: string) {
    this.valueChanged.next({ rowId, key });
  }

  onPageChanged(page: number) {
    this.clearSort();
    if (this.pageIndex !== page) {
      this.pageIndex = page;
    }
    this.selectedPageChanged.next(page);
    this.scrollToTop(this.elRef);
  }

  onSortClicked(column: GridColumnConfig) {
    const direction = column.sortedBy ? GridSortDirection.descending : GridSortDirection.ascending;
    const sortDefinition = new GridSortDefinition(
      column.sortedByAnotherPropertyKey !== null ? column.sortedByAnotherPropertyKey : column.key,
      direction
    );

    if (this.useDefaultSort) {
      this.applyDefaultSort(sortDefinition);
    }

    this.sortChanged.next(sortDefinition);
  }

  drag(column: GridColumnConfig) {
    this.draggedColumn = column;
  }

  dragOver(event) {
    // this seems to be required in order for the drop event to fire
    event.preventDefault();
  }

  drop(column: GridColumnConfig) {
    if (this.draggedColumn) {
      const sourceColumnPosition = this.draggedColumn.position;
      this.draggedColumn.position = column.position;
      column.position = sourceColumnPosition;
      this.draggedColumn = null;

      this.columns = this.columns.sort((a, b) => a.position - b.position);

      // this is necessary to reflect the new ordering in the visible column list
      this.setVisibleColumns();
      this.resizeHelper = new ResizeableColumnHelper(this.visibleColumns);

      this.columnPositionsChanged.next(this.columns.map((x) => ({ key: x.key, position: x.position })));
    }
  }

  toggleSelectAll() {
    this.isAllSelected = !this.isAllSelected;

    if (isTruthy(this.filteredData) && this.filteredData.length > 0) {
      this.filteredData.forEach(data => data.isSelected = this.isAllSelected && !data.isSelectDisabled);

      if (this.isAllSelected) {
        this.selectedRowIds = this.filteredData.filter(data => !data.isSelectDisabled).map(data => data.id);
      } else {
        this.selectedRowIds = [];
      }

      this.rowsSelected.next(this.selectedRowIds);
    }
  }

  formatCurrencyValue(value, currencyCode, noOfDecimalPlaces) {
    const formattedValue = this.currencyPipe.transform(value, currencyCode, 'symbol', noOfDecimalPlaces);
    return this.disableMinusToParenthesis ? formattedValue : this.minusToParenthesisPipe.transform(formattedValue);
  }

  private initialiseColumnPositions() {
    if (this.columns) {
      const withPosition = this.columns.filter((x) => x.position !== -1);
      const withoutPosition = this.columns.filter((x) => x.position === -1);
      withPosition.sort((a, b) => a.position - b.position);
      withPosition.push(...withoutPosition);
      let position = 1; // start at 1 because if there is a row expander column it will be in position 0.
      withPosition.forEach((x) => {
        x.position = position;
        position++;
      });

      this.columns = withPosition.sort((a, b) => a.position - b.position);
    }
  }

  private expand(rowId: number) {
    this.expandedRowId = this.expandedRowId === rowId ? null : rowId;
    this.rowExpanded.next(this.expandedRowId);
  }

  private scrollToTop(element: ElementRef) {
    setTimeout(() => {
      if (navigator.userAgent.indexOf('Edge') > -1) {
        element.nativeElement.scrollIntoView();
        window.scrollBy(0, -(element.nativeElement.offsetHeight - 10));
      } else {
        window.scrollTo({ top: element.nativeElement.offsetTop - 50, behavior: 'smooth' });
      }
    }, 300);
  }

  private applyDefaultFilter(searchTerm: string) {
    this.filteredData = this.data.filter((x) => x.filter(searchTerm));

    if (isTruthy(this.currentSort)) {
      this.sort(this.filteredData, this.currentSort);
    }
    this.calculateTotals();
  }

  private applyDefaultSort(sortDefinition: GridSortDefinition) {
    this.sort(this.filteredData, sortDefinition);
    this.currentSort = sortDefinition;

    this.columns.forEach((x) => {
      if (x.key === this.currentSort.key || x.sortedByAnotherPropertyKey === this.currentSort.key) {
        x.sortedBy = this.currentSort.direction === GridSortDirection.ascending ? true : false;
        x.sortedByDesc = this.currentSort.direction === GridSortDirection.descending ? true : false;
      } else {
        x.sortedBy = false;
        x.sortedByDesc = false;
      }
    });

    // this is necessary to reflect the new ordering in the visible column list
    this.setVisibleColumns();
    this.resizeHelper = new ResizeableColumnHelper(this.visibleColumns);
  }

  private setVisibleColumns() {
    this.visibleColumns = this.columns.filter((col) => col.viewports.find((x) => x.viewport === this.currentViewport && x.visible));
    this.visibleColumns.forEach((col) => {
      if (col.applyConditionalColor && col.cellValues.length > 0) {
        const cellValues = this.filteredData.map((row) => row[col.cellValueKey]);
        if (cellValues.length > 0) {
          col.assignCellColorsBasedOnValue(cellValues);
        }
      }
    });
  }

  private sort(data: ITableItem[], sort: GridSortDefinition) {
    if (this.isDateColumn(sort.key)) {
      this.sortByDate(data, sort);
    } else if (this.isNumberColumn(sort.key)) {
      this.sortByNumber(data, sort);
    } else {
      this.sortByStringifiedColumn(data, sort);
    }
  }

  private isDateColumn(key: string) {
    const column = this.columns.find((col) => col.key === key);
    return (
      isTruthy(column) &&
      (column.type === GridColumnType.shortDate ||
        column.type === GridColumnType.shortDateTime ||
        column.type === GridColumnType.truncatedDateTime)
    );
  }

  private isNumberColumn(key: string) {
    const column = this.columns.find((col) => col.key === key);
    return (
      isTruthy(column) &&
      (column.type === GridColumnType.currency ||
        column.type === GridColumnType.integerInput ||
        column.type === GridColumnType.number ||
        column.type === GridColumnType.numericInput||
        column.type === GridColumnType.weight)
    );
  }

  private sortByStringifiedColumn(data: ITableItem[], sort: GridSortDefinition) {
    data.sort((x, y) => {
      const xValue = getStringValue(x[sort.key]);
      const yValue = getStringValue(y[sort.key]);
      return xValue.localeCompare(yValue, undefined, { sensitivity: 'accent' }) * (sort.direction === GridSortDirection.ascending ? 1 : -1);
    });
  }

  private sortByDate(data: ITableItem[], sort: GridSortDefinition) {
    data.sort((x, y) => {
      const xValue = x[sort.key];
      const yValue = y[sort.key] as Date;
      return compareDates(xValue, yValue) * (sort.direction === GridSortDirection.ascending ? 1 : -1);
    });
  }

  private sortByNumber(data: ITableItem[], sort: GridSortDefinition) {
    data.sort((x, y) => {
      const xValue = x[sort.key];
      const yValue = y[sort.key];
      return this.compareNumbers(xValue, yValue) * (sort.direction === GridSortDirection.ascending ? 1 : -1);
    });
  }

  private compareNumbers(x: number, y: number) {
    const xValue = isNullOrUndefined(x) ? Number.MIN_VALUE : x;
    const yValue = isNullOrUndefined(y) ? Number.MIN_VALUE : y;
    if (xValue < yValue) {
      return -1;
    } else if (xValue > yValue) {
      return 1;
    } else {
      return 0;
    }
  }

  private clearSort() {
    this.currentSort = null;
    this.columns.forEach((x) => {
      x.sortedBy = false;
      x.sortedByDesc = false;
    });
  }

  private changeViewport() {
    if (this.columns) {
      this.setVisibleColumns();
      this.visibleColumns.forEach((col) => {
        if (!(col instanceof GridColumnAdvancedConfig)) {
          const columnViewport = col.viewports.find((x) => x.viewport === this.currentViewport);
          col.widthPercentage = columnViewport.widthPercentage;
        }
      });
      this.resizeHelper = new ResizeableColumnHelper(this.visibleColumns);
    }
  }

  private calculateTotals() {
    this.configureHeaderRow();
    this.configureFooterRow();
  }

  private configureHeaderRow() {
    if (isTruthy(this.columns) && isTruthy(this.totalsHeaderConfig) && isTruthy(this.filteredData) && this.filteredData.length > 0) {
      this.columns
        .filter((column) => column.includeInTopTotal)
        .forEach((column) => {
          const columnKey = isTruthy(column.topTotalColumnMappedToColumnKey) ? column.topTotalColumnMappedToColumnKey : column.key;

          if (isTruthy(this.headerSummaryEntity)) {
            this.topColumnTotals[column.key] = this.headerSummaryEntity[columnKey];
          } else if (this.filteredData.length > 0) {
            this.topColumnTotals[column.key] = this.filteredData.map((row) => row[column.key]).reduce((sum, current) => sum + current);
          }
        });
    }
  }
  private configureFooterRow() {
    if (isTruthy(this.columns) && isTruthy(this.totalsFooterConfig) && isTruthy(this.filteredData) && this.filteredData.length > 0) {
      this.columns
        .filter((column) => column.includeInTotal)
        .forEach((column) => {
          const columnKey = isTruthy(column.topTotalColumnMappedToColumnKey) ? column.topTotalColumnMappedToColumnKey : column.key;

          if (isTruthy(this.footerSummaryEntity)) {
            this.columnTotals[column.key] = this.footerSummaryEntity[columnKey];
          } else if (this.filteredData.length > 0) {
            this.columnTotals[column.key] = this.filteredData.map((row) => row[column.key]).reduce((sum, current) => sum + current);
          }
        });
    }
  }

  private formatTruncatableDates() {
    if (this.filteredData && this.filteredData.length > 0 && isTruthy(this.columns)) {
      this.columns
        .filter((x) => x.type === GridColumnType.truncatedDateTime)
        .forEach((column) => {
          this.filteredData.map((row) => {
            const date: Date = row[column.key];
            row[column.key] = this.datePipe.transform(date, 'shortDate') + ' ' + this.datePipe.transform(date, 'shortTime');
          });
        });
    }
  }

  private scrollToFirstSelectedRow() {
    setTimeout(() => {
      const record = this.data.find((x) => x.isSelected);

      if (record) {
        const element: ElementRef = this.rowDivs.find((x) => x.nativeElement.id === record.id.toString());

        if (element && !this.isElementInViewport(element)) {
          if (this.isEdge) {
            element.nativeElement.scrollIntoView();
            window.scrollBy(0, -(element.nativeElement.offsetHeight - 10));
          } else {
            window.scrollTo({ top: element.nativeElement.offsetTop, behavior: 'smooth' });
          }
        }
      }
    });
  }

  private isElementInViewport(element: ElementRef): boolean {
    const rect = element.nativeElement.getBoundingClientRect();

    return (
      rect.top >= 0 &&
      rect.left >= 0 &&
      rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
      rect.right <= (window.innerWidth || document.documentElement.clientWidth)
    );
  }

  private initiateSelectAll() {
    if (this.data && this.data.length === this.data.filter(x => x.isSelected).length && this.data.length > 0) {
      this.isAllSelected = true;
    } else {
      this.isAllSelected = false;
    }
  }
}
