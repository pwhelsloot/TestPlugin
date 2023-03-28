import { ApiBaseModel, amcsJsonMember, amcsJsonObject, amcsApiUrl } from '@core-module/models/api/api-base.model';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';

@amcsJsonObject()
@amcsApiUrl('recipe/Measurements')
export class Measurement extends ApiBaseModel implements ILookupItem {
  @amcsJsonMember('ApiMeasurementId')
  id: number;

  @amcsJsonMember('Description')
  description: string;
}
