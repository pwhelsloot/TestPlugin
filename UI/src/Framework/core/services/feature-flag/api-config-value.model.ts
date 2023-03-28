import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../../models/api/api-base.model';

@amcsJsonObject()
export class ApiConfigValue extends ApiBaseModel {
  @amcsJsonMember('ConfigId')
  configId: string;

  @amcsJsonMember('Value')
  configValue: string; //TODO: need support for multiple types, typedjson does not supportor any/object or multiple types..
}


