import { amcsJsonObject, ApiBaseModel, amcsJsonMember, amcsJsonArrayMember, amcsApiUrl } from '@core-module/models/api/api-base.model';
import { DinnerPlan } from './dinner-plan.model';
import { DinnerCourseLookup } from './dinner-course-lookup.model';
import { DinnerPlanDifficultyLevelLookup } from './dinner-plan-difficulty-level-lookup.model';

@amcsJsonObject()
@amcsApiUrl('recipe/dinnerPlanEditorDatas')
export class DinnerPlanEditorData extends ApiBaseModel {
  @amcsJsonMember('DataModel')
  dataModel: DinnerPlan;

  @amcsJsonArrayMember('Courses', DinnerCourseLookup)
  dinnerCourses: DinnerCourseLookup[];

  @amcsJsonArrayMember('DifficultyLevels', DinnerPlanDifficultyLevelLookup)
  difficultyLevels: DinnerPlanDifficultyLevelLookup[];
}
