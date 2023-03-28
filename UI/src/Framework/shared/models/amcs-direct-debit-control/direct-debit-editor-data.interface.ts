import { BankLookup } from './bank-lookup.model';
import { DirectDebitAccountTypeLookup } from './direct-debit-account-type-lookup.model';
import { DirectDebitRunConfigurationLookup } from './direct-debit-run-configuration-lookup.model';

/**
 * @deprecated Move to PlatformUI
 */
export interface IDirectDebitEditorData {

    bankLookups: BankLookup[];

    directDebitRunConfigurationLookups: DirectDebitRunConfigurationLookup[];

    directDebitAccountTypeLookups: DirectDebitAccountTypeLookup[];
}
