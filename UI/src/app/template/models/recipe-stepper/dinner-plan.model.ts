import { amcsJsonObject, ApiBaseModel, amcsJsonMember, amcsJsonArrayMember } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class DinnerPlan extends ApiBaseModel {
  @amcsJsonMember('DinnerPlanId')
  id: number;

  @amcsJsonMember('Name')
  name: string;

  @amcsJsonMember('EstimatedTime', true)
  estimatedTime: Date;

  @amcsJsonMember('DifficultyLevelId')
  difficultyLevelId: number;

  @amcsJsonArrayMember('CourseIds', Number)
  courseIds: number[];
}
