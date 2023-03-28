import { Injectable } from '@angular/core';
import { ApiRequest } from '@core-module/models/api/api-request.model';
import { FilterOperation } from '@core-module/models/api/filters/filter-operation.enum';
import { CurrencyConversionRequest } from '@core-module/models/finance/currency-conversion-request.model';
import { CurrencyConversionResponse } from '@core-module/models/finance/currency-conversion-response.model';
import { CurrencyLookup } from '@core-module/models/lookups/currency-lookup.model';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { Observable } from 'rxjs';

@Injectable()
export class CurrencyConversionServiceData {

    constructor(private erpApiService: EnhancedErpApiService) {
    }

    getCurrencyConversion(request: CurrencyConversionRequest): Observable<CurrencyConversionResponse> {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['finance/currencyConversion'];
        return this.erpApiService.postMessage(apiRequest, request, CurrencyConversionRequest, CurrencyConversionResponse);
    }

    getConvertableCurrencies(toCurrencyId: number, dateOfConversion: Date): Observable<CurrencyLookup[]> {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['settings/convertablecurrencies'];
        apiRequest.filters = [
            {
                filterOperation: FilterOperation.Equal,
                name: 'ToCurrencyId',
                value: toCurrencyId
            },
            {
                filterOperation: FilterOperation.Equal,
                name: 'DateOfConversion',
                value: dateOfConversion
            }
        ];
        return this.erpApiService.getArray(apiRequest, CurrencyLookup);
    }
}
