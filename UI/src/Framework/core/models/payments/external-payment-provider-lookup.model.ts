import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ILookupItem } from '../lookups/lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class ExternalPaymentProviderLookup extends ApiBaseModel implements ILookupItem {

  @amcsJsonMember('PaymentProviderId')
  id: number;

  @amcsJsonMember('Description')
  description: string;

  @amcsJsonMember('SupportsCardAuthentication')
  supportsCardAuthentication: boolean;

}
