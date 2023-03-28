import { amcsJsonArrayMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { ExternalPaymentProviderLookup } from './external-payment-provider-lookup.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class ExternalPaymentProviderData extends ApiBaseModel {

    @amcsJsonArrayMember('ExternalPaymentProviderLookups', ExternalPaymentProviderLookup)
    paymentProviders: ExternalPaymentProviderLookup[];
}
