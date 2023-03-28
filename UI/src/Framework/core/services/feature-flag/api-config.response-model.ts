import { amcsJsonArrayMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { ApiConfigValue } from './api-config-value.model';
@amcsJsonObject()
export class ApiConfigResponse extends ApiBaseModel {
  @amcsJsonArrayMember('ConfigurationValues', ApiConfigValue)
  configurationValues: ApiConfigValue[];
}
