import { alias } from '@core-module/config/api-dto-alias.function';
import { amcsJsonArrayMember, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { BankLookup } from './bank-lookup.model';
import { DirectDebitAccountTypeLookup } from './direct-debit-account-type-lookup.model';
import { IDirectDebitEditorData } from './direct-debit-editor-data.interface';
import { DirectDebitRunConfigurationLookup } from './direct-debit-run-configuration-lookup.model';
import { DirectDebit } from './direct-debit.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class DirectDebitEditorData extends ApiBaseModel implements IDirectDebitEditorData {

    @amcsJsonArrayMember('BankLookups', BankLookup)
    @alias('BankLookups')
    bankLookups: BankLookup[];

    @amcsJsonArrayMember('DirectDebitAccountTypeLookups', DirectDebitAccountTypeLookup)
    @alias('DirectDebitAccountTypeLookups')
    directDebitAccountTypeLookups: DirectDebitAccountTypeLookup[];

    @amcsJsonArrayMember('DirectDebitRunConfigurationLookups', DirectDebitRunConfigurationLookup)
    @alias('DirectDebitRunConfigurationLookups')
    directDebitRunConfigurationLookups: DirectDebitRunConfigurationLookup[];

    @amcsJsonMember('DirectDebit')
    @alias('DirectDebit')
    directDebit: DirectDebit;
}
