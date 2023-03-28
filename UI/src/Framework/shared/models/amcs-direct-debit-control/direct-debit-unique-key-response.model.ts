import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class DirectDebitUniqueKeyResponse extends ApiBaseModel {

    @amcsJsonMember('UniqueKey')
    uniqueKey: string;

    @amcsJsonMember('NationalCheckDigits')
    nationalCheckDigits: string;
}
