import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from './api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class TaxCalculationRequest extends ApiBaseModel {

    @amcsJsonMember('NetAmount')
    netAmount: number;

    @amcsJsonMember('Rate')
    rate: number;

    @amcsJsonMember('Quantity')
    quantity: number;

    @amcsJsonMember('Weight')
    weight: number;

    @amcsJsonMember('TaxTemplateCollectionId')
    taxTemplateCollectionId: number;

    @amcsJsonMember('VatId')
    vatId: number;

    @amcsJsonMember('CustomerSiteId')
    customerSiteId: number;

    @amcsJsonMember('SiteOrderId')
    siteOrderId: number;

    @amcsJsonMember('DefaultActionId')
    defaultActionId: number;
}
