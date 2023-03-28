import { AfterViewInit, Component, ViewChild, ViewContainerRef } from '@angular/core';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { AmcsDatepickerConfig } from '@shared-module/components/amcs-datepicker/amcs-datepicker-config.model';
import { AmcsDatepickerComponent } from '@shared-module/components/amcs-datepicker/amcs-datepicker.component';
import { ICellEditorAngularComp } from 'ag-grid-angular';
import { ICellEditorParams } from 'ag-grid-community';
import { DateCellEditorConfig } from './date-celll-editor.config';

@Component({
  selector: 'app-date-cell-editor',
  templateUrl: './date-cell-editor.component.html',
  styleUrls: ['./date-cell-editor.component.scss'],
})
export class DateCellEditorComponent implements ICellEditorAngularComp, AfterViewInit {
  @ViewChild(AmcsDatepickerComponent, { read: ViewContainerRef }) input!: ViewContainerRef;

  cellValue: Date;
  noMargin = true;
  noPadding = true;
  displayMode = FormControlDisplay.Grid;
  isFirstChange = true;
  config: AmcsDatepickerConfig;
  private params!: ICellEditorParams<Date>;

  ngAfterViewInit() {
    setTimeout(() => this.input.element.nativeElement.focus());
  }

  agInit(params: ICellEditorParams<Date>): void {
    this.params = params;
    if (this.params['config']) {
      this.processEditorConfig();
    }
    this.cellValue = this.params.value === null ? new Date() : this.params.value;
  }

  onChange(event: Date) {
    if (this.isFirstChange) {
      this.isFirstChange = false;
    } else {
      this.cellValue = event;
      this.params.stopEditing();
    }
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

  private processEditorConfig() {
    const cellConfig: DateCellEditorConfig = this.params['config'];
    this.config = new AmcsDatepickerConfig();
    if (cellConfig.range) {
      this.config.minDate = AmcsDate.createFrom(this.params.data[cellConfig.range.min]);
      this.config.maxDate = AmcsDate.createFrom(this.params.data[cellConfig.range.max]);
    }
  }
}
