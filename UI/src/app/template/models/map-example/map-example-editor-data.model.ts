import { MapExample } from './map-example.model';
import { amcsJsonObject, ApiBaseModel, amcsJsonMember, amcsApiUrl } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
@amcsApiUrl('recipe/MapExampleEditorDatas')
export class MapExampleEditorData extends ApiBaseModel {
  @amcsJsonMember('MapExampleId')
  mapExampleId: number;

  @amcsJsonMember('MapExample')
  mapExample: MapExample;
}
