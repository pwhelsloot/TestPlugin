import { ApiBaseModel, amcsJsonMember, amcsJsonObject, amcsApiUrl } from '@core-module/models/api/api-base.model';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';

@amcsJsonObject()
@amcsApiUrl('recipe/IngredientTypes')
export class IngredientType extends ApiBaseModel implements ILookupItem {
  @amcsJsonMember('ApiIngredientTypeId')
  id: number;

  @amcsJsonMember('Description')
  description: string;
}
