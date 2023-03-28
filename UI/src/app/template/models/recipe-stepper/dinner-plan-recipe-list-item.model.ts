import { Recipe } from '@app/template/models/recipe.model';
import { DinnerPlanRecipeForm } from './dinner-plan-recipe-form.model';
import { DinnerCourseLookup } from './dinner-course-lookup.model';

export class DinnerPlanRecipeListItem {
  constructor(public form: DinnerPlanRecipeForm, public course: DinnerCourseLookup, public recipes: Recipe[]) {}
}
