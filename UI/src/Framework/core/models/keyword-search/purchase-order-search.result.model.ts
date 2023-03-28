import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class PurchaseOrderSearchResult extends ApiBaseModel {

    @amcsJsonMember('DemandPlanPurchaseOrderId')
    id: number;

    @amcsJsonMember('DemandPlanId')
    demandPlanId: number;

    @amcsJsonMember('PONumber')
    poNumber: string;

    @amcsJsonMember('StartDate')
    startDate: Date;

    @amcsJsonMember('EndDate')
    endDate: Date;

    @amcsJsonMember('IsAccepted')
    isAccepted: Boolean;

    purchaseOrderStatus: string;

    @amcsJsonMember('Materials')
    materials: string;

    @amcsJsonMember('ExpectedRevenue')
    expectedRevenue: number;

    @amcsJsonMember('CurrencyCode')
    currencyCode: string;
}
