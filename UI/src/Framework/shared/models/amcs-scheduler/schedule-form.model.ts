import { formatDate } from '@angular/common';
import { FormControl, Validators } from '@angular/forms';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { BaseForm } from '@shared-module/forms/base-form.model';
import { DailyOccurrence } from '@shared-module/models/amcs-scheduler/daily-occurrence.model';
import { MonthDaysTypeEnum } from '@shared-module/models/amcs-scheduler/month/month-days-type.enum';
import { MonthDays } from '@shared-module/models/amcs-scheduler/month/month-days.model';
import { MonthOffset } from '@shared-module/models/amcs-scheduler/month/month-offset.model';
import { MonthlyOccurrence } from '@shared-module/models/amcs-scheduler/month/monthly-occurrence.model';
import { ScheduleDateUtil } from '@shared-module/models/amcs-scheduler/schedule-date.util';
import { ScheduleTypeEnum } from '@shared-module/models/amcs-scheduler/schedule-type.enum';
import { WeeklyOccurrence } from '@shared-module/models/amcs-scheduler/weekly-occurrence.model';
import { AmcsValidators } from '@shared-module/validators/AmcsValidators.model';
import { ScheduleRepeat } from './schedule-repeat.model';
import { Schedule } from './schedule.model';

export class ScheduleForm extends BaseForm<Schedule, ScheduleForm> {
  // settings
  scheduleType: FormControl; // ScheduleTypeEnum
  startDate: FormControl;
  startTime: FormControl;

  // advanced settings
  isRepeat: FormControl;
  repeatDuration: FormControl;
  repeatEvery: FormControl;
  isExpire: FormControl;
  expireDate: FormControl;
  expireTime: FormControl;

  // daily
  dailyOccurence: FormControl;

  // weekly
  weeklyOccurence: FormControl;
  weeklyDays: FormControl; // WeekDaysEnum

  // months
  months: FormControl; // MonthsEnum
  monthDaysType: FormControl; // MonthDaysTypeEnum

  // month offset
  monthDays: FormControl; // MonthDaysEnum
  monthlyWeeks: FormControl; // MonthWeeksEnum
  monthlyWeekDays: FormControl; // WeekDaysEnum

  isNew: FormControl;

  buildForm(dataModel: Schedule): ScheduleForm {
    const form = new ScheduleForm();
    const defaultOccurence = new DailyOccurrence();
    defaultOccurence.repeat = 1;
    const defaultDate = AmcsDate.now();
    // Schedules don't have Id property so it's 'New' if no start date
    form.isNew = new FormControl(dataModel.start ? 1 : null);
    if (!dataModel.start) {
      dataModel.occurence = defaultOccurence;
      dataModel.start = defaultDate;
    }
    form.scheduleType = new FormControl(dataModel.occurence.type, Validators.required);
    form.startDate = new FormControl(AmcsDate.createFrom(dataModel.start), Validators.required);
    form.startTime = new FormControl(dataModel.start, Validators.required);

    form.isExpire = new FormControl(!!dataModel.expire);
    form.expireDate = new FormControl(AmcsDate.createFrom(dataModel.expire));
    form.expireTime = new FormControl(dataModel.expire);

    const isRepeat = !!dataModel.repeat;
    form.isRepeat = new FormControl(isRepeat);
    // Duration/Interval are in seconds, we create a date then set the seconds
    form.repeatDuration = new FormControl(isRepeat ? ScheduleDateUtil.setTimespanInSeconds(dataModel.repeat.duration) : null);
    form.repeatEvery = new FormControl(isRepeat ? ScheduleDateUtil.setTimespanInSeconds(dataModel.repeat.interval) : null);

    form.dailyOccurence = new FormControl(dataModel.occurence instanceof DailyOccurrence ? dataModel.occurence.repeat : null);

    form.weeklyOccurence = new FormControl(dataModel.occurence instanceof WeeklyOccurrence ? dataModel.occurence.repeat : null);
    form.weeklyDays = new FormControl(dataModel.occurence instanceof WeeklyOccurrence ? dataModel.occurence.days : null);

    form.months = new FormControl(dataModel.occurence instanceof MonthlyOccurrence ? dataModel.occurence.months : null);
    if (dataModel.occurence instanceof MonthlyOccurrence && dataModel.occurence.days instanceof MonthDays) {
      form.monthDaysType = new FormControl(MonthDaysTypeEnum[MonthDaysTypeEnum.days]);
      form.monthDays = new FormControl(dataModel.occurence.days.days);
      form.monthlyWeeks = new FormControl(null);
      form.monthlyWeekDays = new FormControl(null);
    } else if (dataModel.occurence instanceof MonthlyOccurrence && dataModel.occurence.days instanceof MonthOffset) {
      form.monthDaysType = new FormControl(MonthDaysTypeEnum[MonthDaysTypeEnum.offset]);
      form.monthDays = new FormControl(null);
      form.monthlyWeeks = new FormControl(dataModel.occurence.days.week);
      form.monthlyWeekDays = new FormControl(dataModel.occurence.days.days);
    } else {
      form.monthDaysType = new FormControl(MonthDaysTypeEnum[MonthDaysTypeEnum.days]);
      form.monthDays = new FormControl(null);
      form.monthlyWeeks = new FormControl(null);
      form.monthlyWeekDays = new FormControl(null);
    }
    ScheduleForm.setAllConditionalValidation(form);
    return form;
  }

