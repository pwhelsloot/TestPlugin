import { MonthDaysTypeEnum } from './month-days-type.enum';
import { amcsJsonArrayMember, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class MonthDays extends ApiBaseModel {
  @amcsJsonArrayMember('days', String)
  days: string[];

  @amcsJsonMember('type')
  type: string = MonthDaysTypeEnum[MonthDaysTypeEnum.days];
}
