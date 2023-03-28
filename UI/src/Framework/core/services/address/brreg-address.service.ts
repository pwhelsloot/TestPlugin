import { Injectable } from '@angular/core';
import { BrregAddressContext } from '@core-module/models/address/brreg-address-context.enum';
import { BrregEntityContext } from '@core-module/models/address/brreg-entity-context.model';
import { AddressValidationSearchResult } from '@core-module/models/external-dependencies/address-validation-search-result.model';
import { BehaviorSubject, Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { BaseService } from '../base.service';
import { RouteContextService } from '../route-context.service';

/**
 * @deprecated Move to PlatformUI + ScaleUI (or new module) https://dev.azure.com/amcsgroup/Platform/_workitems/edit/188268
 */
@Injectable()
export class BrregAddressService extends BaseService {

    brregAddressContext$: Observable<BrregEntityContext>;
    secondarySiteAddress$: Observable<AddressValidationSearchResult>;
    currentSecondarySiteAddressContext$: Observable<AddressValidationSearchResult>;

    constructor(private routeContextService: RouteContextService) {
        super();
        this.setupBrregAddressContextStream();
        this.setupSecondarySiteAddressStream();
    }

    private brregAddressContextSubject: BehaviorSubject<BrregEntityContext> = new BehaviorSubject<BrregEntityContext>(null);
    private secondarySiteAddressSubject: BehaviorSubject<AddressValidationSearchResult> = new BehaviorSubject<AddressValidationSearchResult>(null);
    private currentSecondarySiteAddressContextSubject: BehaviorSubject<AddressValidationSearchResult> = new BehaviorSubject<AddressValidationSearchResult>(null);

    setBrregAddressContext(entity: BrregEntityContext) {
        this.brregAddressContextSubject.next(entity);
    }

    setSecondarySite(secondaryAddress: AddressValidationSearchResult) {
        this.secondarySiteAddressSubject.next(secondaryAddress);
    }

    // The difference between this and secondarySiteAddress$ is that the latter is for determining whether or not we
    // we redirect the user to service location wizard based on whether there's a value present, whereas this is to cache
    // the secondary address returned via the BRREG search
    setCurrentSecondarySiteAddressContext(secondaryAddress: AddressValidationSearchResult) {
        this.currentSecondarySiteAddressContextSubject.next(secondaryAddress);
    }

    setRedirectParameters() {
        this.secondarySiteAddressSubject.pipe(take(1)).subscribe(secondaryAddress => {
            this.routeContextService.setValue(BrregAddressContext.UseExistingAddress, 'true');
            this.routeContextService.setValue(BrregAddressContext.HouseNumber, secondaryAddress.houseNumber);
            this.routeContextService.setValue(BrregAddressContext.Address1, secondaryAddress.address1);
            this.routeContextService.setValue(BrregAddressContext.Address2, secondaryAddress.address2);
            this.routeContextService.setValue(BrregAddressContext.Address3, secondaryAddress.address3);
            this.routeContextService.setValue(BrregAddressContext.Address4, secondaryAddress.address4);
            this.routeContextService.setValue(BrregAddressContext.Address5, secondaryAddress.address5);
            this.routeContextService.setValue(BrregAddressContext.Postcode, secondaryAddress.postcode);
            this.routeContextService.setValue(BrregAddressContext.FederalId, secondaryAddress.federalId);
            this.routeContextService.setValue(BrregAddressContext.MunicipalityNo, secondaryAddress.municipalityNo);
            this.routeContextService.setValue(BrregAddressContext.SicCode, secondaryAddress.sicCode);
            this.routeContextService.setValue(BrregAddressContext.SiteName, secondaryAddress.siteName);
        });
    }

    private setupBrregAddressContextStream() {
        this.brregAddressContext$ = this.brregAddressContextSubject.asObservable();
    }

    private setupSecondarySiteAddressStream() {
        this.secondarySiteAddress$ = this.secondarySiteAddressSubject.asObservable();
        this.currentSecondarySiteAddressContext$ = this.currentSecondarySiteAddressContextSubject.asObservable();
    }
}
