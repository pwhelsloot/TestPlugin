import { Injectable } from '@angular/core';
import { FilterOperation } from '@core-module/models/api/filters/filter-operation.enum';
import { Country } from '@core-module/models/lookups/country.model';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { Observable } from 'rxjs';

/**
 * @deprecated Move to PlatformUI + ScaleUI (or new module) https://dev.azure.com/amcsgroup/Platform/_workitems/edit/188268
 */
@Injectable()
export class CountryServiceData {

    constructor(private erpApiService: EnhancedErpApiService) {
    }

    getDefaultCountry(): Observable<Country> {
        const apiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['settings/countries'];
        apiRequest.filters = [{
            filterOperation: FilterOperation.Equal,
            name: 'IsDefault',
            value: true
        }];
        return this.erpApiService.get(apiRequest, Country);
    }
}
