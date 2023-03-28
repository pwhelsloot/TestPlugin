import { ApiBaseModel, amcsJsonMember, amcsJsonObject } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class Ingredient extends ApiBaseModel {
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
}
