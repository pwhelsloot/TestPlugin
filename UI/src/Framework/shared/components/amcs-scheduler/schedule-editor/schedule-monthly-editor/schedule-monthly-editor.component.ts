import { Component, Input, OnInit } from '@angular/core';
import { FormDisplayModeEnum } from '@core-module/models/forms/form-display-model.enum';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { FormOptions } from '@shared-module/components/layouts/amcs-form/form-options.model';
import { MonthDaysTypeEnum } from '@shared-module/models/amcs-scheduler/month/month-days-type.enum';
import { MonthDayEnum } from '@shared-module/models/amcs-scheduler/month/month-days.enum';
import { MonthWeekEnum } from '@shared-module/models/amcs-scheduler/month/month-weeks.enum';
import { MonthsEnum } from '@shared-module/models/amcs-scheduler/month/months.enum';
import { ScheduleForm } from '@shared-module/models/amcs-scheduler/schedule-form.model';
import { WeekDaysEnum } from '@shared-module/models/amcs-scheduler/week-days.enum';

@Component({
  selector: 'app-schedule-monthly-editor',
  templateUrl: './schedule-monthly-editor.component.html',
  styleUrls: ['./schedule-monthly-editor.component.scss'],
})
export class ScheduleMonthlyEditorComponent implements OnInit {
  @Input() form: ScheduleForm;
  MonthDaysTypeEnum = MonthDaysTypeEnum;
  months: ILookupItem[] = [];
  monthDays: ILookupItem[] = [];
  monthWeeks: ILookupItem[] = [];
  monthWeekDays: ILookupItem[] = [];
  options = new FormOptions();

  ngOnInit(): void {
    this.options.enableFormActions = false;
    this.options.displayMode = FormDisplayModeEnum.Grid;
    this.populateLookups();
  }

  private populateLookups() {
    for (let month in MonthsEnum) {
      if (isNaN(Number(month))) {
        this.months.push({ id: Number(MonthsEnum[month]), description: month });
      }
    }
    for (let monthWeekDay in WeekDaysEnum) {
      if (isNaN(Number(monthWeekDay))) {
        this.monthWeekDays.push({ id: Number(WeekDaysEnum[monthWeekDay]), description: monthWeekDay });
      }
    }
    // For two below arrays we use bindValue guid so we get a number represented as a string for the value (e.g '2')
    for (let monthDay in MonthDayEnum) {
      if (isNaN(Number(monthDay))) {
        // Last is a special case where instead of '32' we want description to be 'Last'
         if (monthDay === MonthDayEnum[MonthDayEnum.Last]) {
          this.monthDays.push({ id: null, guid: MonthDayEnum[monthDay].toString(), description: monthDay });
        } else {
          this.monthDays.push({ id: null, guid: MonthDayEnum[monthDay].toString(), description: MonthDayEnum[monthDay] });
        }
      }
    }
    for (let monthWeek in MonthWeekEnum) {
      if (isNaN(Number(monthWeek))) {
        // Last is a special case where instead of '5' we want value to be 'Last'
        if (monthWeek === MonthWeekEnum[MonthWeekEnum.Last]) {
          this.monthWeeks.push({ id: null, guid: monthWeek, description: monthWeek });
        } else {
          this.monthWeeks.push({ id: null, guid: MonthWeekEnum[monthWeek].toString(), description: monthWeek });
        }
      }
    }
  }
}
