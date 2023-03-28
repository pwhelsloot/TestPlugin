import { DinnerPlan } from './dinner-plan.model';
import { amcsJsonObject, ApiBaseModel, amcsJsonMember, amcsJsonArrayMember, amcsApiUrl } from '@core-module/models/api/api-base.model';
import { DinnerPlanRecipe } from './dinner-plan-recipe.model';

@amcsJsonObject()
@amcsApiUrl('recipe/recipeStepperSaves')
export class RecipeStepperSave extends ApiBaseModel {
  @amcsJsonMember('DinnerPlan')
  dinnerPlan: DinnerPlan;

  @amcsJsonArrayMember('DinnerPlanRecipes', DinnerPlanRecipe)
  dinnerPlanRecipes: DinnerPlanRecipe[];
}
