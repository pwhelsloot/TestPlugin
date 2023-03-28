import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import { ICellEditorAngularComp } from 'ag-grid-angular';
import { ICellEditorParams } from 'ag-grid-enterprise';

@Component({
  selector: 'app-checkbox-cell-editor',
  templateUrl: './checkbox-cell-editor.component.html',
  styleUrls: ['./checkbox-cell-editor.component.scss'],
})
export class CheckboxCellEditorComponent implements ICellEditorAngularComp, AfterViewInit {
  @ViewChild('checkboxInput') checkbox!: ElementRef;
  cellValue!: boolean;

  private params!: ICellEditorParams<boolean>;

  ngAfterViewInit() {
    setTimeout(() => this.checkbox.nativeElement.focus());
  }

  agInit(params: ICellEditorParams<boolean>): void {
    this.params = params;
    this.cellValue = this.params.value;
  }

  onChange(event: boolean) {
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
