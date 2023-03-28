import { Component } from '@angular/core';
import { ICellRendererParams } from 'ag-grid-enterprise';
import { ICellRendererAngularComp } from 'ag-grid-angular';

@Component({
  selector: 'app-single-click-checkbox-cell-renderer',
  templateUrl: './single-click-checkbox-cell-renderer.component.html',
  styleUrls: ['./single-click-checkbox-cell-renderer.component.scss']
})
export class SingleClickCheckboxCellRendererComponent implements ICellRendererAngularComp {
  params: any;
  isEditable: boolean;

  agInit(params: any): void {
    this.params = params;
    this.isEditable = params.colDef.cellEditorParams.isEditable;
  }

  onChange(event) {
    let checked = event.target.checked;
    let colId = this.params.column.colId;
    this.params.node.setDataValue(colId, checked);
  }

  refresh(params: ICellRendererParams<any>): boolean {
    return false;
  }

}
