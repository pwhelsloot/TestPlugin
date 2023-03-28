
import { alias } from '@coreconfig/api-dto-alias.function';
import { AddressFieldDisplayConfiguration } from '../address/address-field-display-configuration.model';
import { ContractSearchResult } from './contract-search-result.model';
import { CustomerSearchResult } from './customer-search-result.model';
import { MunicipalOfferingsSearchResult } from './municipal-offerings-search-result.model';
import { ServiceLocationSearchResult } from './service-location-search-result.model';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/247298
 */
export class GlobalSearch {

    @alias('SearchTerm')
    searchTerm: string;

    @alias('GlobalMunicipalOfferingsSearch')
    municipalOfferingsSearch: MunicipalOfferingsSearchResult[];

    @alias('MunicipalOfferingsSearchCount')
    municipalOfferingsSearchCount: number;

    @alias('GlobalCustomerSearch')
    customerSearch: CustomerSearchResult[];

    @alias('CustomerSearchCount')
    customerSearchCount: number;

    @alias('serviceLocationSearch')
    serviceLocationSearch: ServiceLocationSearchResult[];

    @alias('ServiceLocationSearchCount')
    serviceLocationSearchCount: number;

    @alias('contractSearch')
    contractSearch: ContractSearchResult[];

    @alias('ContractSearchCount')
    contractSearchCount: number;

    @alias('AddressFieldDisplayConfigurations')
    addressFieldDisplayConfigurations: AddressFieldDisplayConfiguration[];
}
