
import { Injectable } from '@angular/core';
import { TaxCalculationRequest } from '@core-module/models/tax-calculation-request.model';
import { TaxCalculationResponse } from '@core-module/models/tax-calculation-response.model';
import { Observable, Subject } from 'rxjs';
import { switchMap, takeUntil } from 'rxjs/operators';
import { BaseService } from './base.service';
import { TaxCalculationServiceData } from './tax-calculation.service.data';

/**
 * @deprecated Move to PlatformUI
 */
@Injectable()
export class TaxCalculationService extends BaseService {

    static providers = [TaxCalculationService, TaxCalculationServiceData];

    taxCalculationResult$: Observable<TaxCalculationResponse>;
    calculationLoading = false;

    minChargeTaxCalculationResult$: Observable<TaxCalculationResponse>;
    minChargeCalculationLoading = false;

    maxChargeTaxCalculationResult$: Observable<TaxCalculationResponse>;
    maxChargeCalculationLoading = false;

    constructor(
        private dataService: TaxCalculationServiceData) {
        super();
        this.setUpCalculationRequestStream();
        this.setUpMinChargeCalculationRequestStream();
        this.setUpMaxChargeCalculationRequestStream();
    }

    private taxCalculationResult = new Subject<TaxCalculationResponse>();
    private taxCalculationResultRequest = new Subject<TaxCalculationRequest>();

    private minChargeTaxCalculationResult = new Subject<TaxCalculationResponse>();
    private minChargeTaxCalculationResultRequest = new Subject<TaxCalculationRequest>();

    private maxChargeTaxCalculationResult = new Subject<TaxCalculationResponse>();
    private maxChargeTaxCalculationResultRequest = new Subject<TaxCalculationRequest>();

    requestCalculation(request: TaxCalculationRequest) {
        this.calculationLoading = true;
        this.taxCalculationResultRequest.next(request);
    }

    requestMinChargeCalculation(request: TaxCalculationRequest) {
        this.minChargeCalculationLoading = true;
        this.minChargeTaxCalculationResultRequest.next(request);
    }

    requestMaxChargeCalculation(request: TaxCalculationRequest) {
        this.maxChargeCalculationLoading = true;
        this.maxChargeTaxCalculationResultRequest.next(request);
    }

    getCalculation(request: TaxCalculationRequest): Observable<TaxCalculationResponse> {
        return this.dataService.getTaxCalculation(request);
    }

    private setUpCalculationRequestStream() {
        this.taxCalculationResult$ = this.taxCalculationResult.asObservable();
        this.taxCalculationResultRequest.pipe(
            takeUntil(this.unsubscribe),
            switchMap((request: TaxCalculationRequest) => {
                return this.dataService.getTaxCalculation(request);
            })).subscribe((result: TaxCalculationResponse) => {
                this.taxCalculationResult.next(result);
                this.calculationLoading = false;
            });
    }

    private setUpMinChargeCalculationRequestStream() {
        this.minChargeTaxCalculationResult$ = this.minChargeTaxCalculationResult.asObservable();
        this.minChargeTaxCalculationResultRequest.pipe(
            takeUntil(this.unsubscribe),
            switchMap((request: TaxCalculationRequest) => {
                return this.dataService.getTaxCalculation(request);
            })).subscribe((result: TaxCalculationResponse) => {
                this.minChargeTaxCalculationResult.next(result);
                this.minChargeCalculationLoading = false;
            });
    }

    private setUpMaxChargeCalculationRequestStream() {
        this.maxChargeTaxCalculationResult$ = this.maxChargeTaxCalculationResult.asObservable();
        this.maxChargeTaxCalculationResultRequest.pipe(
            takeUntil(this.unsubscribe),
            switchMap((request: TaxCalculationRequest) => {
                return this.dataService.getTaxCalculation(request);
            })).subscribe((result: TaxCalculationResponse) => {
                this.maxChargeTaxCalculationResult.next(result);
                this.maxChargeCalculationLoading = false;
            });
    }
}
