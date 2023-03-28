import { Injectable } from '@angular/core';
import { ClassBuilder } from '@core-module/helpers/dto/class-builder';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { FilterOperation } from '@core-module/models/api/filters/filter-operation.enum';
import { AddressFormatter } from '@core-module/models/external-dependencies/address-formatter';
import { ARAccountCodeSearchResult } from '@core-module/models/keyword-search/ar-account-code-search.result.model';
import { ContainerSearchResult } from '@core-module/models/keyword-search/container-search-result.model';
import { FederalIdSearchResult } from '@core-module/models/keyword-search/federal-id-search-result.model';
import { InvoiceNumberSearchResult } from '@core-module/models/keyword-search/invoice-number-search-result.model';
import { PrePayCardSearchResult } from '@core-module/models/keyword-search/pre-pay-card-search.result.model';
import { SalesOrderSearchResult } from '@core-module/models/keyword-search/sales-order-search.result.model';
import { TradingNameSearchResult } from '@core-module/models/keyword-search/trading-name-search-result.model';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { ApiResourceResponse } from '@coreconfig/api-resource-response.interface';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { GlobalSearchKeywordQuickSearch } from '@coremodels/global-search/global-search-keyword-quick-search.model';
import { combineLatest, map } from 'rxjs/operators';
import { SharedTranslationsService } from '../../../../shared/services/shared-translations.service';
import { BarcodeSearchResult } from '../../../models/keyword-search/barcode-search.result.model';
import { PurchaseOrderNumberSearchResult } from '../../../models/keyword-search/purchase-order-number-search.result.model';
import { PurchaseOrderSearchResult } from '../../../models/keyword-search/purchase-order-search.result.model';
import { ReportSearchResult } from '../../../models/keyword-search/report-search.result.model';
import { WasteDeclarationSearchResult } from '../../../models/keyword-search/waste-declaration-search-result.model';
import { WeighingTicketSearchResult } from '../../../models/keyword-search/weighing-ticket-search-result.model';
import { ReportTreeEnum } from '../../../models/report-tree.enum';
import { ErpApiService } from '../../erp-api.service';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/247298
 */
@Injectable({ providedIn: 'root' })
export class GlobalKeywordSearchServiceData {

    constructor(private erpApiService: ErpApiService,
        private enhancedErpApiService: EnhancedErpApiService,
        private translationsService: SharedTranslationsService) {
    }

