import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-enterprise';

@Component({
  selector: 'app-validation-cell-renderer',
  templateUrl: './validation-cell-renderer.component.html',
  styleUrls: ['./validation-cell-renderer.component.scss'],
})
export class ValidationCellRendererComponent implements ICellRendererAngularComp {
  cellValue: string | number;

  agInit(params: ICellRendererParams<Number>): void {
    this.cellValue = this.getValueToDisplay(params);
  }

  refresh(params: ICellRendererParams<Number>): boolean {
    this.cellValue = this.getValueToDisplay(params);
    return true;
  }

  private getValueToDisplay(params: ICellRendererParams<Number>) {
    return params.value;
  }
}
