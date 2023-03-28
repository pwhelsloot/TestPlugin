import { MonthDaysTypeEnum } from './month-days-type.enum';
import { amcsJsonArrayMember, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class MonthOffset extends ApiBaseModel {
  @amcsJsonArrayMember('days', String)
  days: string[];

  @amcsJsonArrayMember('week', String)
  week: string[];

  @amcsJsonMember('type')
  type: string = MonthDaysTypeEnum[MonthDaysTypeEnum.offset];
}
