import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { ExternalPaymentTransactionStatusEnum } from './external-payment-transaction-status.enum';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class ExternalPaymentTransaction extends ApiBaseModel {

    @amcsJsonMember('PaymentTransactionGUID')
    transactionGuid: string;

    @amcsJsonMember('TransactionStatusId')
    status: ExternalPaymentTransactionStatusEnum;

    @amcsJsonMember('CardType')
    cardType: string;

    @amcsJsonMember('MaskedCardNumber')
    maskedCardNumber: string;

    @amcsJsonMember('PaymentProvider')
    paymentProvider: string;

    @amcsJsonMember('Amount')
    amount: number;

    @amcsJsonMember('PaymentReference')
    paymentReference: string;

    @amcsJsonMember('PaymentCurrency')
    currency: string;

    @amcsJsonMember('PaymentId')
    paymentId: number;

    @amcsJsonMember('CustomerId')
    customerId: number;

    @amcsJsonMember('ServiceFeePaymentId')
    serviceFeePaymentId: number;
}
