
import { Injectable } from '@angular/core';
import { FrameworkAuthorisationClaimNames } from '@auth-module/models/framework-authorisation-claim-names.constants';
import { AuthorisationService } from '@auth-module/services/authorisation.service';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { ContractSearchResult } from '@core-module/models/external-dependencies/contract-search-result.model';
import { CustomerSearchResult } from '@core-module/models/external-dependencies/customer-search-result.model';
import { GlobalSearch } from '@core-module/models/external-dependencies/global-search-model';
import { MunicipalOfferingsSearchResult } from '@core-module/models/external-dependencies/municipal-offerings-search-result.model';
import { ServiceLocationSearchResult } from '@core-module/models/external-dependencies/service-location-search-result.model';
import { BusinessTypeLookup } from '@core-module/models/lookups/business-type-lookup.model';
import { CountCollectionModel } from '@coremodels/api/count-collection.model';
import { BaseService } from '@coreservices/base.service';
import { environment } from '@environments/environment';
import { BehaviorSubject, Observable, of, ReplaySubject, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, take, takeUntil } from 'rxjs/operators';
import { GlobalSearchServiceData } from './data/global-search.service.data';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/247298
 */
@Injectable({ providedIn: 'root' })
export class GlobalSearchService extends BaseService {

    globalResults$: Observable<GlobalSearch>;
    municipalOfferingsResults$: Observable<CountCollectionModel<MunicipalOfferingsSearchResult>>;
    customerResults$: Observable<CountCollectionModel<CustomerSearchResult>>;
    siteResults$: Observable<CountCollectionModel<ServiceLocationSearchResult>>;
    contractResults$: Observable<CountCollectionModel<ContractSearchResult>>;
    searchTerm$: Observable<{ searchTerm: string; businessTypeId: number }>;
    businessTypes$: Observable<BusinessTypeLookup[]>;
    loading = false;

    constructor(private dataService: GlobalSearchServiceData,
        private authorisationService: AuthorisationService) {
        super();
        this.searchTerm$ = this.searchTerm.asObservable();
        this.setUpGlobalSearchStream();
        this.setUpMunicipalOfferingsSearchStream();
        this.setUpCustomerSearchStream();
        this.setUpSiteSearchStream();
        this.setUpContractSearchStream();
        this.setUpBusinessTypes();
    }

    private searchTerm: BehaviorSubject<{ searchTerm: string; businessTypeId: number }> = new BehaviorSubject<{ searchTerm: string; businessTypeId: number }>({ searchTerm: null, businessTypeId: 0 });
    private globalResults: ReplaySubject<GlobalSearch> = new ReplaySubject<GlobalSearch>(1);
    private municipalOfferingsResults: ReplaySubject<CountCollectionModel<MunicipalOfferingsSearchResult>> = new ReplaySubject<CountCollectionModel<MunicipalOfferingsSearchResult>>(1);
    private customerResults: ReplaySubject<CountCollectionModel<CustomerSearchResult>> = new ReplaySubject<CountCollectionModel<CustomerSearchResult>>(1);
    private siteResults: ReplaySubject<CountCollectionModel<ServiceLocationSearchResult>> = new ReplaySubject<CountCollectionModel<ServiceLocationSearchResult>>(1);
    private contractResults: ReplaySubject<CountCollectionModel<ContractSearchResult>> = new ReplaySubject<CountCollectionModel<ContractSearchResult>>(1);
    private businessTypeSubject = new Subject<BusinessTypeLookup[]>();

    private requestBusinessTypeSubject = new Subject();
    private globalSearchRequest: Subject<[string, number]> = new Subject<[string, number]>();
    private municipalOfferingsSearchRequest: BehaviorSubject<{ term: string; page: number; businessTypeId: number }> = new BehaviorSubject<{ term: string; page: number; businessTypeId: number }>({ term: null, page: 0, businessTypeId: 0 });
    private customerSearchRequest: BehaviorSubject<{ term: string; page: number; businessTypeId: number }> = new BehaviorSubject<{ term: string; page: number; businessTypeId: number }>({ term: null, page: 0, businessTypeId: 0 });
    private siteSearchRequest: BehaviorSubject<{ term: string; page: number; businessTypeId: number }> = new BehaviorSubject<{ term: string; page: number; businessTypeId: number }>({ term: null, page: 0, businessTypeId: 0 });
    private contractSearchRequest: BehaviorSubject<{ term: string; page: number; businessTypeId: number }> = new BehaviorSubject<{ term: string; page: number; businessTypeId: number }>({ term: null, page: 0, businessTypeId: 0 });

