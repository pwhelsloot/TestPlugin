import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class InvoiceNumberSearchResult extends ApiBaseModel {

    @amcsJsonMember('InvoiceId')
    invoiceId: number;

    @amcsJsonMember('InvoiceTypeId')
    invoiceTypeId: number;

    @amcsJsonMember('CustomerId')
    customerId: number;

    @amcsJsonMember('CustomerName')
    customerName: string;

    @amcsJsonMember('ARAccountCode')
    aRAccountCode: string;

    @amcsJsonMember('InvoiceNo')
    invoiceNo: string;

    @amcsJsonMember('RemitSiteName')
    remitSiteName: string;

    @amcsJsonMember('InvoiceSiteAddress')
    invoiceSiteAddress: string;

    @amcsJsonMember('Amount')
    amount: number;

    @amcsJsonMember('CurrencyCode')
    currencyCode: string;
}
