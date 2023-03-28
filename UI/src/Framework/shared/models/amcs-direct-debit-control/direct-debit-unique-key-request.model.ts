import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class DirectDebitUniqueKeyRequest extends ApiBaseModel {

    @amcsJsonMember('UniqueKey')
    uniqueKey: string;

    @amcsJsonMember('Type')
    type: string;

    @amcsJsonMember('AccountNo')
    accountNo: string;

    @amcsJsonMember('SortCode')
    sortCode: string;

    @amcsJsonMember('NationalBankCode')
    nationalBankCode: string;

    @amcsJsonMember('NationalCheckDigits')
    nationalCheckDigits: string;
}
