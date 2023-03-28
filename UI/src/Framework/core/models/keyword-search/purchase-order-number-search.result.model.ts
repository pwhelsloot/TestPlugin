import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class PurchaseOrderNumberSearchResult extends ApiBaseModel {

    @amcsJsonMember('PONumberId')
    id: number;

    @amcsJsonMember('PONumberChainId')
    purchaseOrderNumberChainId: number;

    @amcsJsonMember('CustomerId')
    customerId: number;

    @amcsJsonMember('SiteOrderId')
    siteOrderId: number;

    @amcsJsonMember('CustomerSiteOrder')
    customerSiteOrder: string;

    @amcsJsonMember('CustomerName')
    customerName: string;

    @amcsJsonMember('PONumber')
    poNumber: string;

    @amcsJsonMember('StartDate')
    startDate: Date;

    @amcsJsonMember('EndDate')
    endDate: Date;
}
