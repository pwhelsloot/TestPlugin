import { Injectable } from '@angular/core';
import { BaseService } from '@core-module/services/base.service';
import { Subject } from 'rxjs';

/**
 * @deprecated Move to PlatformUI
 */
@Injectable()
export class ExternalPaymentModalService extends BaseService {

    startModalSubject = new Subject();
    isLoadingModal = false;
}
