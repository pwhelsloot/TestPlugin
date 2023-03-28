import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { ExternalPaymentRequestTypeEnum } from '../payments/external-payment-request-type.enum';
import { PriceTypeEnum } from './payment-request-type.enum';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class InitiateExternalPaymentRequest extends ApiBaseModel {

    @amcsJsonMember('CustomerId')
    customerId: number;

    @amcsJsonMember('Amount')
    amount: number;

    @amcsJsonMember('Currency')
    currency: string;

    @amcsJsonMember('CompanyOutletId')
    companyOutletId: number;

    @amcsJsonMember('PaymentTypeId')
    paymentTypeId: number;

    @amcsJsonMember('CreditCardVaultId')
    creditCardVaultId: number;

    @amcsJsonMember('Notes')
    notes: string;

    @amcsJsonMember('Reference')
    reference: string;

    @amcsJsonMember('RequestTypeId')
    requestType: ExternalPaymentRequestTypeEnum;

    @amcsJsonMember('Provider')
    paymentProviderId: number;

    @amcsJsonMember('OrderCombinationGroupId')
    orderCombinationGroupId: number;

    @amcsJsonMember('OrderCombinationGroupingTransactionId')
    orderCombinationGroupingTransactionId: number;

    @amcsJsonMember('CustomerSiteId')
    customerSiteId: number;

    @amcsJsonMember('ContactId')
    contactId: number;

    @amcsJsonMember('CanStoreProfile')
    canStoreProfile: boolean;

    @amcsJsonMember('CustomerIPAddress')
    customerIPAddress: string;

    @amcsJsonMember('CustomerBaseUrl')
    customerBaseUrl: string;

    @amcsJsonMember('PaymentRequestType')
    paymentRequestType: PriceTypeEnum;

    @amcsJsonMember('ApplyServiceFee')
    applyServiceFee: boolean;

    @amcsJsonMember('TermsAndConditionsId')
    termsAndConditionsId: number;

    @amcsJsonMember('TermsAndConditionsContentId')
    termsAndConditionsContentId: number;

    @amcsJsonMember('TermsAndConditionsUrl')
    termsAndConditionsUrl: string;

    @amcsJsonMember('ServiceFeeAmount')
    serviceFeeAmount: number;

    @amcsJsonMember('PrincipalAmount')
    principalAmount: number;

    @amcsJsonMember('Total')
    total: number;

    isDevMode: boolean;

    constructor() {
        super();
        this.paymentRequestType = PriceTypeEnum.PlatformUI;
    }
}
