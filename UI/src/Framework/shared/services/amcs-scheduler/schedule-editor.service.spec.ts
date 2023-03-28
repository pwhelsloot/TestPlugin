import { EventEmitter } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { ReactiveFormsModule, Validators } from '@angular/forms';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { DailyOccurrence } from '@shared-module/models/amcs-scheduler/daily-occurrence.model';
import { MonthDaysTypeEnum } from '@shared-module/models/amcs-scheduler/month/month-days-type.enum';
import { MonthDays } from '@shared-module/models/amcs-scheduler/month/month-days.model';
import { MonthOffset } from '@shared-module/models/amcs-scheduler/month/month-offset.model';
import { MonthlyOccurrence } from '@shared-module/models/amcs-scheduler/month/monthly-occurrence.model';
import { ScheduleDateUtil } from '@shared-module/models/amcs-scheduler/schedule-date.util';
import { ScheduleEditorApi } from '@shared-module/models/amcs-scheduler/schedule-editor-api.model';
import { ScheduleRepeat } from '@shared-module/models/amcs-scheduler/schedule-repeat.model';
import { ScheduleTypeEnum } from '@shared-module/models/amcs-scheduler/schedule-type.enum';
import { Schedule } from '@shared-module/models/amcs-scheduler/schedule.model';
import { WeeklyOccurrence } from '@shared-module/models/amcs-scheduler/weekly-occurrence.model';
import { ScheduleEditorService } from '@shared-module/services/amcs-scheduler/schedule-editor.service';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { MockProvider } from 'ng-mocks';
import { take } from 'rxjs/operators';

