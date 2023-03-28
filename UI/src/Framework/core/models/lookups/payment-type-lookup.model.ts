import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { alias } from '../../config/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class PaymentTypeLookUp extends ApiBaseModel {

    @alias('PaymentTypeId')
    @amcsJsonMember('PaymentTypeId')
    id: number;

    @alias('Description')
    @amcsJsonMember('Description')
    description: string;

    @alias('IsCash')
    @amcsJsonMember('IsCash')
    isCash: boolean;

    @alias('IsCheque')
    @amcsJsonMember('IsCheque')
    isCheque: boolean;

    @alias('IsCard')
    @amcsJsonMember('IsCard')
    isCard: boolean;

    @alias('IsDirectDebit')
    @amcsJsonMember('IsDirectDebit')
    isDirectDebit: boolean;

    @alias('IsSystemPayment')
    @amcsJsonMember('IsSystemPayment')
    isSystemPayment: boolean;

    @alias('IsAutoPay')
    @amcsJsonMember('IsAutoPay')
    isAutoPay: boolean;

    @alias('IsElectronic')
    @amcsJsonMember('IsElectronic')
    isElectronic: boolean;

    @alias('IsServiceAgreementRestrictedPaymentType')
    @amcsJsonMember('IsServiceAgreementRestrictedPaymentType')
    isServiceAgreementRestrictedPaymentType: boolean;

    @alias('PaymentReferenceMandatory')
    @amcsJsonMember('PaymentReferenceMandatory')
    paymentReferenceMandatory: boolean;

    @alias('GUID')
    @amcsJsonMember('GUID')
    guid: string;
}
