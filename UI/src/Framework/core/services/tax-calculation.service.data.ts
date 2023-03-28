import { Injectable } from '@angular/core';
import { ApiRequest } from '@core-module/models/api/api-request.model';
import { TaxCalculationRequest } from '@core-module/models/tax-calculation-request.model';
import { TaxCalculationResponse } from '@core-module/models/tax-calculation-response.model';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { Observable } from 'rxjs';

/**
 * @deprecated Move to PlatformUI
 */
@Injectable()
export class TaxCalculationServiceData {

    constructor(private erpApiService: EnhancedErpApiService) {
    }

    getTaxCalculation(request: TaxCalculationRequest): Observable<TaxCalculationResponse> {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['customer/ApiTaxCalculation'];
        return this.erpApiService.postMessage(apiRequest, request, TaxCalculationRequest, TaxCalculationResponse);
    }
}
