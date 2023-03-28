import { amcsApiUrl, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsApiUrl('configuration/dynamicAppConfiguration')
@amcsJsonObject()
export class ApiConfigRequest extends ApiBaseModel{

}
