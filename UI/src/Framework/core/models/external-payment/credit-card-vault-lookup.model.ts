import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class CreditCardVaultLookup extends ApiBaseModel implements ILookupItem {

    @amcsJsonMember('CreditCardVaultId')
    id: number;

    @amcsJsonMember('PaymentProviderId')
    paymentProviderId: number;

    @amcsJsonMember('MaskedCreditCardNumber')
    description: string;

    @amcsJsonMember('SupportsCardPayment')
    supportsCardPayment: boolean;

    @amcsJsonMember('SupportsOtherPayment')
    supportsOtherPayment: boolean;

    @amcsJsonMember('IsPrimary')
    isPrimary: boolean;
}
