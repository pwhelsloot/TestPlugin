import { DatePipe } from '@angular/common';
import { Component } from '@angular/core';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-community';

@Component({
  selector: 'app-date-cell-renderer',
  templateUrl: './date-cell-renderer.component.html',
  styleUrls: ['./date-cell-renderer.component.scss'],
})
export class DateCellRendererComponent implements ICellRendererAngularComp {
  cellValue: string;
  readonly = true;
  disabled = true;
  noMargin = true;
  noPadding = true;
  displayMode = FormControlDisplay.Grid;

  constructor(private readonly datePipe: DatePipe) {}

  agInit(params: ICellRendererParams<Date>): void {
    this.cellValue = this.getValueToDisplay(params);
  }

  refresh(params: ICellRendererParams<Date>): boolean {
    this.cellValue = this.getValueToDisplay(params);
    return true;
  }

  private getValueToDisplay(params: ICellRendererParams<Date>) {
    if (typeof params.value === 'string') {
      const date = new Date(params.value);
      if (isValidDate(date)) {
        return this.datePipe.transform(date);
      }
      return null;
    }
    return this.datePipe.transform(params.value);
  }
}

export function dateCellValueFormatter(value: Date | string) {
  if (!value) {
    return null;
  }
  if (typeof value === 'string') {
    const date = new Date(value);
    if (isValidDate(date)) {
      return AmcsDate.createFrom(date).toISOString();
    }
    return null;
  }
  return AmcsDate.createFrom(value).toISOString();
}

function isValidDate(date: any) {
  return !isNaN(date) && date instanceof Date;
}