  parseForm(typedForm: ScheduleForm): Schedule {
    let schedule = new Schedule();
    // Start/Expire use date picker + timepicker so must combine them
    schedule.start = ScheduleDateUtil.combineDateAndTime(typedForm.startDate.value, typedForm.startTime.value);
    if (typedForm.isExpire.value) {
      schedule.expire = ScheduleDateUtil.combineDateAndTime(typedForm.expireDate.value, typedForm.expireTime.value);
    }
    if (typedForm.isRepeat.value) {
      schedule.repeat = new ScheduleRepeat();
      // Duration/Interval are dates here so convert back to seconds
      schedule.repeat.duration = ScheduleDateUtil.getTimespanInSeconds(typedForm.repeatDuration.value);
      schedule.repeat.interval = ScheduleDateUtil.getTimespanInSeconds(typedForm.repeatEvery.value);
    }
    let occurrence;
    switch (typedForm.scheduleType.value) {
      case ScheduleTypeEnum[ScheduleTypeEnum.daily]:
        occurrence = new DailyOccurrence();
        occurrence.repeat = typedForm.dailyOccurence.value;
        break;

      case ScheduleTypeEnum[ScheduleTypeEnum.weekly]:
        occurrence = new WeeklyOccurrence();
        occurrence.repeat = typedForm.weeklyOccurence.value;
        occurrence.days = typedForm.weeklyDays.value;
        break;

      case ScheduleTypeEnum[ScheduleTypeEnum.monthly]:
        occurrence = new MonthlyOccurrence();
        occurrence.months = typedForm.months.value;
        let monthDays;
        switch (typedForm.monthDaysType.value) {
          case MonthDaysTypeEnum[MonthDaysTypeEnum.days]:
            monthDays = new MonthDays();
            monthDays.days = typedForm.monthDays.value;
            break;

          case MonthDaysTypeEnum[MonthDaysTypeEnum.offset]:
            monthDays = new MonthOffset();
            monthDays.week = typedForm.monthlyWeeks.value;
            monthDays.days = typedForm.monthlyWeekDays.value;
            break;
        }
        occurrence.days = monthDays;
        break;
    }
    schedule.occurence = occurrence;
    return schedule;
  }

  getPrimaryKeyControl(): FormControl {
    return this.isNew;
  }

