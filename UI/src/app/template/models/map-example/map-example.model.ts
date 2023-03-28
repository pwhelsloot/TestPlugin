import { amcsJsonObject, ApiBaseModel, amcsJsonMember, amcsApiUrl } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
@amcsApiUrl('recipe/MapExamples')
export class MapExample extends ApiBaseModel {
  @amcsJsonMember('MapExampleId')
  mapExampleId: number;

  @amcsJsonMember('Description')
  description: string;

  @amcsJsonMember('Latitude')
  latitude: number;

  @amcsJsonMember('Longitude')
  longitude: number;
}
