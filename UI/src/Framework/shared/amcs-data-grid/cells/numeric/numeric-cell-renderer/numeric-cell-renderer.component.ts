import { coerceNumberProperty } from '@angular/cdk/coercion';
import { DecimalPipe } from '@angular/common';
import { Component } from '@angular/core';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-enterprise';

@Component({
  selector: 'app-numeric-cell-renderer',
  templateUrl: './numeric-cell-renderer.component.html',
  styleUrls: ['./numeric-cell-renderer.component.scss'],
})
export class NumericCellRendererComponent implements ICellRendererAngularComp {
  cellValue: string | number;
  displayMode = FormControlDisplay.Grid;
  noMargin = true;
  noPadding = true;
  isDisabled = true;
  precision = 2;

  constructor(private readonly decimalPipe: DecimalPipe) {}

  agInit(params: ICellRendererParams<Number>): void {
    this.precision = params['precision'] ?? 2;
    this.cellValue = this.getValueToDisplay(params);
  }

  refresh(params: ICellRendererParams<Number>): boolean {
    this.cellValue = this.getValueToDisplay(params);
    return true;
  }

  private getValueToDisplay(params: ICellRendererParams<Number>) {
    const number = coerceNumberProperty(params.value);
    if (number && typeof number === 'number') {
      return this.decimalPipe.transform(params.value, `1.0-${this.precision}`);
    }
    return null;
  }
}

export function numericCellValueFormatter(value: string | number) {
  const number = coerceNumberProperty(value);
  if (number && typeof number === 'number') {
    return value;
  }
  return null;
}
