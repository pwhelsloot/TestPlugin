import { amcsJsonMember, ApiBaseModel } from '@core-module/models/api/api-base.model';

export class ComponentFilter extends ApiBaseModel {

  @amcsJsonMember('filterId')
  filterId: number;

  @amcsJsonMember('propertyId')
  propertyId: number;

  @amcsJsonMember('property')
  property: string;

  @amcsJsonMember('propertyValueType')
  propertyValueType: number;

  @amcsJsonMember('propertyKey')
  propertyKey: string;

  @amcsJsonMember('filterTypeId')
  filterTypeId: number;

  @amcsJsonMember('filterType')
  filterType: string;

  @amcsJsonMember('filterTextValue')
  filterTextValue: string;

  @amcsJsonMember('filterNumberValue')
  filterNumberValue: number;

  @amcsJsonMember('filterDateValue', true)
  filterDateValue: Date;

  @amcsJsonMember('filterBooleanValue')
  filterBooleanValue: number;

  filterEnumValue: number;

  filterEnumDescriptionValue: string;
}