  static setRepeatConditionalValidation(form: ScheduleForm) {
    form.repeatDuration.clearValidators();
    form.repeatEvery.clearValidators();
    form.repeatDuration.disable();
    form.repeatEvery.disable();
    if (form.isRepeat.value) {
      form.repeatDuration.setValidators(Validators.required);
      form.repeatEvery.setValidators(Validators.required);
      form.repeatDuration.enable();
      form.repeatEvery.enable();
    }
    form.htmlFormGroup?.updateValueAndValidity();
  }

  static setExpireConditionalValidation(form: ScheduleForm) {
    form.expireDate.clearValidators();
    form.expireDate.disable();
    if (form.isExpire.value) {
      form.expireDate.setValidators([Validators.required, AmcsValidators.dateMin(form.startDate.value)]);
      form.expireDate.enable();
    }
    this.setExpiryTimeValidation(form);
    form.htmlFormGroup?.updateValueAndValidity();
  }

  static setExpiryTimeValidation(form: ScheduleForm) {
    form.expireTime.clearValidators();
    form.expireTime.disable();
    if (form.isExpire.value) {
      if (formatDate(form.expireDate.value, 'dd-mm-yyyy', 'en_GB') === formatDate(form.startDate.value, 'dd-mm-yyyy', 'en_GB')) {
        form.expireTime.setValidators([Validators.required, AmcsValidators.dateMin(form.startTime.value)]);
      } else {
        form.expireTime.setValidators(Validators.required);
      }
      form.expireTime.enable();
    }
    form.htmlFormGroup?.updateValueAndValidity();
  }

  static setDailyConditionalValidation(form: ScheduleForm) {
    form.dailyOccurence.clearValidators();
    if (form.scheduleType.value === ScheduleTypeEnum[ScheduleTypeEnum.daily]) {
      if (form.dailyOccurence.value === null) {
        form.dailyOccurence.setValue(1);
      }
      form.dailyOccurence.setValidators(Validators.required);
    }
    form.htmlFormGroup?.updateValueAndValidity();
  }

  static setWeeklyConditionalValidation(form: ScheduleForm) {
    form.weeklyOccurence.clearValidators();
    form.weeklyDays.clearValidators();
    if (form.scheduleType.value === ScheduleTypeEnum[ScheduleTypeEnum.weekly]) {
      if (form.weeklyOccurence.value === null) {
        form.weeklyOccurence.setValue(1);
      }
      form.weeklyOccurence.setValidators(Validators.required);
      form.weeklyDays.setValidators(Validators.required);
    }
    form.htmlFormGroup?.updateValueAndValidity();
  }

  static setMonthlyConditionalValidation(form: ScheduleForm) {
    form.months.clearValidators();
    form.monthDays.clearValidators();
    form.monthlyWeeks.clearValidators();
    form.monthlyWeekDays.clearValidators();
    form.monthDays.disable();
    form.monthlyWeeks.disable();
    form.monthlyWeekDays.disable();
    if (form.scheduleType.value === ScheduleTypeEnum[ScheduleTypeEnum.monthly]) {
      form.months.setValidators(Validators.required);
      if (form.monthDaysType.value === MonthDaysTypeEnum[MonthDaysTypeEnum.days]) {
        form.monthDays.setValidators(Validators.required);
        form.monthDays.enable();
      } else {
        form.monthlyWeeks.setValidators(Validators.required);
        form.monthlyWeekDays.setValidators(Validators.required);
        form.monthlyWeeks.enable();
        form.monthlyWeekDays.enable();
      }
    }
    form.htmlFormGroup?.updateValueAndValidity();
  }

  private static setAllConditionalValidation(form: ScheduleForm) {
    this.setRepeatConditionalValidation(form);
    this.setExpireConditionalValidation(form);
    this.setDailyConditionalValidation(form);
    this.setWeeklyConditionalValidation(form);
    this.setMonthlyConditionalValidation(form);
  }
}
