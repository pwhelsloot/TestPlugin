import { TemplateRef } from '@angular/core';
import { ComponentFilterProperty } from '@shared-module/components/amcs-component-filter/component-filter-property.model';
import { AmcsDropdownIconEnum } from '@shared-module/components/amcs-dropdown/amcs-dropdown-icon-enum.model';
import { GridActionColumnHeaderOptions } from '@shared-module/components/amcs-grid-action-column/amcs-grid-action-column-header/amcs-grid-action-column-header-options';
import { GridTotalsHeaderConfig } from '@shared-module/components/amcs-grid/grid-totals-header-config.model';
import { ActionColumnButton } from '../../amcs-grid-action-column/amcs-grid-action-button';
import { GridColumnConfig } from '../../amcs-grid/grid-column-config';
import { GridTotalsFooterConfig } from '../../amcs-grid/grid-totals-footer-config.model';

export class GridOptions {
  data: any[];
  columns: GridColumnConfig[];
  showFilter = true;
  showDeleted = false;
  expandOnSelection = true; // ? remove, check if expand template exists instead
  filterPlaceholder: string;
  rightAlignFilter = false;
  disableSelection = false;
  allowMultiSelect = false;
  allowHorizontalScroll = false;
  showCustomiser = false;
  showCustomFilter = false;
  includePaging = false;
  totalItemCount = 0;
  pageSize = 50;
  pageIndex = 0;
  useDefaultSort = true; // let the grid handle sorting. the alternative is to listen for and handle the sortChanged event.
  useDefaultFilter = true;
  removeCellBorders = false;
  alignHeaderWithCell = false;
  dropup = false;
  dropdownIconEnum = AmcsDropdownIconEnum.EllipsisHorizontal;
  compName: string;
  accordianTitle: string;
  filterProperties: ComponentFilterProperty[];
  allowVerticalScroll = false;
  gridHeight: string | number = 'auto';
  gridViewHeight: string | number = 'auto';
  gridMaxHeight: string | number = undefined;
  disableMinusToParenthesis = false;
  filterCustomClass: string;

  // let the grid handle filtering via the filter function on each table item.
  totalsHeaderConfig: GridTotalsHeaderConfig;
  headerSummaryEntity: any;
  totalsFooterConfig: GridTotalsFooterConfig;
  desktopRowHeight: string;
  mobileRowHeight: string;
  scrollToSelectedRow = false;
  allowColumnRebind = false;
  actionColumnButtons: ActionColumnButton[];
  displayActionColumnsButtonsInline = false;
  rowTemplate: TemplateRef<any>; // not implemented yet, doesn't appear to be used anywhere..
  rowDetailTemplate: TemplateRef<any>;
  mobileRowDetailTemplate: TemplateRef<any>;
  headerTemplate: TemplateRef<any>;
  detailTemplate: TemplateRef<any>;

  // action column configuration
  actionColumnTemplate: TemplateRef<any>;
  actionColumnHeaderOptions: GridActionColumnHeaderOptions;
  actionColumnWidth = 48;

  additionalOptionsTemplate: TemplateRef<any>;
  popoverTemplate: TemplateRef<any>;
  noDataTemplate: TemplateRef<any>;
  rowHeaderTemplate: TemplateRef<any>;
}
