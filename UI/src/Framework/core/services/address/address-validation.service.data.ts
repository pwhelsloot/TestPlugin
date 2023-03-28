import { Injectable } from '@angular/core';
import { ClassBuilder } from '@core-module/helpers/dto/class-builder';
import { BrregAddressSearchResult } from '@core-module/models/address/brreg-address-search-result.model';
import { BrregSearchResult } from '@core-module/models/address/brreg-search-result.model';
import { AddressValidationSearchResult } from '@core-module/models/external-dependencies/address-validation-search-result.model';
import { AddressValidationSearch } from '@core-module/models/external-dependencies/address-validation-search.model';
import { ApiResourceResponse } from '@coreconfig/api-resource-response.interface';
import { ApiOptionsEnum } from '@coremodels/api/api-options.enum';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { FilterOperation } from '@coremodels/api/filters/filter-operation.enum';
import { IFilter } from '@coremodels/api/filters/iFilter';
import { ErpApiService } from '@coreservices/erp-api.service';

/**
 * @deprecated Move to PlatformUI + ScaleUI (or new module) https://dev.azure.com/amcsgroup/Platform/_workitems/edit/188268
 */
@Injectable()
export class AddressValidationServiceData {

    constructor(private erpApiService: ErpApiService) {
    }

    getBrregAddressSearch(brregNumber: string, brregTypeEntry: number) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['BregAddressSearches'];
        apiRequest.filters = [
            {
                filterOperation: FilterOperation.Equal,
                name: 'BregNumber',
                value: brregNumber
            },
            {
                filterOperation: FilterOperation.Equal,
                name: 'BregTypeEntry',
                value: brregTypeEntry
            }
        ];

        return this.erpApiService.getRequestHandleError<ApiResourceResponse>(apiRequest, this.getBrregAddressSearchMap, false, () => null);
    }

    getAddressSearch(searchText: string, countryId: number) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['AddressSearches'];
        apiRequest.apiOptions = ApiOptionsEnum.core;

        apiRequest.filters = [];

        const searchFilter: IFilter = {
            filterOperation: FilterOperation.Equal,
            name: 'SearchString',
            // eslint-disable-next-line
            'value': searchText,

        };
        apiRequest.filters.push(searchFilter);

        const countryFilter: IFilter = {
            filterOperation: FilterOperation.Equal,
            name: 'CountryId',
            // eslint-disable-next-line
            'value': countryId
        };
        apiRequest.filters.push(countryFilter);

        return this.erpApiService.getRequest<ApiResourceResponse, AddressValidationSearch>(apiRequest, this.getAddressSearchMap, AddressValidationSearch);
    }

    getAddressReverseSearch(latitude: number, longitude: number, countryId: number) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['AddressReverseSearches'];
        apiRequest.apiOptions = ApiOptionsEnum.core;

        apiRequest.filters = [];

        const latitudeFilter: IFilter = {
            filterOperation: FilterOperation.Equal,
            name: 'Latitude',
            // eslint-disable-next-line
            'value': latitude
        };
        apiRequest.filters.push(latitudeFilter);

        const longitudeFilter: IFilter = {
            filterOperation: FilterOperation.Equal,
            name: 'Longitude',
            value: longitude
        };
        apiRequest.filters.push(longitudeFilter);

        const countryFilter: IFilter = {
            filterOperation: FilterOperation.Equal,
            name: 'CountryId',
            // eslint-disable-next-line
            'value': countryId
        };
        apiRequest.filters.push(countryFilter);

        return this.erpApiService.getRequest<ApiResourceResponse, AddressValidationSearch>(apiRequest, this.getAddressReverseSearchMap, AddressValidationSearch);
    }

    getAddress(href: string, countryId: number) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['AddressSearchResults'];

        apiRequest.filters = [
            {
                filterOperation: FilterOperation.Equal,
                name: 'Href',
                value: href,
            },
            {
                filterOperation: FilterOperation.Equal,
                name: 'CountryId',
                value: countryId,
            }
        ];

        return this.erpApiService.getRequest<ApiResourceResponse, AddressValidationSearchResult>(apiRequest, this.getAddressMap, AddressValidationSearchResult);
    }

    private getBrregAddressSearchMap(response: ApiResourceResponse) {
        const payload = response.resource[0];
        const result: BrregSearchResult = ClassBuilder.buildFromApiResourceResponse<BrregSearchResult>(payload, BrregSearchResult);
        result.searchResults = ClassBuilder.buildFromApiResourceResults<BrregAddressSearchResult>(payload['SearchResults'], BrregAddressSearchResult);
        result.searchResults.forEach(searchResult => {
            // DB constraint of 50 characters
            searchResult.address1.substring(0, 50);
        });
        return result;
    }

    private getAddressSearchMap(response: ApiResourceResponse) {
        const result: AddressValidationSearch = ClassBuilder.buildFromApiResourceResponse<AddressValidationSearch>(
            response.resource[0],
            AddressValidationSearch);

        result.searchResults = ClassBuilder.buildFromApiResourceResults<AddressValidationSearchResult>(
            response.resource[0]['SearchResults'],
            AddressValidationSearchResult);
        return result;
    }

    private getAddressReverseSearchMap(response: ApiResourceResponse) {
        const result: AddressValidationSearch = ClassBuilder.buildFromApiResourceResponse<AddressValidationSearch>(
            response.resource[0],
            AddressValidationSearch);

        const resultset = ClassBuilder.buildFromApiResourceResponse<AddressValidationSearchResult>(
            response.resource[0]['SearchResult'],
            AddressValidationSearchResult);

        resultset.latitude = response.resource[0]['Latitude'];
        resultset.longitude = response.resource[0]['Longitude'];

        result.searchResults = [];
        result.searchResults.push(resultset);
        return result;
    }

    private getAddressMap(response: ApiResourceResponse) {
        return ClassBuilder.buildFromApiResourceResponse<AddressValidationSearchResult>(response.resource[0], AddressValidationSearchResult);
    }
}
