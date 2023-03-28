import { CellPresentation } from '@coremodels/grid/cell-presentation.enum';
import { CellType } from '@coremodels/grid/cell-type.enum';

export class DataTableCell {
  headerName: string;
  cellProperty: string;
  valueClass: string;
  cellClass: string;
  isSelectable = false;
  width = 150;
  cellType: CellType;
  currencyCode: string;

  /*
    dateFormat should be one of the predefined formats for the built-in angular date pipe - 'short', medium' etc.
    the pipe should render the date appropriately using these formats depending on the locale.
    custom format strings such as 'M/d/yy' will also work but the date will appear in the same format
    regardless of the current locale.
  */
  dateFormat: string;

  mobileLandscape = false;
  mobilePortrait = false;
  desktop = false;
  details: DataTableCell[];
  sortable = true;

  constructor(headerName: string, cellProperty: string, cellType: CellType, dateFormat?: string, currencyCode?: string) {
    this.headerName = headerName;
    this.cellProperty = cellProperty;
    this.cellType = cellType;
    this.dateFormat = dateFormat || null;
    this.currencyCode = currencyCode || null;
  }

  addValueClass(value: string) {
    this.valueClass = value;
    return this;
  }

  addCellClass(value: string) {
    this.cellClass = value;
    return this;
  }

  addIsSelectable(value: boolean) {
    this.isSelectable = value;
    return this;
  }

  addWidth(value: number) {
    this.width = value;
    return this;
  }

  addViewPresentation(value: CellPresentation[]) {
    for (const item of value) {
      switch (item) {
        case CellPresentation.All:
          this.desktop = true;
          this.mobileLandscape = true;
          this.mobilePortrait = true;
          break;
        case CellPresentation.Desktop:
          this.desktop = true;
          break;
        case CellPresentation.MobileLandscape:
          this.mobileLandscape = true;
          break;
        case CellPresentation.MobilePortrait:
          this.mobilePortrait = true;
          break;
      }
    }
    return this;
  }

  addSortable(value: boolean) {
    this.sortable = value;
    return this;
  }
}
