import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class CurrencyConversionResponse extends ApiBaseModel {

    @amcsJsonMember('FromCurrencyId')
    fromCurrencyId: number;

    @amcsJsonMember('ToCurrencyId')
    toCurrencyId: number;

    @amcsJsonMember('DateOfConversion', true)
    dateOfConversion: Date;

    @amcsJsonMember('ExchangeRate')
    exchangeRate: number;

    @amcsJsonMember('OriginalAmount')
    originalAmount: number;

    @amcsJsonMember('ConvertedAmount')
    convertedAmount: number;
}
