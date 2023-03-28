import { Injectable } from '@angular/core';
import { CurrencyConversionRequest } from '@core-module/models/finance/currency-conversion-request.model';
import { CurrencyConversionResponse } from '@core-module/models/finance/currency-conversion-response.model';
import { CurrencyLookup } from '@core-module/models/lookups/currency-lookup.model';
import { BaseService } from '@coreservices/base.service';
import { Observable, Subject } from 'rxjs';
import { switchMap, take, takeUntil, tap } from 'rxjs/operators';
import { CurrencyConversionServiceData } from './currency-conversion.service.data';

@Injectable()
export class CurrencyConversionService extends BaseService {

    static providers = [CurrencyConversionService, CurrencyConversionServiceData];

    currencyConversionResult$: Observable<CurrencyConversionResponse>;
    get loading(): boolean { return this.conversionLoading || this.currenciesLoading; }
    conversionLoading = false;

    constructor(
        private dataService: CurrencyConversionServiceData) {
        super();
        this.setUpCurrencyConversionStream();
    }

    private currencyConversionResult = new Subject<CurrencyConversionResponse>();
    private currencyConversionRequest = new Subject<CurrencyConversionRequest>();
    private currenciesLoading = false;

    requestConversion(request: CurrencyConversionRequest) {
        if (request.isValid()) {
            this.conversionLoading = true;
            this.currencyConversionRequest.next(request);
        }
    }

    getConvertableCurrencies(toCurrencyId: number, dateOfConversion: Date): Observable<CurrencyLookup[]> {
        this.currenciesLoading = true;
        return this.dataService.getConvertableCurrencies(toCurrencyId, dateOfConversion)
            // RDM - The below just ensures we set loading = false on next/error/complete
            .pipe(tap(() => { this.currenciesLoading = false; }, () => { this.currenciesLoading = false; }, () => { this.currenciesLoading = false; }), take(1));
    }

    private setUpCurrencyConversionStream() {
        this.currencyConversionResult$ = this.currencyConversionResult.asObservable();
        this.currencyConversionRequest.pipe(
            takeUntil(this.unsubscribe),
            switchMap((request: CurrencyConversionRequest) => {
                return this.dataService.getCurrencyConversion(request);
            })).subscribe((result: CurrencyConversionResponse) => {
                this.currencyConversionResult.next(result);
                this.conversionLoading = false;
            });
    }
}
