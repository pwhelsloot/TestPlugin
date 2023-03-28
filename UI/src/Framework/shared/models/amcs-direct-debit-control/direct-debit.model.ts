import { postAlias } from '@core-module/config/alias-to-api.function';
import { alias } from '@core-module/config/api-dto-alias.function';
import { amcsJsonArrayMember, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { IPropertiesMetadata } from '../properties-metadata.interface';
import { PropertyMetadata } from '../property-metadata.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class DirectDebit extends ApiBaseModel implements IPropertiesMetadata {

    @amcsJsonMember('DirectDebitId')
    @alias('DirectDebitId')
    @postAlias('DirectDebitId')
    directDebitId: number;

    @amcsJsonMember('ValidationContextId')
    @alias('ValidationContextId')
    @postAlias('ValidationContextId')
    validationContextId: number;

    @amcsJsonMember('AccountName')
    @alias('AccountName')
    @postAlias('AccountName')
    accountName: string;

    @amcsJsonMember('AuthorisedSignatory')
    @alias('AuthorisedSignatory')
    @postAlias('AuthorisedSignatory')
    authorisedSignatory: string;

    @amcsJsonMember('BankId')
    @alias('BankId')
    @postAlias('BankId')
    bankId: number;

    @amcsJsonMember('BankName')
    @alias('BankName')
    @postAlias('BankName')
    bankName: string;

    @amcsJsonMember('AccountNo')
    @alias('AccountNo')
    @postAlias('AccountNo')
    accountNo: string;

    @amcsJsonMember('SortCode')
    @alias('SortCode')
    @postAlias('SortCode')
    sortCode: string;

    @amcsJsonMember('BIC')
    @alias('BIC')
    @postAlias('BIC')
    bic: string;

    @amcsJsonMember('IBAN')
    @alias('IBAN')
    @postAlias('IBAN')
    iban: string;

    @amcsJsonMember('UMR')
    @alias('UMR')
    @postAlias('UMR')
    umr: string;

    @amcsJsonMember('NationalBankCode')
    @alias('NationalBankCode')
    @postAlias('NationalBankCode')
    nationalBankCode: string;

    @amcsJsonMember('NationalCheckDigits')
    @alias('NationalCheckDigits')
    @postAlias('NationalCheckDigits')
    nationalCheckDigits: string;

    @amcsJsonMember('RIBNumber')
    @alias('RIBNumber')
    @postAlias('RIBNumber')
    ribNumber: string;

    @amcsJsonMember('BranchCode')
    @alias('BranchCode')
    @postAlias('BranchCode')
    branchCode: string;

    @amcsJsonMember('DateAuthorised')
    @alias('DateAuthorised')
    @postAlias('DateAuthorised')
    dateAuthorised: Date;

    @amcsJsonMember('DirectDebitRunConfigurationId')
    @alias('DirectDebitRunConfigurationId')
    @postAlias('DirectDebitRunConfigurationId')
    directDebitRunConfigurationId: number;

    @amcsJsonMember('Address1')
    @alias('Address1')
    @postAlias('Address1')
    address1: string;

    @amcsJsonMember('Address2')
    @alias('Address2')
    @postAlias('Address2')
    address2: string;

    @amcsJsonMember('Address3')
    @alias('Address3')
    @postAlias('Address3')
    address3: string;

    @amcsJsonMember('Address4')
    @alias('Address4')
    @postAlias('Address4')
    address4: string;

    @amcsJsonMember('Address5')
    @alias('Address5')
    @postAlias('Address5')
    address5: string;

    @amcsJsonMember('Postcode')
    @alias('Postcode')
    @postAlias('Postcode')
    postcode: string;

    @amcsJsonMember('IsVerified')
    @alias('IsVerified')
    @postAlias('IsVerified')
    isVerified: boolean;

    @amcsJsonMember('IsProcessed')
    @alias('IsProcessed')
    @postAlias('IsProcessed')
    isProcessed: boolean;

    @amcsJsonMember('DirectDebitAccountTypeId')
    @alias('DirectDebitAccountTypeId')
    @postAlias('DirectDebitAccountTypeId')
    directDebitAccountTypeId: number;

    @amcsJsonArrayMember('PropertiesMetadata', PropertyMetadata)
    @alias('PropertiesMetadata')
    propertiesMetadata: PropertyMetadata[];

    @amcsJsonMember('UMRPrefix')
    @alias('UMRPrefix')
    umrPrefix: string;

    @amcsJsonMember('FirstDateForPaymentRequest')
    @alias('FirstDateForPaymentRequest')
    @postAlias('FirstDateForPaymentRequest')
    firstDateForPaymentRequest: Date;
}
