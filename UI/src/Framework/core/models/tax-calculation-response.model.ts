import { amcsJsonArrayMember, amcsJsonMember, amcsJsonObject, ApiBaseModel } from './api/api-base.model';
import { TaxBreakdown } from './tax-breakdown.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class TaxCalculationResponse extends ApiBaseModel {

    @amcsJsonMember('Rate')
    rate: number;

    @amcsJsonMember('Quantity')
    quantity: number;

    @amcsJsonMember('Weight')
    weight: number;

    @amcsJsonMember('GrossAmount')
    grossAmount: number;

    @amcsJsonMember('NetAmount')
    netAmount: number;

    @amcsJsonMember('VATAmount')
    vatAmount: number;

    @amcsJsonArrayMember('Taxes', TaxBreakdown)
    taxes: TaxBreakdown[];
}
