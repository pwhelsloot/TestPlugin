import { Injectable } from '@angular/core';
import { ClassBuilder } from '@core-module/helpers/dto/class-builder';
import { AddressFieldDisplayConfiguration } from '@core-module/models/address/address-field-display-configuration.model';
import { FilterOperation } from '@core-module/models/api/filters/filter-operation.enum';
import { ContractSearchOrderResult } from '@core-module/models/external-dependencies/contract-order-search-result.model';
import { ContractSearchResult } from '@core-module/models/external-dependencies/contract-search-result.model';
import { CustomerSearchResult } from '@core-module/models/external-dependencies/customer-search-result.model';
import { GlobalSearch } from '@core-module/models/external-dependencies/global-search-model';
import { MunicipalOfferingsSearchResult } from '@core-module/models/external-dependencies/municipal-offerings-search-result.model';
import { SeviceLocationSearchOrderResult } from '@core-module/models/external-dependencies/service-location-search-order-result.model';
import { ServiceLocationSearchResult } from '@core-module/models/external-dependencies/service-location-search-result.model';
import { BusinessTypeLookup } from '@core-module/models/lookups/business-type-lookup.model';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { ApiResourceResponse } from '@coreconfig/api-resource-response.interface';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { CountCollectionModel } from '@coremodels/api/count-collection.model';
import { ErpApiService } from '@coreservices/erp-api.service';
import { InstrumentationService } from '@coreservices/logging/instrumentationService.service';
import { LoggingVerbs } from '@coreservices/logging/loggingVerbs.model';
import { TrackingEventType } from '@coreservices/logging/tracking-event-type.enum';
import { GlobalSearchFormatter } from './global-search-formatter';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/247298
 */
@Injectable({ providedIn: 'root' })
export class GlobalSearchServiceData {

    constructor(
        private instrumentationService: InstrumentationService,
        private erpApiService: ErpApiService,
        private enhancedErpApiService: EnhancedErpApiService) {
    }

    private addressFieldDisplayConfigurations: AddressFieldDisplayConfiguration[] = [];

    doGlobalSearch(searchTerm: string, businessTypeId: number = 0) {
        this.instrumentationService.trackEvent(LoggingVerbs.GlobalSearchRequest, { searchTerm }, null, TrackingEventType.GlobalSearchTerm);
        this.instrumentationService.startTrackTimedEvent(LoggingVerbs.GlobalSearchRequestTimer);
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/globalsearches'];
        apiRequest.searchTerms = [searchTerm.trim()];
        apiRequest.max = 5;
        apiRequest.includeCount = true;
        apiRequest.filters = [];

        if (businessTypeId > 0) {
            apiRequest.filters.push({
                filterOperation: FilterOperation.Equal,
                name: 'BusinessTypeId',
                value: businessTypeId
            });
        }

        return this.erpApiService.getRequest<ApiResourceResponse, GlobalSearch>(apiRequest, this.getGlobalSearchMap.bind(this), GlobalSearch);
    }

    doMunicipalOfferingsSearch(searchTerm: string, page: number, businessTypeId: number = 0) {
        this.instrumentationService.trackEvent(LoggingVerbs.MunicipalOfferingsSearchRequest, { searchTerm }, null, TrackingEventType.GlobalSearchTerm);
        this.instrumentationService.startTrackTimedEvent(LoggingVerbs.MunicipalOfferingsSearchRequestTimer);
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/municipalOfferingsSearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 50;
        apiRequest.page = page;
        apiRequest.includeCount = true;
        apiRequest.filters = [];

        if (businessTypeId > 0) {
            apiRequest.filters.push({
                filterOperation: FilterOperation.Equal,
                name: 'BusinessTypeId',
                value: businessTypeId
            });
        }
        return this.erpApiService.getRequest<ApiResourceResponse, MunicipalOfferingsSearchResult>(apiRequest, this.getMunicipalOfferingsSearchMap.bind(this), new CountCollectionModel<MunicipalOfferingsSearchResult>([], 0));
    }

