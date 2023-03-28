import { CellPresentation } from '@coremodels/grid/cell-presentation.enum';
import { GridAction } from '@coremodels/grid/grid-action.model';
import { IDataGridCtrl } from '@shared-module/models/amcs-data-grid/amcs-data-grid-ctrl.model';
import { DataTableCell } from '@shared-module/models/amcs-data-grid/amcs-data-table-cell.model';

export class DataGridMetaData {
    rows: IDataGridCtrl[];
    columns: DataTableCell[];
    detailRowPresentation: CellPresentation[];
    detailRowHeaderText: string;
    hasDropdown = false;
    rowActions: GridAction[] = [];

    static buildGrid<T>(rows: IDataGridCtrl[], translations: string[], type: new () => T): DataGridMetaData {
        const result = new DataGridMetaData();
        result.rows = rows;
        result.columns = (type as any).getGridColumns(translations);
        return result;
    }

    static buildGridFromExisting(newRows: IDataGridCtrl[], existing: DataGridMetaData): DataGridMetaData {
        const result = new DataGridMetaData();
        result.rows = newRows;
        result.columns = existing.columns;
        return result;
    }

    setDetailRowHeaderText(headerText: string) {
        this.detailRowHeaderText = headerText;
    }

    setDetailRowPresentation(value: CellPresentation[]) {
        this.detailRowPresentation = value;
    }

    setDropDownMenuPresentation(value: GridAction[]) {
        this.hasDropdown = true;
        this.rowActions = value;
    }
}
