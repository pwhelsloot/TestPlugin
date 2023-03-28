import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import * as moment from 'moment';
import { TypedJSON } from 'typedjson';
import { DailyOccurrence } from './daily-occurrence.model';
import { MonthlyOccurrence } from './month/monthly-occurrence.model';
import { ScheduleRepeat } from './schedule-repeat.model';
import { ScheduleTypeEnum } from './schedule-type.enum';
import { WeeklyOccurrence } from './weekly-occurrence.model';

@amcsJsonObject()
export class Schedule extends ApiBaseModel {
  /// <summary>
  /// Gets or sets the last date/time when schedule starts.
  /// </summary>
  @amcsJsonMember('start', false, { deserializer: dateDeserializer, serializer: dateSerializer })
  start: Date;

  /// <summary>
  /// Gets or sets the last date/time when schedule expires.
  /// </summary>
  @amcsJsonMember('expire', false, { deserializer: dateDeserializer, serializer: dateSerializer })
  expire: Date;

  /// <summary>
  /// Gets or sets the repetition schedule for a scheduled job.
  /// </summary>
  @amcsJsonMember('repeat')
  repeat: ScheduleRepeat;

  /// <summary>
  /// Gets or sets the occurence for a schedule.
  /// </summary>
  @amcsJsonMember('schedule', false, { deserializer: occurrenceDeserializer, serializer: occurrenceSerializer })
  occurence: DailyOccurrence | WeeklyOccurrence | MonthlyOccurrence;
}

function dateDeserializer(json: string): Date {
  if (json && json !== null) {
    return moment(json).toDate();
  } else {
    return null;
  }
}

function dateSerializer(value: Date): string {
  if (value && value !== null) {
    return moment(value).format('YYYY-MM-DDTHH:mm:ss.SSSSSSSZ');
  } else {
    return null;
  }
}

function occurrenceDeserializer(json: any) {
  switch (json['type']) {
    case ScheduleTypeEnum[ScheduleTypeEnum.daily]:
      return TypedJSON.parse(json, DailyOccurrence, { preserveNull: true });
    case ScheduleTypeEnum[ScheduleTypeEnum.weekly]:
      return TypedJSON.parse(json, WeeklyOccurrence, { preserveNull: true });
    case ScheduleTypeEnum[ScheduleTypeEnum.monthly]:
      return TypedJSON.parse(json, MonthlyOccurrence, { preserveNull: true });
    default:
      throw new Error('type not recognised');
  }
}
function occurrenceSerializer(value: DailyOccurrence | WeeklyOccurrence | MonthlyOccurrence) {
  switch (value.type) {
    case ScheduleTypeEnum[ScheduleTypeEnum.daily]:
      return TypedJSON.toPlainJson(value as DailyOccurrence, DailyOccurrence, { preserveNull: true });
    case ScheduleTypeEnum[ScheduleTypeEnum.weekly]:
      return TypedJSON.toPlainJson(value as WeeklyOccurrence, WeeklyOccurrence, { preserveNull: true });
    case ScheduleTypeEnum[ScheduleTypeEnum.monthly]:
      return TypedJSON.toPlainJson(value as MonthlyOccurrence, MonthlyOccurrence, { preserveNull: true });
    default:
      throw new Error('type not recognised');
  }
}