    doCustomerSearch(searchTerm: string, page: number, businessTypeId: number = 0) {
        this.instrumentationService.trackEvent(LoggingVerbs.CustomerSearchRequest, { searchTerm }, null, TrackingEventType.GlobalSearchTerm);
        this.instrumentationService.startTrackTimedEvent(LoggingVerbs.CustomerSearchRequestTimer);
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/customersearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 50;
        apiRequest.page = page;
        apiRequest.includeCount = true;
        apiRequest.filters = [];

        if (businessTypeId > 0) {
            apiRequest.filters.push({
                filterOperation: FilterOperation.Equal,
                name: 'BusinessTypeId',
                value: businessTypeId
            });
        }
        return this.erpApiService.getRequest<ApiResourceResponse, CustomerSearchResult>(apiRequest, this.getCustomerSearchMap.bind(this), new CountCollectionModel<CustomerSearchResult>([], 0));
    }

    doSiteSearch(searchTerm: string, page: number, businessTypeId: number = 0) {
        this.instrumentationService.trackEvent(LoggingVerbs.SiteSearchRequest, { searchTerm }, null, TrackingEventType.GlobalSearchTerm);
        this.instrumentationService.startTrackTimedEvent(LoggingVerbs.SiteSearchRequestTimer);
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/sitelocationsearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 50;
        apiRequest.page = page;
        apiRequest.includeCount = true;
        apiRequest.filters = [];

        if (businessTypeId > 0) {
            apiRequest.filters.push({
                filterOperation: FilterOperation.Equal,
                name: 'BusinessTypeId',
                value: businessTypeId
            });
        }
        return this.erpApiService.getRequest<ApiResourceResponse, ServiceLocationSearchResult>(
            apiRequest,
            this.getServiceLocationSearchMap.bind(this),
            new CountCollectionModel<ServiceLocationSearchResult>([], 0));
    }

    doContractSearch(searchTerm: string, page: number, businessTypeId: number = 0) {
        this.instrumentationService.trackEvent(LoggingVerbs.ContractSearchRequest, { searchTerm }, null, TrackingEventType.GlobalSearchTerm);
        this.instrumentationService.startTrackTimedEvent(LoggingVerbs.ContractSearchRequestTimer);
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/contractsearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 50;
        apiRequest.page = page;
        apiRequest.includeCount = true;
        apiRequest.filters = [];

        if (businessTypeId > 0) {
            apiRequest.filters.push({
                filterOperation: FilterOperation.Equal,
                name: 'BusinessTypeId',
                value: businessTypeId
            });
        }
        return this.erpApiService.getRequest<ApiResourceResponse, ContractSearchResult>(apiRequest, this.getContractSearchMap.bind(this), new CountCollectionModel<ContractSearchResult>([], 0));
    }

    getBusinessTypes() {
        const apiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.filters = [];
        apiRequest.urlResourcePath = ['settings', 'CustomerTemplateBusinessTypes'];

        return this.enhancedErpApiService.getArray<BusinessTypeLookup>(apiRequest, BusinessTypeLookup);
    }

    private getGlobalSearchMap(response: ApiResourceResponse) {
        const responsePayload = response.resource[0];
        const result: GlobalSearch = ClassBuilder.buildFromApiResourceResponse<GlobalSearch>(
            responsePayload,
            GlobalSearch
        );

        this.addressFieldDisplayConfigurations = ClassBuilder.buildFromApiResourceResults<AddressFieldDisplayConfiguration>(
            responsePayload?.AddressFieldDisplayConfigurations,
            AddressFieldDisplayConfiguration
        );

        result.municipalOfferingsSearch = this.getGlobalMunicipalOfferingsSearchMap(responsePayload);
        result.customerSearch = this.getGlobalCustomerSearchMap(responsePayload);
        result.contractSearch = this.getGlobalContractSearchMap(responsePayload);
        result.serviceLocationSearch = this.getGlobalServiceLocationSearchMap(responsePayload);
        this.instrumentationService.stopTrackTimedEvent(LoggingVerbs.GlobalSearchRequestTimer);
        return result;
    }

    private getGlobalMunicipalOfferingsSearchMap(response: any) {
        return this.municipalOfferingsSearchMap(response['GlobalMunicipalOfferingsSearch']);
    }

    private getGlobalCustomerSearchMap(response: any) {
        return this.customerSearchMap(response['GlobalCustomerSearch']);
    }

    private getGlobalContractSearchMap(response: any) {
        return this.contractSearchMap(response['GlobalContractSearch']);
    }

    private getGlobalServiceLocationSearchMap(response: any) {
        return this.serviceLocationSearchMap(response['GlobalServiceLocationSearch']);
    }

