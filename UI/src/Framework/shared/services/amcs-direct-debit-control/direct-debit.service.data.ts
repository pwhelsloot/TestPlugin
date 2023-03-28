import { Injectable } from '@angular/core';
import { ApiRequest } from '@core-module/models/api/api-request.model';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { DirectDebitUniqueKeyRequest } from '@shared-module/models/amcs-direct-debit-control/direct-debit-unique-key-request.model';
import { DirectDebitUniqueKeyResponse } from '@shared-module/models/amcs-direct-debit-control/direct-debit-unique-key-response.model';
import { Observable } from 'rxjs';

/**
 * @deprecated Move to PlatformUI
 */
@Injectable()
export class DirectDebitDataService {

    constructor(private erpApiService: EnhancedErpApiService) {
    }

    generateUniqueKey(requestModel: DirectDebitUniqueKeyRequest): Observable<DirectDebitUniqueKeyResponse> {
        const request: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
        request.urlResourcePath = ['payment/generateUniqueKeys'];
        return this.erpApiService.postMessage(request, requestModel, DirectDebitUniqueKeyRequest, DirectDebitUniqueKeyResponse);
    }
}
