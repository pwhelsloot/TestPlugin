import { amcsJsonArrayMember, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { TypedJSON } from 'typedjson';
import { ScheduleTypeEnum } from '../schedule-type.enum';
import { MonthDaysTypeEnum } from './month-days-type.enum';
import { MonthDays } from './month-days.model';
import { MonthOffset } from './month-offset.model';

@amcsJsonObject()
export class MonthlyOccurrence extends ApiBaseModel {
  @amcsJsonMember('days', false, { deserializer: monthDayDeserializer, serializer: monthDaySerializer })
  days: MonthDays | MonthOffset;

  @amcsJsonArrayMember('months', String)
  months: string[];

  @amcsJsonMember('type')
  type: string = ScheduleTypeEnum[ScheduleTypeEnum.monthly];
}
function monthDayDeserializer(json: any) {
  switch (json['type']) {
    case MonthDaysTypeEnum[MonthDaysTypeEnum.days]:
      return TypedJSON.parse(json, MonthDays, { preserveNull: true });
    case MonthDaysTypeEnum[MonthDaysTypeEnum.offset]:
      return TypedJSON.parse(json, MonthOffset, { preserveNull: true });
    default:
      throw new Error('type not recognised');
  }
}
function monthDaySerializer(value: MonthDays | MonthOffset) {
  switch (value.type) {
    case MonthDaysTypeEnum[MonthDaysTypeEnum.days]:
      return TypedJSON.toPlainJson(value as MonthDays, MonthDays, { preserveNull: true });
    case MonthDaysTypeEnum[MonthDaysTypeEnum.offset]:
      return TypedJSON.toPlainJson(value as MonthOffset, MonthOffset, { preserveNull: true });
    default:
      throw new Error('type not recognised');
  }
}