    doContainerKeywordQuickSearch(searchTerm: string) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/containerQuickSearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 10;
        return this.erpApiService.getRequest<ApiResourceResponse, GlobalSearchKeywordQuickSearch>(apiRequest, this.getQuickSearchMap, GlobalSearchKeywordQuickSearch);
    }

    doInvoiceNumberKeywordQuickSearch(searchTerm: string) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/invoiceNumberQuickSearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 10;
        return this.erpApiService.getRequest<ApiResourceResponse, GlobalSearchKeywordQuickSearch>(apiRequest, this.getQuickSearchMap, GlobalSearchKeywordQuickSearch);
    }

    doPrePayCardKeywordQuickSearch(searchTerm: string) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/prepayCardQuickSearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 10;
        return this.erpApiService.getRequest<ApiResourceResponse, GlobalSearchKeywordQuickSearch>(apiRequest, this.getQuickSearchMap, GlobalSearchKeywordQuickSearch);
    }

    doFederalIdKeywordQuickSearch(searchTerm: string) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/federalIdQuickSearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 10;
        return this.erpApiService.getRequest<ApiResourceResponse, GlobalSearchKeywordQuickSearch>(apiRequest, this.getQuickSearchMap, GlobalSearchKeywordQuickSearch);
    }

    doSalesOrderNoKeywordQuickSearch(searchTerm: string) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/SalesOrderQuickSearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 10;
        return this.erpApiService.getRequest<ApiResourceResponse, GlobalSearchKeywordQuickSearch>(apiRequest, this.getQuickSearchMap, GlobalSearchKeywordQuickSearch);
    }

    doPurchaseOrderNoKeywordQuickSearch(searchTerm: string) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/PurchaseOrderQuickSearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 10;
        return this.erpApiService.getRequest<ApiResourceResponse, GlobalSearchKeywordQuickSearch>(apiRequest, this.getQuickSearchMap, GlobalSearchKeywordQuickSearch);
    }

    doTradingNameNoKeywordQuickSearch(searchTerm: string) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/TradingNameQuickSearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 10;
        return this.erpApiService.getRequest<ApiResourceResponse, GlobalSearchKeywordQuickSearch>(apiRequest, this.getQuickSearchMap, GlobalSearchKeywordQuickSearch);
    }

    doReportKeywordQuickSearch(searchTerm: string) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/ReportQuickSearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 10;
        apiRequest.filters = [
            {
                filterOperation: FilterOperation.Equal,
                name: 'Tree',
                value: ReportTreeEnum.DataMart
            }
        ];
        return this.erpApiService.getRequest<ApiResourceResponse, GlobalSearchKeywordQuickSearch>(apiRequest, this.getQuickSearchMap, GlobalSearchKeywordQuickSearch);
    }

    doWasteDeclarationNoKeywordQuickSearch(searchTerm: string) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/WasteDeclarationQuickSearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 10;
        return this.erpApiService.getRequest<ApiResourceResponse, GlobalSearchKeywordQuickSearch>(apiRequest, this.getQuickSearchMap, GlobalSearchKeywordQuickSearch);
    }

    doPurchaseOrderNumberNoKeywordQuickSearch(searchTerm: string) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/PurchaseOrderNumberQuickSearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 10;
        return this.erpApiService.getRequest<ApiResourceResponse, GlobalSearchKeywordQuickSearch>(apiRequest, this.getQuickSearchMap, GlobalSearchKeywordQuickSearch);
    }

    doWeighingTicketNoKeywordQuickSearch(searchTerm: string) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/WeighingTicketQuickSearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 10;
        return this.erpApiService.getRequest<ApiResourceResponse, GlobalSearchKeywordQuickSearch>(apiRequest, this.getQuickSearchMap, GlobalSearchKeywordQuickSearch);
    }

    doARAccountCodeQuickSearch(searchTerm: string) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/ARAccountCodeQuickSearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 10;
        return this.erpApiService.getRequest<ApiResourceResponse, GlobalSearchKeywordQuickSearch>(apiRequest, this.getQuickSearchMap, GlobalSearchKeywordQuickSearch);
    }

    doBarCodeNoKeywordQuickSearch(searchTerm: string) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/BarcodeQuickSearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 10;
        return this.erpApiService.getRequest<ApiResourceResponse, GlobalSearchKeywordQuickSearch>(apiRequest, this.getQuickSearchMap, GlobalSearchKeywordQuickSearch);
    }

    doMatrikkelnummerKeywordQuickSearch(searchTerm: string) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/matrikkelnummerQuickSearches'];
        apiRequest.searchTerms = [searchTerm];
        apiRequest.max = 10;
        return this.erpApiService.getRequest<ApiResourceResponse, GlobalSearchKeywordQuickSearch>(apiRequest, this.getQuickSearchMap, GlobalSearchKeywordQuickSearch);
    }

    getContainerSearchResult(containerId: number) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/ContainerSearchResults'];
        apiRequest.filters = [
            {
                filterOperation: FilterOperation.Equal,
                name: 'ContainerId',
                value: containerId
            }
        ];
        return this.enhancedErpApiService.get<ContainerSearchResult>(apiRequest, ContainerSearchResult)
            .pipe(map((result: ContainerSearchResult) => {
                result.formattedAddress = AddressFormatter.getFullGenericAddress(
                    result.siteHouseNo,
                    result.siteAddress1,
                    result.siteAddress2,
                    result.siteAddress3,
                    result.siteAddress4,
                    result.siteAddress5,
                    result.sitePostcode);
                return result;
            }));
    }

    getPrePayCardSearchResult(prePayCardId: number) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/PrePayCardSearchResults'];
        apiRequest.filters = [
            {
                filterOperation: FilterOperation.Equal,
                name: 'PrePayCardId',
                value: prePayCardId
            }
        ];
        return this.enhancedErpApiService.get<PrePayCardSearchResult>(apiRequest, PrePayCardSearchResult)
            .pipe(map((result: PrePayCardSearchResult) => {

                // temporary fix until we have a solution for the mapper to handle null values in the JSON.
                if (result.siteHouseNo === undefined) {
                    result.siteHouseNo = null;
                }

                if (result.siteAddress1 === undefined) {
                    result.siteAddress1 = null;
                }

                if (result.siteAddress2 === undefined) {
                    result.siteAddress2 = null;
                }

                if (result.siteAddress3 === undefined) {
                    result.siteAddress3 = null;
                }

                if (result.siteAddress4 === undefined) {
                    result.siteAddress4 = null;
                }

                if (result.siteAddress5 === undefined) {
                    result.siteAddress5 = null;
                }

                if (result.sitePostcode === undefined) {
                    result.sitePostcode = null;
                }

                if (result.phoneNumber === undefined) {
                    result.phoneNumber = null;
                }

                if (result.deactivatedDate === undefined) {
                    result.deactivatedDate = null;
                }

                result.formattedAddress = AddressFormatter.getFullGenericAddress(
                    result.siteHouseNo,
                    result.siteAddress1,
                    result.siteAddress2,
                    result.siteAddress3,
                    result.siteAddress4,
                    result.siteAddress5,
                    result.sitePostcode);

                if (result.isActive) {

                }

                return result;
            }));
    }

    getFederalIdSearchResult(customerId: number, customerSiteId: number) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/FederalIdSearchResults'];
        apiRequest.filters = [];
        if (isTruthy(customerId)) {
            apiRequest.filters.push(
                {
                    filterOperation: FilterOperation.Equal,
                    name: 'CustomerId',
                    value: customerId
                }
            );
        }
        if (isTruthy(customerSiteId)) {
            apiRequest.filters.push(
                {
                    filterOperation: FilterOperation.Equal,
                    name: 'CustomerSiteId',
                    value: customerSiteId
                }
            );
        }
        return this.enhancedErpApiService.get<FederalIdSearchResult>(apiRequest, FederalIdSearchResult)
            .pipe(map((result: FederalIdSearchResult) => {
                result.formattedAddress = AddressFormatter.getFullGenericAddress(
                    result.siteHouseNo,
                    result.siteAddress1,
                    result.siteAddress2,
                    result.siteAddress3,
                    result.siteAddress4,
                    result.siteAddress5,
                    result.sitePostcode);
                return result;
            }));
    }

    getSalesOrderSearchResult(salesOrderId: number) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/SalesOrderSearchResults'];
        apiRequest.filters = [
            {
                filterOperation: FilterOperation.Equal,
                name: 'salesOrderId',
                value: salesOrderId
            }
        ];
        return this.enhancedErpApiService.get<SalesOrderSearchResult>(apiRequest, SalesOrderSearchResult)
            .pipe(map((result: SalesOrderSearchResult) => {
                return result;
            }));
    }

    getInvoiceNumberSearchResult(invoiceId: number) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/InvoiceNumberSearchResults'];
        apiRequest.filters = [
            {
                filterOperation: FilterOperation.Equal,
                name: 'InvoiceId',
                value: invoiceId
            }
        ];
        return this.enhancedErpApiService.get<InvoiceNumberSearchResult>(apiRequest, InvoiceNumberSearchResult)
            .pipe(map((result: InvoiceNumberSearchResult) => {
                return result;
            }));
    }

    getPurchaseOrderSearchResult(demandPlanPurchaseOrderId: number) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/PurchaseOrderSearchResults'];
        apiRequest.filters = [
            {
                filterOperation: FilterOperation.Equal,
                name: 'demandPlanPurchaseOrderId',
                value: demandPlanPurchaseOrderId
            }
        ];
        return this.enhancedErpApiService.get<PurchaseOrderSearchResult>(apiRequest, PurchaseOrderSearchResult)
            .pipe(map((result: PurchaseOrderSearchResult) => {
                return result;
            }));
    }

    getTradingNameSearchResult(CustomerTradingNameId: number) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/TradingNameSearchResults'];
        apiRequest.filters = [
            {
                filterOperation: FilterOperation.Equal,
                name: 'CustomerTradingNameId',
                value: CustomerTradingNameId
            }
        ];
        return this.enhancedErpApiService.get<TradingNameSearchResult>(apiRequest, TradingNameSearchResult)
            .pipe(map((result: TradingNameSearchResult) => {
                return result;
            }));
    }

    getReportSearchResult(reportId: number) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/ReportSearchResults'];
        apiRequest.filters = [
            {
                filterOperation: FilterOperation.Equal,
                name: 'ReportId',
                value: reportId
            }
        ];
        return this.enhancedErpApiService.get<ReportSearchResult>(apiRequest, ReportSearchResult)
            .pipe(
                combineLatest(this.translationsService.translations),
                map(data => {
                    const result: ReportSearchResult = data[0];
                    const translations: string[] = data[1];
                    result.init(translations);
                    return result;
                }));
    }

    getWasteDeclarationSearchResult(WasteDeclarationTransactionId: number) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/WasteDeclarationSearchResults'];
        apiRequest.filters = [
            {
                filterOperation: FilterOperation.Equal,
                name: 'WasteDeclarationTransactionId',
                value: WasteDeclarationTransactionId
            }
        ];
        return this.enhancedErpApiService.get<WasteDeclarationSearchResult>(apiRequest, WasteDeclarationSearchResult)
            .pipe(map((result: WasteDeclarationSearchResult) => {
                return result;
            }));
    }

    getPurchaseOrderNumberSearchResult(poNumberId: number) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/PurchaseOrderNumberSearchResults'];
        apiRequest.filters = [
            {
                filterOperation: FilterOperation.Equal,
                name: 'PONumberId',
                value: poNumberId
            }
        ];
        return this.enhancedErpApiService.get<PurchaseOrderNumberSearchResult>(apiRequest, PurchaseOrderNumberSearchResult)
            .pipe(map((result: PurchaseOrderNumberSearchResult) => {
                return result;
            }));
    }

    getWeighingTicketSearchResult(weighingId: number) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/WeighingTicketSearchResults'];
        apiRequest.filters = [
            {
                filterOperation: FilterOperation.Equal,
                name: 'WeighingId',
                value: weighingId
            }
        ];
        return this.enhancedErpApiService.get<WeighingTicketSearchResult>(apiRequest, WeighingTicketSearchResult)
            .pipe(map((result: WeighingTicketSearchResult) => {
                return result;
            }));
    }

    getARAccountCodeSearchResult(customerId: number) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/ARAccountCodeSearchResults'];
        apiRequest.filters = [
            {
                filterOperation: FilterOperation.Equal,
                name: 'CustomerId',
                value: customerId
            }
        ];
        return this.enhancedErpApiService.get<ARAccountCodeSearchResult>(apiRequest, ARAccountCodeSearchResult)
            .pipe(map((result: ARAccountCodeSearchResult) => {
                return result;
            }));
    }

    getBarcodeSearchResult(weighingId: number, barcode: string) {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['globalsearch/BarcodeSearchResults'];
        apiRequest.filters = [];
        if (isTruthy(weighingId)) {
            apiRequest.filters.push(
                {
                    filterOperation: FilterOperation.Equal,
                    name: 'WeighingId',
                    value: weighingId
                }
            );
        }
        if (isTruthy(barcode)) {
            apiRequest.filters.push(
                {
                    filterOperation: FilterOperation.Equal,
                    name: 'Barcode',
                    value: barcode
                }
            );
        }
        return this.enhancedErpApiService.get<BarcodeSearchResult>(apiRequest, BarcodeSearchResult)
            .pipe(map((result: BarcodeSearchResult) => {
                return result;
            }));
    }

    private getQuickSearchMap(response: ApiResourceResponse) {
        return ClassBuilder.buildFromApiResourceResults<GlobalSearchKeywordQuickSearch>(response.resource, GlobalSearchKeywordQuickSearch);
    }
}