    requestSearch(searchTerm: string, businessTypeId: number = 0) {
        this.searchTerm$.pipe(take(1)).subscribe((data: { searchTerm: string; businessTypeId: number }) => {
            if (data.searchTerm !== searchTerm || businessTypeId !== data.businessTypeId) {
                this.searchTerm.next({ searchTerm, businessTypeId });
            }
        });

        if (searchTerm === null) {
            this.requestGlobalSearch(null);
            this.clearMunicipalOfferingsRequest();
            this.clearContractRequest();
            this.clearCustomerRequest();
            this.clearSiteRequest();
        }
    }

    requestGlobalSearch(searchTerm: string, businessTypeId?: number) {
        this.globalSearchRequest.next([searchTerm, businessTypeId]);
    }

    requestMunicipalOfferingsSearch(searchTerm: string, page?: number, businessTypeId?: number) {
        // If page is null then simply use the page last supplied
        if (page == null) {
            this.municipalOfferingsSearchRequest.pipe(take(1)).subscribe((data: { term: string; page: number }) => {
                this.municipalOfferingsSearchRequest.next({ term: searchTerm, page: data.page, businessTypeId });
            });
        } else {
            this.municipalOfferingsSearchRequest.next({ term: searchTerm, page, businessTypeId });
        }
    }

    requestCustomerSearch(searchTerm: string, page?: number, businessTypeId?: number) {
        // If page is null then simply use the page last supplied
        if (page == null) {
            this.customerSearchRequest.pipe(take(1)).subscribe((data: { term: string; page: number }) => {
                this.customerSearchRequest.next({ term: searchTerm, page: data.page, businessTypeId });
            });
        } else {
            this.customerSearchRequest.next({ term: searchTerm, page, businessTypeId });
        }
    }

    requestSiteSearch(searchTerm: string, page?: number, businessTypeId?: number) {
        // If page is null then simply use the page last supplied
        if (page == null) {
            this.siteSearchRequest.pipe(take(1)).subscribe((data: { term: string; page: number }) => {
                this.siteSearchRequest.next({ term: searchTerm, page: data.page, businessTypeId });
            });
        } else {
            this.siteSearchRequest.next({ term: searchTerm, page, businessTypeId });
        }
    }

    requestContractSearch(searchTerm: string, page?: number, businessTypeId?: number) {
        // If page is null then simply use the page last supplied
        if (page == null) {
            this.contractSearchRequest.pipe(take(1)).subscribe((data: { term: string; page: number }) => {
                this.contractSearchRequest.next({ term: searchTerm, page: data.page, businessTypeId });
            });
        } else {
            this.contractSearchRequest.next({ term: searchTerm, page, businessTypeId });
        }
    }

    requestBusinessTypes() {
        this.requestBusinessTypeSubject.next();
    }

    // Clearing the requests just means resetting them, this doesnt actually make a server
    // call it just wipes the observables.
    clearMunicipalOfferingsRequest() {
        this.requestMunicipalOfferingsSearch(null, 0);
    }

    clearCustomerRequest() {
        this.requestCustomerSearch(null, 0);
    }

    clearSiteRequest() {
        this.requestSiteSearch(null, 0);
    }

    clearContractRequest() {
        this.requestContractSearch(null, 0);
    }

    private setUpGlobalSearchStream() {
        this.globalResults$ = this.globalResults.asObservable();
        this.globalSearchRequest
            .pipe(
                distinctUntilChanged(),
                takeUntil(this.unsubscribe),
                debounceTime(300),
                switchMap((data) => {
                    const searchTerm: string = data[0];
                    if (isTruthy(searchTerm) && (this.authorisationService.hasAuthorisation(FrameworkAuthorisationClaimNames.customerFeature) || environment.applicationType !== 'csr')) {
                        this.loading = true;
                        return this.dataService.doGlobalSearch(searchTerm, data[1]);
                    } else {
                        return of(new GlobalSearch());
                    }
                })
            )
            .subscribe(
                (result: GlobalSearch) => {
                    this.globalResults.next(result);
                }
            );
    }

