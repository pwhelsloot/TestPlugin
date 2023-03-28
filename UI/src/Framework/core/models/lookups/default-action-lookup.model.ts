import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class DefaultActionLookup extends ApiBaseModel {

    @amcsJsonMember('DefaultActionId')
    id: number;

    @amcsJsonMember('ServiceId')
    serviceId: number;

    @amcsJsonMember('MaterialClassId')
    materialClassId: number;

    @amcsJsonMember('ActionId')
    actionId: number;

    @amcsJsonMember('PricingBasisId')
    pricingBasisId: number;

    @amcsJsonMember('VATId')
    vatId: number;

    @amcsJsonMember('TaxTemplateCollectionId')
    taxTemplateCollectionId: number;

    @amcsJsonMember('UnitOfMeasurementId')
    unitOfMeasurementId: number;
}
