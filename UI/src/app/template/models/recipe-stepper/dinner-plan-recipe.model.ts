import { amcsJsonObject, ApiBaseModel, amcsJsonMember } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class DinnerPlanRecipe extends ApiBaseModel {
  @amcsJsonMember('DinnerPlanId')
  dinnerPlanId: number;

  @amcsJsonMember('DinnerCourseId')
  courseId: number;

  @amcsJsonMember('RecipeId')
  recipeId: number;

  @amcsJsonMember('PreparationDate', true)
  preparationDate: Date;
}