    private setUpMunicipalOfferingsSearchStream() {
        this.municipalOfferingsResults$ = this.municipalOfferingsResults.asObservable();
        this.municipalOfferingsSearchRequest
            .pipe(
                distinctUntilChanged((data1, data2) => data1.term === data2.term && data1.page === data2.page && data1.businessTypeId === data2.businessTypeId),
                takeUntil(this.unsubscribe),
                debounceTime(300),
                switchMap((data: { term: string; page: number; businessTypeId?: number }) => {
                    if (data.term != null) {
                        this.loading = true;
                        return this.dataService.doMunicipalOfferingsSearch(data.term, data.page, data.businessTypeId);
                    } else {
                        return of(new CountCollectionModel<MunicipalOfferingsSearchResult>([], 0));
                    }
                })
            )
            .subscribe(
                (result: CountCollectionModel<MunicipalOfferingsSearchResult>) => {
                    this.municipalOfferingsResults.next(result);
                }
            );
    }

    private setUpCustomerSearchStream() {
        this.customerResults$ = this.customerResults.asObservable();
        this.customerSearchRequest
            .pipe(
                distinctUntilChanged((data1, data2) => data1.term === data2.term && data1.page === data2.page && data1.businessTypeId === data2.businessTypeId),
                takeUntil(this.unsubscribe),
                debounceTime(300),
                switchMap((data: { term: string; page: number; businessTypeId?: number }) => {
                    if (data.term != null && (this.authorisationService.hasAuthorisation(FrameworkAuthorisationClaimNames.customerFeature) || environment.applicationType !== 'csr')) {
                        this.loading = true;
                        return this.dataService.doCustomerSearch(data.term, data.page, data.businessTypeId);
                    } else {
                        return of(new CountCollectionModel<CustomerSearchResult>([], 0));
                    }
                })
            )
            .subscribe(
                (result: CountCollectionModel<CustomerSearchResult>) => {
                    this.customerResults.next(result);
                }
            );
    }

    private setUpSiteSearchStream() {
        this.siteResults$ = this.siteResults.asObservable();
        this.siteSearchRequest
            .pipe(
                distinctUntilChanged((data1, data2) => data1.term === data2.term && data1.page === data2.page && data1.businessTypeId === data2.businessTypeId),
                takeUntil(this.unsubscribe),
                debounceTime(300),
                switchMap((data: { term: string; page: number; businessTypeId?: number }) => {
                    if (data.term != null && (this.authorisationService.hasAuthorisation(FrameworkAuthorisationClaimNames.customerFeature) || environment.applicationType !== 'csr')) {
                        this.loading = true;
                        return this.dataService.doSiteSearch(data.term, data.page, data.businessTypeId);
                    } else {
                        return of(new CountCollectionModel<ServiceLocationSearchResult>([], 0));
                    }
                })
            ).subscribe(
                (result: CountCollectionModel<ServiceLocationSearchResult>) => {
                    this.siteResults.next(result);
                }
            );
    }

    private setUpContractSearchStream() {
        this.contractResults$ = this.contractResults.asObservable();
        this.contractSearchRequest
            .pipe(
                distinctUntilChanged((data1, data2) => data1.term === data2.term && data1.page === data2.page && data1.businessTypeId === data2.businessTypeId),
                takeUntil(this.unsubscribe),
                debounceTime(300),
                switchMap((data: { term: string; page: number; businessTypeId?: number }) => {
                    if (data.term != null && (this.authorisationService.hasAuthorisation(FrameworkAuthorisationClaimNames.customerFeature) || environment.applicationType !== 'csr')) {
                        this.loading = true;
                        return this.dataService.doContractSearch(data.term, data.page, data.businessTypeId);
                    } else {
                        return of(new CountCollectionModel<ContractSearchResult>([], 0));
                    }
                })
            )
            .subscribe(
                (result: CountCollectionModel<ContractSearchResult>) => {
                    this.contractResults.next(result);
                }
            );
    }

    private setUpBusinessTypes() {
        this.businessTypes$ = this.businessTypeSubject.asObservable();
        this.requestBusinessTypeSubject.pipe(
            takeUntil(this.unsubscribe),
            switchMap(() => {
                return this.dataService.getBusinessTypes();
            })).subscribe(data => {
                this.businessTypeSubject.next(data);
            });
    }
}
