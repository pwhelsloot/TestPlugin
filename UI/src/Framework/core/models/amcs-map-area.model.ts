import { amcsJsonArrayMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { AmcsMapPosition } from '@core-module/models/amcs-map-position-model';

@amcsJsonObject()
export class AmcsMapArea extends ApiBaseModel {
  @amcsJsonArrayMember('Positions', AmcsMapPosition)
  positions: AmcsMapPosition[];
}
