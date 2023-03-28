import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class EntityHistoryChange extends ApiBaseModel {

    @amcsJsonMember('PropertyKey')
    propertyKey: string;

    @amcsJsonMember('OldValue')
    oldValue: string;

    @amcsJsonMember('NewValue')
    newValue: string;

    formattedPropertyKey: string;

}
