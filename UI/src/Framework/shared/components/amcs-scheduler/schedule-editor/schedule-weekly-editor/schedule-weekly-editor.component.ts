import { Component, Input, OnInit } from '@angular/core';
import { FormDisplayModeEnum } from '@core-module/models/forms/form-display-model.enum';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { FormOptions } from '@shared-module/components/layouts/amcs-form/form-options.model';
import { ScheduleForm } from '@shared-module/models/amcs-scheduler/schedule-form.model';
import { WeekDaysEnum } from '@shared-module/models/amcs-scheduler/week-days.enum';

@Component({
  selector: 'app-schedule-weekly-editor',
  templateUrl: './schedule-weekly-editor.component.html',
  styleUrls: ['./schedule-weekly-editor.component.scss'],
})
export class ScheduleWeeklyEditorComponent implements OnInit {
  @Input() form: ScheduleForm;
  weekDays: ILookupItem[] = [];
  options = new FormOptions();

  ngOnInit(): void {
    this.options.enableFormActions = false;
    this.options.displayMode = FormDisplayModeEnum.Grid;
    this.populateLookups();
  }

  private populateLookups() {
    for (let monthWeekDay in WeekDaysEnum) {
      if (isNaN(Number(monthWeekDay))) {
        this.weekDays.push({ id: null, description: monthWeekDay });
      }
    }
  }
}
