import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { ApiBaseModel, amcsJsonMember, amcsJsonObject, amcsJsonArrayMember, amcsApiUrl } from '@core-module/models/api/api-base.model';
import { Ingredient } from './ingredient.model';

@amcsJsonObject()
@amcsApiUrl('recipe/recipes')
export class Recipe extends ApiBaseModel implements ILookupItem {
  @amcsJsonMember('RecipeId')
  id: number;

  @amcsJsonMember('Name')
  description: string;

  @amcsJsonMember('Method')
  method: string;

  @amcsJsonMember('DinnerCourseId')
  dinnerCourseId: number;

  @amcsJsonArrayMember('Ingredients', Ingredient)
  ingredients: Ingredient[];
}
