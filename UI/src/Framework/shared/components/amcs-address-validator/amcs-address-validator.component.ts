import { Component, ElementRef, Input, OnInit, Renderer2, ViewEncapsulation } from '@angular/core';
import { AddressControlTypeEnum } from '@core-module/models/address/address-control-type.enum';
import { AddressSearchMethod } from '@core-module/models/address/address-search-method.enum';
import { BrregAddressSearchType } from '@core-module/models/address/brreg-address-search-type.enum';
import { AddressValidationFormService } from '@coreservices/address/address-validation-form.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

/**
 * @deprecated Move to PlatformUI + ScaleUI (or new module) https://dev.azure.com/amcsgroup/Platform/_workitems/edit/188268
 */
@Component({
    selector: 'app-amcs-address-validator',
    templateUrl: './amcs-address-validator.component.html',
    styleUrls: ['./amcs-address-validator.component.scss'],
    encapsulation: ViewEncapsulation.None
    // please don't provide any form stuff here
})
export class AmcsAddressValidatorComponent extends AutomationLocatorDirective implements OnInit {

    @Input('addressSearchType') addressSearchType: BrregAddressSearchType;

    AddressSearchMethod = AddressSearchMethod;
    AddressControlTypeEnum = AddressControlTypeEnum;

    // formService to be provided by the component using this one, NOT THIS ONE.
    constructor(
        protected elRef: ElementRef, protected renderer: Renderer2,
        public formService: AddressValidationFormService
    ) {
        super(elRef, renderer);
    }

    ngOnInit() {
        this.formService.currentSearchMethod = this.formService.brregIntegrationEnabled ? AddressSearchMethod.Brreg : AddressSearchMethod.Default;
    }

    onAddressSearchTypeChanged(event: any) {
        this.formService.currentSearchMethod = +event.target.value === AddressSearchMethod.Brreg ? AddressSearchMethod.Brreg : AddressSearchMethod.Default;
        this.formService.resetAddressValidationSearchResult(true);
    }
}
