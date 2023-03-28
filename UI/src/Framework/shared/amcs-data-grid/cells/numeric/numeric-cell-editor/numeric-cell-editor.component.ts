import { AfterViewInit, Component, ViewChild, ViewContainerRef } from '@angular/core';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { AmcsNumericalInputComponent } from '@shared-module/components/amcs-numerical-input/amcs-numerical-input.component';
import { ICellEditorAngularComp } from 'ag-grid-angular';
import { ICellEditorParams } from 'ag-grid-community';

@Component({
  selector: 'app-numeric-cell-editor',
  templateUrl: './numeric-cell-editor.component.html',
  styleUrls: ['./numeric-cell-editor.component.scss'],
})
export class NumericCellEditorComponent implements ICellEditorAngularComp, AfterViewInit {
  @ViewChild(AmcsNumericalInputComponent, { read: ViewContainerRef }) input!: ViewContainerRef;

  cellValue!: number;
  displayMode = FormControlDisplay.Grid;
  noMargin = true;
  noPadding = true;
  isInitializing = true;
  precision = 2;
  private params!: ICellEditorParams<Number>;

  ngAfterViewInit() {
    setTimeout(() => this.input.element.nativeElement.focus());
  }

  agInit(params: ICellEditorParams<Number>): void {
    this.params = params;
    this.precision = params['precision'] ?? 2;
    this.cellValue = this.params.value;
  }

  onChange(event: number) {
    this.cellValue = event;
  }

  getValue() {
    return this.cellValue;
  }

  isCancelBeforeStart() {
    return false;
  }

  isCancelAfterEnd() {
    return false;
  }
}
