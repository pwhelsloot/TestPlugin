import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { alias } from '@coreconfig/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class TaxBreakdown extends ApiBaseModel {

    @alias('Description')
    @amcsJsonMember('Description')
    description: string;

    @alias('Rate')
    @amcsJsonMember('Rate')
    rate: number;

    @alias('Amount')
    @amcsJsonMember('Amount')
    amount: number;

    @alias('TaxRateTypeId')
    @amcsJsonMember('TaxRateTypeId')
    taxRateTypeId: number;

    @alias('UnitOfMeasurement')
    @amcsJsonMember('UnitOfMeasurement')
    unitOfMeasurement: string;
}
