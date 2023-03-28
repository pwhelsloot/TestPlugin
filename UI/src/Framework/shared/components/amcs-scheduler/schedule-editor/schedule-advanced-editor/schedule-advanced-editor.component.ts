import { Component, Input, OnInit } from '@angular/core';
import { FormDisplayModeEnum } from '@core-module/models/forms/form-display-model.enum';
import { AmcsDatepickerConfig } from '@shared-module/components/amcs-datepicker/amcs-datepicker-config.model';
import { FormOptions } from '@shared-module/components/layouts/amcs-form/form-options.model';
import { ScheduleForm } from '@shared-module/models/amcs-scheduler/schedule-form.model';

@Component({
  selector: 'app-schedule-advanced-editor',
  templateUrl: './schedule-advanced-editor.component.html',
  styleUrls: ['./schedule-advanced-editor.component.scss'],
})
export class ScheduleAdvancedEditorComponent implements OnInit {
  @Input() form: ScheduleForm;
  dateConfig = new AmcsDatepickerConfig();
  options = new FormOptions();

  ngOnInit(): void {
    this.options.enableFormActions = false;
    this.options.displayMode = FormDisplayModeEnum.Grid;
  }
}
