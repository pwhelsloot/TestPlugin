import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class TradingNameSearchResult extends ApiBaseModel {

    @amcsJsonMember('CustomerId')
    customerId: number;

    @amcsJsonMember('CustomerName')
    customerName: string;

    @amcsJsonMember('Description')
    description: string;

    @amcsJsonMember('IsForAllSites')
    isForAllSites: boolean;

    @amcsJsonMember('TradingNameTypeId')
    tradingNameTypeId: number;

    @amcsJsonMember('TradingNameType')
    tradingNameType: string;
}
