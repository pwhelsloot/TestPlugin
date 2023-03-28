import { coerceBooleanProperty } from '@angular/cdk/coercion';
import { Component, ElementRef, ViewChild } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-community';

@Component({
  selector: 'app-checkbox-cell-renderer',
  templateUrl: './checkbox-cell-renderer.component.html',
  styleUrls: ['./checkbox-cell-renderer.component.scss'],
})
export class CheckboxCellRendererComponent implements ICellRendererAngularComp {
  @ViewChild('checkboxInput') checkbox!: ElementRef;

  cellValue: boolean;

  agInit(params: ICellRendererParams<boolean>): void {
    this.cellValue = this.getValueToDisplay(params);
  }

  refresh(params: ICellRendererParams<boolean>): boolean {
    this.cellValue = this.getValueToDisplay(params);
    return true;
  }

  private getValueToDisplay(params: ICellRendererParams<boolean>) {
    return checkboxCellValueFormatter(params.value);
  }
}

export function checkboxCellValueFormatter(value: string | boolean) {
  return coerceBooleanProperty(value.toString().toLowerCase());
}
