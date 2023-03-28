import { alias } from '@coreconfig/api-dto-alias.function';
import { AddressValidationSearchResult } from './address-validation-search-result.model';

/**
 * @deprecated Move to PlatformUI
 */
export class AddressValidationSearch {
    @alias('SearchString')
    searchString: string;

    @alias('SearchResults')
    searchResults: AddressValidationSearchResult[];
}
