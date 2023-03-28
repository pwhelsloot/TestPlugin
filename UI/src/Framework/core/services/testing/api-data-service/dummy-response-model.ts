import { amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class DummyApiResponseModel extends ApiBaseModel {
  prop4: string;
  prop5: number;
  prop6: Date;
}
