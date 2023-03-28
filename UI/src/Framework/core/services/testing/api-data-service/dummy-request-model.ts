import { amcsApiUrl, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
@amcsJsonObject()
@amcsApiUrl('someParent/testUrl')
export class DummyApiRequestModel extends ApiBaseModel {
  prop1: string;
  prop2: number;
  prop3: Date;
}
