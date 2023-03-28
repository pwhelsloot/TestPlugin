import { Injectable } from '@angular/core';
import { ExternalPaymentProviderData } from '@core-module/models/payments/external-payment-provider-data.model';
import { Observable, Subject } from 'rxjs';
import { switchMap, takeUntil } from 'rxjs/operators';
import { BaseService } from '../base.service';
import { ExternalPaymentProviderServiceData } from './data/external-payment-provider.service.data';

/**
 * @deprecated Move to PlatformUI
 */
@Injectable()
export class ExternalPaymentProviderService extends BaseService {

    static providers = [ExternalPaymentProviderService, ExternalPaymentProviderServiceData];

    editorData$: Observable<ExternalPaymentProviderData>;

    constructor(private dataService: ExternalPaymentProviderServiceData) {
        super();
        this.requestEditorDataSetupStream();
    }

    private editorData = new Subject<ExternalPaymentProviderData>();
    private requestEditorDataSubject = new Subject();

    requestExternalPaymentProviderData() {
        this.requestEditorDataSubject.next();
    }

    private requestEditorDataSetupStream() {
        this.editorData$ = this.editorData.asObservable();
        this.requestEditorDataSubject.pipe(
            switchMap(() => {
                return this.dataService.getExternalPaymentProviderData();
            }),
            takeUntil(this.unsubscribe)
        ).subscribe((editorData: ExternalPaymentProviderData) => {
            this.editorData.next(editorData);
        });
    }
}
