import { ApiBaseModel, amcsJsonMember, amcsJsonObject, amcsApiUrl } from '@core-module/models/api/api-base.model';
import { GridColumnConfig } from '@shared-module/components/amcs-grid/grid-column-config';
import { GridColumnType } from '@shared-module/components/amcs-grid/grid-column-type.enum';
import { isTruthy } from '@core-module/helpers/is-truthy.function';

@amcsJsonObject()
@amcsApiUrl('recipe/Ingredients')
export class Ingredient extends ApiBaseModel {
  @amcsJsonMember('IngredientId')
  ingredientId: number;

  @amcsJsonMember('Name')
  name: string;

  @amcsJsonMember('Amount')
  amount: number;

  @amcsJsonMember('MeasurementId')
  measurementId: number;

  @amcsJsonMember('Measurement')
  measurement: string;

  @amcsJsonMember('Type')
  type: string;

  @amcsJsonMember('TypeId')
  typeId: number;

  @amcsJsonMember('Optional')
  optional: boolean;

  filter(text: string): boolean {
    return this.contains(this.name, text) || this.contains(this.measurement, text) || this.contains(this.type, text);
  }

  static getGridColumns(translations: string[]): GridColumnConfig[] {
    return [
      new GridColumnConfig(translations['template.ingredientBrowser.ingredient.name'], 'name', [20]).withType(GridColumnType.truncatedText),
      new GridColumnConfig(translations['template.ingredientBrowser.ingredient.measurement'], 'measurement', [20]).withType(
        GridColumnType.truncatedText
      ),
      new GridColumnConfig(translations['template.ingredientBrowser.ingredient.type'], 'type', [20]).withType(GridColumnType.truncatedText),
      new GridColumnConfig(translations['template.ingredientBrowser.ingredient.optional'], 'optional', [20]).withType(
        GridColumnType.boolean
      ),
    ];
  }

  private contains(propertyValue: string, text: string) {
    return isTruthy(propertyValue) && propertyValue.toLowerCase().includes(text);
  }
}