describe('ScheduleEditorService', () => {
  let dailySchedule: Schedule;
  let weeklySchedule: Schedule;
  let monthlyDateSchedule: Schedule;
  let monthlyOccurenceSchedule: Schedule;
  let onSave: EventEmitter<Schedule>;
  let onCancel: EventEmitter<void>;
  let service: ScheduleEditorService;
  const onScheduleEditorReady = new EventEmitter<ScheduleEditorApi>();

  beforeEach(() => {
    onSave = new EventEmitter<Schedule>();
    onCancel = new EventEmitter<void>();

    dailySchedule = new Schedule();
    dailySchedule.start = new Date(AmcsDate.create().setHours(6));
    dailySchedule.occurence = new DailyOccurrence();
    dailySchedule.occurence.repeat = 2;

    weeklySchedule = new Schedule();
    weeklySchedule.start = new Date(AmcsDate.create().setHours(1));
    weeklySchedule.occurence = new WeeklyOccurrence();
    (weeklySchedule.occurence as WeeklyOccurrence).days = ['Wed, Fri'];
    weeklySchedule.occurence.repeat = 3;
    weeklySchedule.expire = new Date(AmcsDate.create().setHours(3));

    monthlyDateSchedule = new Schedule();
    monthlyDateSchedule.start = new Date(AmcsDate.create().setHours(2));
    monthlyDateSchedule.occurence = new MonthlyOccurrence();
    (monthlyDateSchedule.occurence as MonthlyOccurrence).months = ['Feb', 'Mar'];
    monthlyDateSchedule.occurence.days = new MonthDays();
    (monthlyDateSchedule.occurence.days as MonthDays).days = ['1', '2', '8'];
    monthlyDateSchedule.repeat = new ScheduleRepeat();
    monthlyDateSchedule.repeat.duration = 1001;
    monthlyDateSchedule.repeat.interval = 25;

    monthlyOccurenceSchedule = new Schedule();
    monthlyOccurenceSchedule.start = new Date(AmcsDate.create().setHours(10));
    monthlyOccurenceSchedule.occurence = new MonthlyOccurrence();
    (monthlyOccurenceSchedule.occurence as MonthlyOccurrence).months = ['Jan', 'Dec'];
    monthlyOccurenceSchedule.occurence.days = new MonthOffset();
    (monthlyOccurenceSchedule.occurence.days as MonthOffset).week = ['1', 'Last'];
    (monthlyOccurenceSchedule.occurence.days as MonthOffset).days = ['Mon', 'Fri'];

    TestBed.configureTestingModule({
      imports: [ReactiveFormsModule],
      providers: [ScheduleEditorService, MockProvider(SharedTranslationsService)]
    });
    service = TestBed.inject(ScheduleEditorService);
  });

  it('init of new schedule builds default form', () => {
    // act
    service.init(undefined, onSave, onCancel, onScheduleEditorReady, false);

    // asset
    expect(service.form.isNew.value).toEqual(null);
    expect(service.form.scheduleType.value).toEqual(ScheduleTypeEnum[ScheduleTypeEnum.daily]);
    expect(service.form.startDate.value).toEqual(AmcsDate.create());
    expect(service.form.startTime.value.getMilliseconds()).toBeLessThanOrEqual(AmcsDate.now().getMilliseconds());
    expect(service.form.isRepeat.value).toEqual(false);
    expect(service.form.repeatDuration.value).toEqual(null);
    expect(service.form.repeatEvery.value).toEqual(null);
    expect(service.form.isExpire.value).toEqual(false);
    expect(service.form.expireDate.value).toEqual(null);
    expect(service.form.expireTime.value).toEqual(null);
    expect(service.form.dailyOccurence.value).toEqual(1);
    expect(service.form.weeklyOccurence.value).toEqual(null);
    expect(service.form.weeklyDays.value).toEqual(null);
    expect(service.form.months.value).toEqual(null);
    expect(service.form.monthDaysType.value).toEqual(MonthDaysTypeEnum[MonthDaysTypeEnum.days]);
    expect(service.form.monthDays.value).toEqual(null);
    expect(service.form.monthlyWeeks.value).toEqual(null);
    expect(service.form.monthlyWeekDays.value).toEqual(null);
  });

  it('init of dailyschedule builds daily form', () => {
    // act
    service.init(dailySchedule, onSave, onCancel, onScheduleEditorReady, false);

    // asset
    expect(service.form.isNew.value).toEqual(1);
    expect(service.form.scheduleType.value).toEqual(ScheduleTypeEnum[ScheduleTypeEnum.daily]);
    expect(service.form.startDate.value).toEqual(AmcsDate.create());
    expect(service.form.startTime.value).toEqual(new Date(AmcsDate.create().setHours(6)));
    expect(service.form.isRepeat.value).toEqual(false);
    expect(service.form.repeatDuration.value).toEqual(null);
    expect(service.form.repeatEvery.value).toEqual(null);
    expect(service.form.isExpire.value).toEqual(false);
    expect(service.form.expireDate.value).toEqual(null);
    expect(service.form.expireTime.value).toEqual(null);
    expect(service.form.dailyOccurence.value).toEqual(2);
    expect(service.form.weeklyOccurence.value).toEqual(null);
    expect(service.form.weeklyDays.value).toEqual(null);
    expect(service.form.months.value).toEqual(null);
    expect(service.form.monthDaysType.value).toEqual(MonthDaysTypeEnum[MonthDaysTypeEnum.days]);
    expect(service.form.monthDays.value).toEqual(null);
    expect(service.form.monthlyWeeks.value).toEqual(null);
    expect(service.form.monthlyWeekDays.value).toEqual(null);
  });

  it('init of weeklyschedule builds weekly form', () => {
    // act
    service.init(weeklySchedule, onSave, onCancel, onScheduleEditorReady, false);

    // asset
    expect(service.form.isNew.value).toEqual(1);
    expect(service.form.scheduleType.value).toEqual(ScheduleTypeEnum[ScheduleTypeEnum.weekly]);
    expect(service.form.startDate.value).toEqual(AmcsDate.create());
    expect(service.form.startTime.value).toEqual(new Date(AmcsDate.create().setHours(1)));
    expect(service.form.isRepeat.value).toEqual(false);
    expect(service.form.repeatDuration.value).toEqual(null);
    expect(service.form.repeatEvery.value).toEqual(null);
    expect(service.form.isExpire.value).toEqual(true);
    expect(service.form.expireDate.value).toEqual(AmcsDate.create());
    expect(service.form.expireTime.value).toEqual(new Date(AmcsDate.create().setHours(3)));
    expect(service.form.dailyOccurence.value).toEqual(null);
    expect(service.form.weeklyOccurence.value).toEqual(3);
    expect(service.form.weeklyDays.value).toEqual(['Wed, Fri']);
    expect(service.form.months.value).toEqual(null);
    expect(service.form.monthDaysType.value).toEqual(MonthDaysTypeEnum[MonthDaysTypeEnum.days]);
    expect(service.form.monthDays.value).toEqual(null);
    expect(service.form.monthlyWeeks.value).toEqual(null);
    expect(service.form.monthlyWeekDays.value).toEqual(null);
  });

  it('init of monthlyDateSchedule builds monthly date form', () => {
    // act
    service.init(monthlyDateSchedule, onSave, onCancel, onScheduleEditorReady, false);

    // asset
    expect(service.form.isNew.value).toEqual(1);
    expect(service.form.scheduleType.value).toEqual(ScheduleTypeEnum[ScheduleTypeEnum.monthly]);
    expect(service.form.startDate.value).toEqual(AmcsDate.create());
    expect(service.form.startTime.value).toEqual(new Date(AmcsDate.create().setHours(2)));
    expect(service.form.isRepeat.value).toEqual(true);
    expect(service.form.repeatDuration.value).toEqual(ScheduleDateUtil.setTimespanInSeconds(1001));
    expect(service.form.repeatEvery.value).toEqual(ScheduleDateUtil.setTimespanInSeconds(25));
    expect(service.form.isExpire.value).toEqual(false);
    expect(service.form.expireDate.value).toEqual(null);
    expect(service.form.expireTime.value).toEqual(null);
    expect(service.form.dailyOccurence.value).toEqual(null);
    expect(service.form.weeklyOccurence.value).toEqual(null);
    expect(service.form.weeklyDays.value).toEqual(null);
    expect(service.form.months.value).toEqual(['Feb', 'Mar']);
    expect(service.form.monthDaysType.value).toEqual(MonthDaysTypeEnum[MonthDaysTypeEnum.days]);
    expect(service.form.monthDays.value).toEqual(['1', '2', '8']);
    expect(service.form.monthlyWeeks.value).toEqual(null);
    expect(service.form.monthlyWeekDays.value).toEqual(null);
  });

  it('init of monthlyOccurenceSchedule builds monthly date form', () => {
    // act
    service.init(monthlyOccurenceSchedule, onSave, onCancel, onScheduleEditorReady, false);

    // asset
    expect(service.form.isNew.value).toEqual(1);
    expect(service.form.scheduleType.value).toEqual(ScheduleTypeEnum[ScheduleTypeEnum.monthly]);
    expect(service.form.startDate.value).toEqual(AmcsDate.create());
    expect(service.form.startTime.value).toEqual(new Date(AmcsDate.create().setHours(10)));
    expect(service.form.isRepeat.value).toEqual(false);
    expect(service.form.repeatDuration.value).toEqual(null);
    expect(service.form.repeatEvery.value).toEqual(null);
    expect(service.form.isExpire.value).toEqual(false);
    expect(service.form.expireDate.value).toEqual(null);
    expect(service.form.expireTime.value).toEqual(null);
    expect(service.form.dailyOccurence.value).toEqual(null);
    expect(service.form.weeklyOccurence.value).toEqual(null);
    expect(service.form.weeklyDays.value).toEqual(null);
    expect(service.form.months.value).toEqual(['Jan', 'Dec']);
    expect(service.form.monthDaysType.value).toEqual(MonthDaysTypeEnum[MonthDaysTypeEnum.offset]);
    expect(service.form.monthDays.value).toEqual(null);
    expect(service.form.monthlyWeeks.value).toEqual(['1', 'Last']);
    expect(service.form.monthlyWeekDays.value).toEqual(['Mon', 'Fri']);
  });

  it('parsing new schedule form results in default schedule', function(done) {
    // arrange
    let parsedSchedule: Schedule;
    onSave.pipe(take(1)).subscribe((value) => {
      parsedSchedule = value;
    });

    // act
    service.init(undefined, onSave, onCancel, onScheduleEditorReady, false);
    service.save();

    const defaultSchedule = new Schedule();
    const defaultOccurence = new DailyOccurrence();
    defaultOccurence.repeat = 1;
    const defaultDate = AmcsDate.now();
    defaultSchedule.occurence = defaultOccurence;
    defaultSchedule.start = defaultDate;

    // assert
    expect(parsedSchedule.start.toString()).toEqual(defaultSchedule.start.toString());
    expect(parsedSchedule.expire?.toString()).toEqual(defaultSchedule.expire?.toString());
    expect(parsedSchedule.repeat).toEqual(defaultSchedule.repeat);
    expect(parsedSchedule.occurence).toEqual(defaultSchedule.occurence);
    done();
  });

  it('parsing dailySchedule form results in dailySchedule', function(done) {
    // arrange
    let parsedSchedule: Schedule;
    onSave.pipe(take(1)).subscribe((value) => {
      parsedSchedule = value;
    });

    // act
    service.init(dailySchedule, onSave, onCancel, onScheduleEditorReady, false);
    service.save();

    // assert
    expect(parsedSchedule.start.toString()).toEqual(dailySchedule.start.toString());
    expect(parsedSchedule.expire?.toString()).toEqual(dailySchedule.expire?.toString());
    expect(parsedSchedule.repeat).toEqual(dailySchedule.repeat);
    expect(parsedSchedule.occurence).toEqual(dailySchedule.occurence);
    done();
  });

  it('parsing weeklySchedule form results in weeklySchedule', function(done) {
    // arrange
    let parsedSchedule: Schedule;
    onSave.pipe(take(1)).subscribe((value) => {
      parsedSchedule = value;
    });

    // act
    service.init(weeklySchedule, onSave, onCancel, onScheduleEditorReady, false);
    service.save();

    // assert
    expect(parsedSchedule.start.toString()).toEqual(weeklySchedule.start.toString());
    expect(parsedSchedule.expire?.toString()).toEqual(weeklySchedule.expire?.toString());
    expect(parsedSchedule.repeat).toEqual(weeklySchedule.repeat);
    expect(parsedSchedule.occurence).toEqual(weeklySchedule.occurence);
    done();
  });

  it('parsing monthlyDateSchedule form results in monthlyDateSchedule', function(done) {
    // arrange
    let parsedSchedule: Schedule;
    onSave.pipe(take(1)).subscribe((value) => {
      parsedSchedule = value;
    });

    // act
    service.init(monthlyDateSchedule, onSave, onCancel, onScheduleEditorReady, false);

    service.save();

    // assert
    expect(parsedSchedule.start.toString()).toEqual(monthlyDateSchedule.start.toString());
    expect(parsedSchedule.expire?.toString()).toEqual(monthlyDateSchedule.expire?.toString());
    expect(parsedSchedule.repeat).toEqual(monthlyDateSchedule.repeat);
    expect(parsedSchedule.occurence).toEqual(monthlyDateSchedule.occurence);
    done();
  });

  it('parsing monthlyOccurenceSchedule form results in monthlyOccurenceSchedule', function(done) {
    // arrange
    let parsedSchedule: Schedule;
    onSave.pipe(take(1)).subscribe((value) => {
      parsedSchedule = value;
    });

    // act
    service.init(monthlyOccurenceSchedule, onSave, onCancel, onScheduleEditorReady, false);

    service.save();

    // assert
    expect(parsedSchedule.start.toString()).toEqual(monthlyOccurenceSchedule.start.toString());
    expect(parsedSchedule.expire?.toString()).toEqual(monthlyOccurenceSchedule.expire?.toString());
    expect(parsedSchedule.repeat).toEqual(monthlyOccurenceSchedule.repeat);
    expect(parsedSchedule.occurence).toEqual(monthlyOccurenceSchedule.occurence);
    done();
  });

  it('switching form from monthly to weekly results in weeklySchedule', function(done) {
    // arrange
    let parsedSchedule: Schedule;
    onSave.pipe(take(1)).subscribe((value) => {
      parsedSchedule = value;
    });

    // act
    service.init(monthlyOccurenceSchedule, onSave, onCancel, onScheduleEditorReady, false);

    service.form.scheduleType.setValue(ScheduleTypeEnum[ScheduleTypeEnum.weekly]);
    service.form.weeklyDays.setValue(['Mon', 'Wed']);
    service.form.weeklyOccurence.setValue(5);

    service.save();

    const weeklyOccurence = new WeeklyOccurrence();
    weeklyOccurence.days = ['Mon', 'Wed'];
    weeklyOccurence.repeat = 5;

    // assert
    expect(parsedSchedule.start.toString()).toEqual(monthlyOccurenceSchedule.start.toString());
    expect(parsedSchedule.expire?.toString()).toEqual(monthlyOccurenceSchedule.expire?.toString());
    expect(parsedSchedule.repeat).toEqual(monthlyOccurenceSchedule.repeat);
    expect(parsedSchedule.occurence).toEqual(weeklyOccurence);
    done();
  });

  it('form conditional validation enabled', () => {
    // act
    service.init(dailySchedule, onSave, onCancel, onScheduleEditorReady, false);

    // assert
    expect(service.form.dailyOccurence.hasValidator(Validators.required)).toBeTrue();

    expect(service.form.weeklyOccurence.hasValidator(Validators.required)).toBeFalse();
    expect(service.form.weeklyDays.hasValidator(Validators.required)).toBeFalse();

    expect(service.form.months.hasValidator(Validators.required)).toBeFalse();
    expect(service.form.monthDays.hasValidator(Validators.required)).toBeFalse();
    expect(service.form.monthlyWeeks.hasValidator(Validators.required)).toBeFalse();
    expect(service.form.monthlyWeekDays.hasValidator(Validators.required)).toBeFalse();

    expect(service.form.expireDate.hasValidator(Validators.required)).toBeFalse();
    expect(service.form.expireDate.disabled).toBeTrue();
    expect(service.form.expireTime.hasValidator(Validators.required)).toBeFalse();
    expect(service.form.expireTime.disabled).toBeTrue();

    expect(service.form.repeatDuration.hasValidator(Validators.required)).toBeFalse();
    expect(service.form.repeatDuration.disabled).toBeTrue();
    expect(service.form.repeatEvery.hasValidator(Validators.required)).toBeFalse();
    expect(service.form.repeatEvery.disabled).toBeTrue();

    // enable expire
    service.form.isExpire.setValue(true);
    expect(service.form.expireDate.hasValidator(Validators.required)).toBeTrue();
    expect(service.form.expireDate.disabled).toBeFalse();
    expect(service.form.expireTime.hasValidator(Validators.required)).toBeTrue();
    expect(service.form.expireTime.disabled).toBeFalse();

    //set expireDate < startDate
    service.form.expireDate.setValue(new Date(Date.now() - 86400000));
    expect(service.form.expireDate.valid).toBeFalse();

    //set expireDate > startDate
    service.form.expireDate.setValue(new Date(Date.now() + 86400000));
    expect(service.form.expireDate.valid).toBeTrue();

    //set expireDate = startDate and expireTime > startTime
    service.form.expireDate.setValue(service.form.startDate.value);
    service.form.expireTime.setValue(new Date(service.form.startTime.value + 60000));
    expect(service.form.expireTime.valid).toBeTrue();

    //set expireDate = startDate and expireTime < startTime
    service.form.expireDate.setValue(service.form.startDate.value);
    service.form.expireTime.setValue(new Date(service.form.startTime.value - 60000));
    expect(service.form.expireTime.valid).toBeFalse();

    // enable repeat
    service.form.isRepeat.setValue(true);
    expect(service.form.repeatDuration.hasValidator(Validators.required)).toBeTrue();
    expect(service.form.repeatDuration.disabled).toBeFalse();
    expect(service.form.repeatEvery.hasValidator(Validators.required)).toBeTrue();
    expect(service.form.repeatEvery.disabled).toBeFalse();

    // switch to weekly
    service.form.scheduleType.setValue(ScheduleTypeEnum[ScheduleTypeEnum.weekly]);
    expect(service.form.dailyOccurence.hasValidator(Validators.required)).toBeFalse();
    expect(service.form.weeklyOccurence.hasValidator(Validators.required)).toBeTrue();
    expect(service.form.weeklyDays.hasValidator(Validators.required)).toBeTrue();

    // switch to monthly (date)
    service.form.scheduleType.setValue(ScheduleTypeEnum[ScheduleTypeEnum.monthly]);
    expect(service.form.dailyOccurence.hasValidator(Validators.required)).toBeFalse();
    expect(service.form.weeklyOccurence.hasValidator(Validators.required)).toBeFalse();
    expect(service.form.weeklyDays.hasValidator(Validators.required)).toBeFalse();

    expect(service.form.months.hasValidator(Validators.required)).toBeTrue();
    expect(service.form.monthDays.hasValidator(Validators.required)).toBeTrue();
    expect(service.form.monthDays.disabled).toBeFalse();
    expect(service.form.monthlyWeeks.hasValidator(Validators.required)).toBeFalse();
    expect(service.form.monthlyWeeks.disabled).toBeTrue();
    expect(service.form.monthlyWeekDays.hasValidator(Validators.required)).toBeFalse();
    expect(service.form.monthlyWeekDays.disabled).toBeTrue();

    // switch to monthly (offset)
    service.form.monthDaysType.setValue(MonthDaysTypeEnum[MonthDaysTypeEnum.offset]);
    expect(service.form.months.hasValidator(Validators.required)).toBeTrue();
    expect(service.form.monthDays.hasValidator(Validators.required)).toBeFalse();
    expect(service.form.monthDays.disabled).toBeTrue();
    expect(service.form.monthlyWeeks.hasValidator(Validators.required)).toBeTrue();
    expect(service.form.monthlyWeeks.disabled).toBeFalse();
    expect(service.form.monthlyWeekDays.hasValidator(Validators.required)).toBeTrue();
    expect(service.form.monthlyWeekDays.disabled).toBeFalse();
  });
});
