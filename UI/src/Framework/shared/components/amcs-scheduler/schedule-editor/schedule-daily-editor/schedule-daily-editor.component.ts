import { Component, Input, OnInit } from '@angular/core';
import { FormDisplayModeEnum } from '@core-module/models/forms/form-display-model.enum';
import { FormOptions } from '@shared-module/components/layouts/amcs-form/form-options.model';
import { ScheduleForm } from '@shared-module/models/amcs-scheduler/schedule-form.model';

@Component({
  selector: 'app-schedule-daily-editor',
  templateUrl: './schedule-daily-editor.component.html',
  styleUrls: ['./schedule-daily-editor.component.scss'],
})
export class ScheduleDailyEditorComponent implements OnInit {
  @Input() form: ScheduleForm;
  options = new FormOptions();

  ngOnInit(): void {
    this.options.enableFormActions = false;
    this.options.displayMode = FormDisplayModeEnum.Grid;
  }
}
