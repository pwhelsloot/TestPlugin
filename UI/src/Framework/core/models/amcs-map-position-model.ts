import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class AmcsMapPosition extends ApiBaseModel {
  @amcsJsonMember('Long')
  long: number;

  @amcsJsonMember('Lat')
  lat: number;
}
