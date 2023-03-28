import { Injectable } from '@angular/core';
import { ApiRequest } from '@core-module/models/api/api-request.model';
import { FilterOperation } from '@core-module/models/api/filters/filter-operation.enum';
import { ExternalPaymentTransaction } from '@core-module/models/external-payment/external-payment-transaction.model';
import { InitiateExternalPaymentRequest } from '@core-module/models/external-payment/initiate-external-payment-request.model';
import { InitiateExternalPaymentResponse } from '@core-module/models/external-payment/initiate-external-payment-response.model';
import { BaseService } from '@core-module/services/base.service';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { Observable } from 'rxjs';

/**
 * @deprecated Move to PlatformUI
 */
@Injectable()
export class ExternalPaymentServiceData extends BaseService {

    constructor(private erpApiService: EnhancedErpApiService) {
        super();
    }

    initiatePaymentRequest(paymentRequest: InitiateExternalPaymentRequest): Observable<InitiateExternalPaymentResponse> {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['payment/initiatePayment'];
        return this.erpApiService.postMessage(apiRequest, paymentRequest, InitiateExternalPaymentRequest, InitiateExternalPaymentResponse);
    }

    getPaymentTransactionStatus(transactionGuid: string): Observable<ExternalPaymentTransaction> {
        const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        apiRequest.urlResourcePath = ['payment/transactions'];
        apiRequest.filters = [
            {
                filterOperation: FilterOperation.Equal,
                name: 'PaymentTransactionGUID',
                value: transactionGuid
            }
        ];
        return this.erpApiService.get(apiRequest, ExternalPaymentTransaction);
    }
}
