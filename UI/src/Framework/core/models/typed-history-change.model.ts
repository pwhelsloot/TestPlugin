import { alias } from '../config/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from './api/api-base.model';

@amcsJsonObject()
export class TypedHistoryChange extends ApiBaseModel {
    @alias('PropertyKey')
    @amcsJsonMember('PropertyKey')
    propertyKey: string;

    @alias('OldValue')
    @amcsJsonMember('OldValue')
    oldValue: string;

    @alias('NewValue')
    @amcsJsonMember('NewValue')
    newValue: string;

    formattedPropertyKey: string;
}
