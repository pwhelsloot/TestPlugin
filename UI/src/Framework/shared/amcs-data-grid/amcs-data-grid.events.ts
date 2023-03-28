import { ColumnApi, GridApi, GridReadyEvent } from 'ag-grid-community';
import { ChangedRowAmountHelper } from './helpers/changed-row-amount-helper';

// eslint-disable-next-line @typescript-eslint/no-empty-interface
export class AmcsGridReadyEvent implements GridReadyEvent {
  api: GridApi<any>;
  columnApi: ColumnApi;
  type: string;
  rowAmount: ChangedRowAmountHelper;
}
