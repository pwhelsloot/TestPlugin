import { Injectable } from '@angular/core';
import { Country } from '@core-module/models/lookups/country.model';
import { Observable } from 'rxjs';
import { publishReplay, refCount, tap } from 'rxjs/operators';
import { CountryServiceData } from './country.service.data';

/**
 * @deprecated Move to PlatformUI + ScaleUI ( https://dev.azure.com/amcsgroup/Platform/_workitems/edit/188268) + TDM
 */
@Injectable()
export class CountryService {

    static providers = [CountryService, CountryServiceData];
    loading = false;

    constructor(private dataService: CountryServiceData) {
    }

    private defaultCountry: Observable<Country> = null;

    // A bit of a simplier style of observable. The first caller of getDefaultCountry will make an API call,
    // all others will simply get a 'replay' style observable with the result
    getDefaultCountry(): Observable<Country> {
        if (this.defaultCountry == null) {
            this.loading = true;
            this.defaultCountry = this.dataService.getDefaultCountry().pipe(publishReplay(1), refCount(), tap(() => this.loading = false));
        }
        return this.defaultCountry;
    }
}
