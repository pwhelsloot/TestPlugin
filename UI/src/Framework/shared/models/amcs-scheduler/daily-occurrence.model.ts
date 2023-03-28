import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { ScheduleTypeEnum } from '@shared-module/models/amcs-scheduler/schedule-type.enum';

@amcsJsonObject()
export class DailyOccurrence extends ApiBaseModel {
  /// <summary>
  /// Gets or sets the interval in days at which this schedule repeats.
  /// </summary>
  @amcsJsonMember('repeat')
  repeat: number;

  @amcsJsonMember('type')
  type: string = ScheduleTypeEnum[ScheduleTypeEnum.daily];
}
