import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class CoreUserPreference extends ApiBaseModel {

    @amcsJsonMember('key')
    key: string;

    @amcsJsonMember('value')
    value: string;
}
