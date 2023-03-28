import { amcsJsonArrayMember, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { ScheduleTypeEnum } from '@shared-module/models/amcs-scheduler/schedule-type.enum';

@amcsJsonObject()
export class WeeklyOccurrence extends ApiBaseModel {
  /// <summary>
  /// Gets or sets the interval in weeks at which this schedule repeats.
  /// </summary>
  @amcsJsonMember('repeat')
  repeat: number;

  @amcsJsonArrayMember('days', String)
  days: string[];

  @amcsJsonMember('type')
  type: string = ScheduleTypeEnum[ScheduleTypeEnum.weekly];
}
