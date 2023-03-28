import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import { CellValueChangedEvent, PasteEndEvent, PasteStartEvent } from 'ag-grid-community';
import { AmcsDataGridRowData } from './amcs-data-grid-row-data';
import { AmcsGridReadyEvent } from './amcs-data-grid.events';
import { AmcsDataGridOptions } from './amcs-data-grid.options';
import { AmcsColDefs } from './amcs-column-definitions';
import { DateCellRendererComponent, dateCellValueFormatter } from './cells/date/date-cell-renderer/date-cell-renderer.component';
import { ChangedRowAmountHelper } from './helpers/changed-row-amount-helper';

@Component({
  selector: 'app-amcs-data-grid',
  templateUrl: './amcs-data-grid.component.html',
  styleUrls: ['./amcs-data-grid.component.scss'],
})
export class AmcsDataGridComponent {
  @ViewChild(AgGridAngular, { static: true }) grid!: AgGridAngular;

  /**
   * RowData, this is prioritized over Options
   */
  get rowData(): AmcsDataGridRowData[] {
    return this._rowData;
  }

  @Input() set rowData(value: AmcsDataGridRowData[]) {
    this._rowData = value;
    if (this.options) {
      this.options.undoRedoCellEditing = value?.length < 50000;
    }
  }

  /**
   * ColumnDefinitions, this is prioritized over Options
   */
  @Input() columnDefs: AmcsColDefs;

  /**
   * Options
   */
  @Input() options: AmcsDataGridOptions<AmcsDataGridRowData>;

  @Output() gridReadyEvent = new EventEmitter<AmcsGridReadyEvent>();

  @Output() rowChangesAmount = new EventEmitter<number>();

  get defaultColDef() {
    return this.options?.defaultColDef;
  }

  isPasting = false;
  changedRowAmountHelper = new ChangedRowAmountHelper();

  private _rowData: AmcsDataGridRowData[];

  /**
   * Emitted when grid is ready
   * @param params
   */
  onGridReadyEvent(params: AmcsGridReadyEvent) {
    params.rowAmount = this.changedRowAmountHelper;
    this.gridReadyEvent.emit(params);
  }

  onCellValueChanged(event: CellValueChangedEvent) {
    if (event.colDef.field === this.options.errorField) {
      return;
    }

    let oldValue: string | Date, newValue: string | Date;
    if (event.column.getColDef().cellRenderer === DateCellRendererComponent) {
      oldValue = dateCellValueFormatter(event.oldValue);
      newValue = dateCellValueFormatter(event.newValue);
    } else {
      oldValue = event.oldValue?.toString();
      newValue = event.newValue?.toString();
    }

    if (oldValue !== newValue) {
      this.changedRowAmountHelper.storeEdit(event.node.id, event.colDef.field);
    }

    if (!this.isPasting) {
      this.emitRowChangesAmount();
    }
  }

  /**
   * Set pasting
   */
  onPasteStart(event: PasteStartEvent) {
    this.isPasting = true;
  }

  /**
   * Pasting event has ended, emit rowchanges amount once
   */
  onPasteEnd(event: PasteEndEvent) {
    this.isPasting = false;
    this.emitRowChangesAmount();
  }

  /**
   * Emit the row changes amount calculated using the unique row indexes
   */
  private emitRowChangesAmount() {
    this.rowChangesAmount.emit(this.changedRowAmountHelper.getChangeAmount());
  }
}
