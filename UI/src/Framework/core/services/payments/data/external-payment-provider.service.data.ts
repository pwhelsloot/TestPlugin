import { Injectable } from '@angular/core';
import { ApiRequest } from '@core-module/models/api/api-request.model';
import { ExternalPaymentProviderData } from '@core-module/models/payments/external-payment-provider-data.model';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';

/**
 * @deprecated Move to PlatformUI
 */
@Injectable()
export class ExternalPaymentProviderServiceData {

    constructor(private enhancedErpApiService: EnhancedErpApiService) {
    }

    getExternalPaymentProviderData() {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['customer/externalPaymentProviderDatas'];
        apiRequest.filters = [];

        return this.enhancedErpApiService.get<ExternalPaymentProviderData>(apiRequest, ExternalPaymentProviderData);
    }
}
