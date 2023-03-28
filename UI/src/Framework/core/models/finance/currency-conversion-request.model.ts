import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class CurrencyConversionRequest extends ApiBaseModel {

    @amcsJsonMember('FromCurrencyId')
    fromCurrencyId: number;

    @amcsJsonMember('ToCurrencyId')
    toCurrencyId: number;

    @amcsJsonMember('DateOfConversion', true)
    dateOfConversion: Date;

    @amcsJsonMember('Amount')
    amount: number;

    isValid(): boolean {
        return isTruthy(this.fromCurrencyId) && isTruthy(this.toCurrencyId) && isTruthy(this.dateOfConversion) && isTruthy(this.amount);
    }
}
