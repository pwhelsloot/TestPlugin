import { Injectable } from '@angular/core';
import { BrregSearchResult } from '@core-module/models/address/brreg-search-result.model';
import { AddressValidationSearchResult } from '@core-module/models/external-dependencies/address-validation-search-result.model';
import { AddressValidationSearch } from '@core-module/models/external-dependencies/address-validation-search.model';
import { EMPTY, Observable, Subject } from 'rxjs';
import { debounceTime, switchMap, take, takeUntil } from 'rxjs/operators';
import { ApplicationConfigurationStore } from '../config/application-configuration.store';
import { BaseService } from './../base.service';
import { AddressValidationServiceData } from './address-validation.service.data';

/**
 * @deprecated Move to PlatformUI + ScaleUI (or new module) https://dev.azure.com/amcsgroup/Platform/_workitems/edit/188268
 */
@Injectable()
export class AddressValidationService extends BaseService {

    addressSearchResults$: Observable<AddressValidationSearchResult[]>;
    addressReverseSearchResults$: Observable<AddressValidationSearchResult[]>;
    brregAddressSearchResult$: Observable<BrregSearchResult>;

    // with some search providers the full address is not supplied with the search results.
    // in that case, a second call can be made to get a search result object with the full address.
    address$: Observable<AddressValidationSearchResult>;
    searchingAddress = false;

    constructor(private dataService: AddressValidationServiceData,
        private configStore: ApplicationConfigurationStore) {
        super();
        this.setUpAddressSearchStream();
        this.setUpAddressReverseSearchStream();
        this.setupAddressStream();
        this.setupExternalAddressSearchStream();
    }

    private addressSearchResults: Subject<AddressValidationSearchResult[]> = new Subject<AddressValidationSearchResult[]>();
    private addressSearchResultsRequest: Subject<{ searchText: string; countryId: number }> = new Subject<{ searchText: string; countryId: number }>();
    private addressSubject = new Subject<AddressValidationSearchResult>();

    private addressReverseSearchResults: Subject<AddressValidationSearchResult[]> = new Subject<AddressValidationSearchResult[]>();
    private addressReverseSearchResultsRequest: Subject<{ latitude: number; longitude: number; countryId: number }> = new Subject<{ latitude: number; longitude: number; countryId: number }>();
    private addressRequestSubject = new Subject<[string, number]>();

    private brregAddressSearchResultSubject: Subject<BrregSearchResult> = new Subject<BrregSearchResult>();
    private brregAddressSearchResultRequest: Subject<[string, number]> = new Subject<[string, number]>();

    requestExternalAddressSearchResults(entry: string, type: number) {
        this.brregAddressSearchResultRequest.next([entry, type]);
    }

    requestAddressSearchResults(text: string, country: number) {
        this.searchingAddress = true;
        this.addressSearchResultsRequest.next({ searchText: text, countryId: country });
    }

    requestAddressReverseSearchResults(latitude: number, longitude: number, country: number) {
        this.addressReverseSearchResultsRequest.next({ latitude, longitude, countryId: country });
    }

    requestAddress(href: string, countryId: number) {
        this.addressRequestSubject.next([href, countryId]);
    }

    private setupExternalAddressSearchStream() {
        this.brregAddressSearchResult$ = this.brregAddressSearchResultSubject.asObservable();

        this.configStore.configuration$.pipe(take(1)).subscribe(config => {
            if (config.brregIntegrationEnabled) {
                this.brregAddressSearchResultRequest.pipe(
                    takeUntil(this.unsubscribe),
                    switchMap(data => {
                        const brregNumber = data[0];
                        const brregType = data[1];

                        return this.dataService.getBrregAddressSearch(brregNumber, brregType);
                    })
                ).subscribe((data: BrregSearchResult) => {
                    this.brregAddressSearchResultSubject.next(data);
                });
            }
        });
    }

    private setUpAddressReverseSearchStream() {
        this.addressReverseSearchResults$ = this.addressReverseSearchResults.asObservable();
        this.addressReverseSearchResultsRequest
            .pipe(
                takeUntil(this.unsubscribe),
                debounceTime(300),
                switchMap((request: { latitude: number; longitude: number; countryId: number }) => {
                    if (request.latitude != null && request.longitude != null) {
                        return this.dataService.getAddressReverseSearch(request.latitude, request.longitude, request.countryId);
                    } else {
                        return EMPTY;
                    }
                })
            )
            .subscribe((result: AddressValidationSearch) => {
                this.addressReverseSearchResults.next(result.searchResults);
            });
    }

    private setUpAddressSearchStream() {
        this.addressSearchResults$ = this.addressSearchResults.asObservable();
        this.addressSearchResultsRequest
            .pipe(
                takeUntil(this.unsubscribe),
                debounceTime(300),
                switchMap((request: { searchText: string; countryId: number }) => {
                    if (request.searchText != null && request.searchText !== '') {
                        return this.dataService.getAddressSearch(request.searchText, request.countryId);
                    } else {
                        this.searchingAddress = false;
                        this.addressSearchResults.next(null);
                        return EMPTY;
                    }
                }),
            )
            .subscribe((result: AddressValidationSearch) => {
                this.addressSearchResults.next(result.searchResults);
                this.searchingAddress = false;
            });
    }

    private setupAddressStream() {
        this.address$ = this.addressSubject.asObservable();
        this.addressRequestSubject
            .pipe(
                takeUntil(this.unsubscribe),
                switchMap((addressrequestdata: [string, number]) => {
                    return this.dataService.getAddress(addressrequestdata[0], addressrequestdata[1]);
                })
            )
            .subscribe((result: AddressValidationSearchResult) => {
                this.addressSubject.next(result);
            });
    }
}
