import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class FakeApiModel extends ApiBaseModel {

  @amcsJsonMember('Id')
  id: number;

  @amcsJsonMember('Description')
  description: string;
}
