import { alias } from '@core-module/config/api-dto-alias.function';
import { BrregAddressSearchResult } from './brreg-address-search-result.model';

/**
 * @deprecated Move to PlatformUI + ScaleUI (or new module) https://dev.azure.com/amcsgroup/Platform/_workitems/edit/188268
 */
export class BrregSearchResult {
    @alias('BregNumber')
    bregNumber: string;

    @alias('BregType')
    bregType: string;

    @alias('CountryId')
    countryId: number;

    @alias('SearchResults')
    searchResults: BrregAddressSearchResult[];
}
