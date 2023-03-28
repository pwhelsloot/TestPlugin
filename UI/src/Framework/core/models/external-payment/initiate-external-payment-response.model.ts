import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class InitiateExternalPaymentResponse extends ApiBaseModel {

    @amcsJsonMember('PaymentUrl')
    paymentURL: string;

    @amcsJsonMember('PaymentTransactionGUID')
    transactionGuid: string;
}
