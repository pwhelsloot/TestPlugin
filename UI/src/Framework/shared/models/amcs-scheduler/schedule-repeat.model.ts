import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class ScheduleRepeat extends ApiBaseModel {
  /// <summary>
  /// Gets or sets the total number of seconds a schedule must repeat.
  /// </summary>
  @amcsJsonMember('duration')
  duration: number;

  /// <summary>
  /// Gets or sets the interval in seconds the schedule must execute at.
  /// </summary>
  @amcsJsonMember('interval')
  interval: number;
}
