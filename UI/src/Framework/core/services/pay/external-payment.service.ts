import { Injectable } from '@angular/core';
import { ExternalPaymentTransaction } from '@core-module/models/external-payment/external-payment-transaction.model';
import { InitiateExternalPaymentRequest } from '@core-module/models/external-payment/initiate-external-payment-request.model';
import { InitiateExternalPaymentResponse } from '@core-module/models/external-payment/initiate-external-payment-response.model';
import { BaseService } from '@core-module/services/base.service';
import { Observable } from 'rxjs';
import { ExternalPaymentServiceData } from './external-payment.service.data';

/**
 * @deprecated Move to PlatformUI
 */
@Injectable()
export class ExternalPaymentService extends BaseService {

    static providers = [ExternalPaymentService, ExternalPaymentServiceData];

    constructor(private dataService: ExternalPaymentServiceData) {
        super();
    }

    initiatePaymentRequest(paymentRequest: InitiateExternalPaymentRequest): Observable<InitiateExternalPaymentResponse> {
        return this.dataService.initiatePaymentRequest(paymentRequest);
    }

    getPaymentTransactionStatus(transactionGuid: string): Observable<ExternalPaymentTransaction> {
        return this.dataService.getPaymentTransactionStatus(transactionGuid);
    }
}