    private getMunicipalOfferingsSearchMap(response: ApiResourceResponse) {
        this.instrumentationService.stopTrackTimedEvent(LoggingVerbs.MunicipalOfferingsSearchRequestTimer);
        return new CountCollectionModel<MunicipalOfferingsSearchResult>(this.municipalOfferingsSearchMap(response.resource), response.extra.count);
    }

    private getCustomerSearchMap(response: ApiResourceResponse) {
        this.instrumentationService.stopTrackTimedEvent(LoggingVerbs.CustomerSearchRequestTimer);
        return new CountCollectionModel<CustomerSearchResult>(this.customerSearchMap(response.resource), response.extra.count);
    }

    private getServiceLocationSearchMap(response: ApiResourceResponse) {
        this.instrumentationService.stopTrackTimedEvent(LoggingVerbs.SiteSearchRequestTimer);
        return new CountCollectionModel<ServiceLocationSearchResult>(this.serviceLocationSearchMap(response.resource), response.extra.count);
    }

    private getContractSearchMap(response: ApiResourceResponse) {
        this.instrumentationService.stopTrackTimedEvent(LoggingVerbs.ContractSearchRequestTimer);
        return new CountCollectionModel<ContractSearchResult>(this.contractSearchMap(response.resource), response.extra.count);
    }

    private municipalOfferingsSearchMap(response: any) {
        const result: MunicipalOfferingsSearchResult[] =
            ClassBuilder.buildFromApiResourceResults<MunicipalOfferingsSearchResult>(response, MunicipalOfferingsSearchResult);
        if (result != null) {
            result.forEach(element => {
                GlobalSearchFormatter.getFormattedFullAddress(this.addressFieldDisplayConfigurations, element);
                GlobalSearchFormatter.setCustomerStatus(element);
            });
        }
        return result;
    }

    private customerSearchMap(response: any) {
        const result: CustomerSearchResult[] = ClassBuilder.buildFromApiResourceResults<CustomerSearchResult>(
            response,
            CustomerSearchResult
        );
        if (result != null) {
            result.forEach(element => {
                GlobalSearchFormatter.getFormattedFullAddress(this.addressFieldDisplayConfigurations, element);
                GlobalSearchFormatter.setCustomerStatus(element);
            });
        }
        return result;
    }

    private serviceLocationSearchMap(response: any) {
        const result: ServiceLocationSearchResult[] = ClassBuilder.buildFromApiResourceResults<ServiceLocationSearchResult>(
            response,
            ServiceLocationSearchResult
        );
        if (result != null) {
            let i = 0;
            while (i < result.length) {
                result[i].orders = ClassBuilder.buildFromApiResourceResults<SeviceLocationSearchOrderResult>(
                    response[i]['Orders'],
                    SeviceLocationSearchOrderResult
                );
                GlobalSearchFormatter.getFormattedFullAddress(this.addressFieldDisplayConfigurations, result[i]);
                if (result[i].orders != null) {
                    result[i].orders.forEach(element => {
                        GlobalSearchFormatter.setDisplayOrder(element);
                    });
                }
                i++;
            }
            result.forEach(element => {
                if (element.orders != null) {
                    element.displayOrders = element.orders.slice(0, 3);
                }
            });
        }
        return result;
    }

    private contractSearchMap(response: any) {
        const result: ContractSearchResult[] = ClassBuilder.buildFromApiResourceResults<ContractSearchResult>(
            response,
            ContractSearchResult
        );
        if (result != null) {
            let i = 0;
            while (i < result.length) {
                GlobalSearchFormatter.getFormattedFullAddress(this.addressFieldDisplayConfigurations, result[i]);
                if (response[i]['Orders']) {
                    result[i].orders = ClassBuilder.buildFromApiResourceResults<ContractSearchOrderResult>(
                        response[i]['Orders'],
                        ContractSearchOrderResult
                    );
                }
                GlobalSearchFormatter.setDisplayServices(result[i]);
                if (result[i].orders != null) {
                    result[i].orders.forEach(element => {
                        GlobalSearchFormatter.setDisplayOrder(element);
                    });
                }
                i++;
            }

            result.forEach(element => {
                if (element.orders != null) {
                    element.displayOrders = element.orders.slice(0, 3);
                }
            });
        }
        return result;
    }
}
